using System.Reflection;

namespace NhemDangFugBixs.DiSmokeValidation;

internal sealed class ReflectionSmokeValidator {
    public SmokeValidationResult Validate(SmokeValidationOptions options) {
        var result = new SmokeValidationResult();

        if (string.IsNullOrWhiteSpace(options.AssemblyPath)) {
            result.AddError("No assembly path was provided.");
            return result;
        }

        if (!File.Exists(options.AssemblyPath)) {
            result.AddError($"Assembly not found: {options.AssemblyPath}");
            return result;
        }

        Assembly assembly;
        try {
            assembly = Assembly.LoadFrom(options.AssemblyPath);
        } catch (Exception ex) {
            result.AddError($"Failed to load assembly: {ex.Message}");
            return result;
        }

        var injectorTypes = assembly
            .GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name.Contains("VContainerRegistration", StringComparison.Ordinal))
            .ToList();

        if (injectorTypes.Count == 0) {
            result.AddError("No generated VContainerRegistration type was found.");
            return result;
        }

        if (injectorTypes.Count > 1) {
            result.AddError($"Multiple generated VContainerRegistration types were found ({injectorTypes.Count}).");
        }

        var registrationMethods = injectorTypes
            .SelectMany(GetScopeRegistrationMethods)
            .ToList();

        if (registrationMethods.Count == 0) {
            result.AddWarning("No scoped registration methods were found in generated injector output.");
        }

        var duplicateMethods = registrationMethods
            .GroupBy(m => m.Name, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        foreach (var methodName in duplicateMethods) {
            result.AddError($"Duplicate registration method found: {methodName}.");
        }

        var reportType = assembly
            .GetTypes()
            .FirstOrDefault(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name == "RegistrationReport");

        if (reportType == null) {
            result.AddWarning("No generated RegistrationReport type was found.");
            return result;
        }

        ValidateReportAgainstInjector(result, registrationMethods, reportType);

        return result;
    }

    private static IEnumerable<MethodInfo> GetScopeRegistrationMethods(Type injectorType) {
        return injectorType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m =>
                m.Name.StartsWith("Register", StringComparison.Ordinal) &&
                !string.Equals(m.Name, "RegisterAll", StringComparison.Ordinal));
    }

    private static void ValidateReportAgainstInjector(
        SmokeValidationResult result,
        IReadOnlyCollection<MethodInfo> registrationMethods,
        Type reportType) {
        var reportedServices = ReadIntField(reportType, "ServiceCount");
        var reportedScopes = ReadIntField(reportType, "ScopeCount");
        var scopes = ReadStringArrayField(reportType, "Scopes");
        var entryStrings = ReadStringArrayField(reportType, "Entries");
        var consumerStrings = ReadStringArrayField(reportType, "Consumers") ?? Array.Empty<string>();
        var loggerRootStrings = ReadStringArrayField(reportType, "LoggerRoots") ?? Array.Empty<string>();
        var loggerConsumerStrings = ReadStringArrayField(reportType, "LoggerConsumers") ?? Array.Empty<string>();

        if (reportedServices == null || reportedScopes == null) {
            result.AddError("RegistrationReport is missing required count fields.");
            return;
        }

        if (entryStrings == null || scopes == null) {
            result.AddError("RegistrationReport is missing required metadata arrays.");
            return;
        }

        var entries = entryStrings
            .Select(ParseEntry)
            .ToList();

        if (entries.Count != reportedServices.Value) {
            result.AddError($"Service count mismatch: report={reportedServices.Value}, entries={entries.Count}.");
        }

        var distinctScopes = entries
            .Select(e => e.Scope)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(scope => scope, StringComparer.Ordinal)
            .ToList();

        if (distinctScopes.Count != reportedScopes.Value) {
            result.AddError($"Scope count mismatch: report={reportedScopes.Value}, entries={distinctScopes.Count}.");
        }

        var declaredScopes = scopes
            .Distinct(StringComparer.Ordinal)
            .OrderBy(scope => scope, StringComparer.Ordinal)
            .ToList();

        if (!declaredScopes.SequenceEqual(distinctScopes, StringComparer.Ordinal)) {
            result.AddError("Scope list mismatch: RegistrationReport.Scopes does not match entry scopes.");
        }

        var reflectedMethods = registrationMethods
            .Select(m => m.Name)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        var expectedMethods = declaredScopes
            .Select(GetRegistrationMethodName)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        foreach (var entry in entries
                     .GroupBy(e => $"{e.Scope}|{e.Service}", StringComparer.Ordinal)
                     .Where(g => g.Count() > 1)) {
            result.AddError($"Duplicate registration entry found: {entry.Key}.");
        }

        foreach (var expectedMethod in expectedMethods.Where(m => !reflectedMethods.Contains(m, StringComparer.Ordinal))) {
            result.AddError($"Missing registration method for scope: {expectedMethod}.");
        }

        foreach (var reflectedMethod in reflectedMethods.Where(m => !expectedMethods.Contains(m, StringComparer.Ordinal))) {
            result.AddError($"Unexpected registration method without report metadata: {reflectedMethod}.");
        }

        ValidateMessagePipeConsumers(result, entries, consumerStrings);
        ValidateLoggerConsumers(result, loggerRootStrings, loggerConsumerStrings);
    }

    private static int? ReadIntField(Type reportType, string fieldName) {
        var field = reportType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        if (field?.GetValue(null) is int value) {
            return value;
        }

        return null;
    }

    private static IReadOnlyList<string>? ReadStringArrayField(Type reportType, string fieldName) {
        var field = reportType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        return field?.GetValue(null) as string[];
    }

    private static ReportEntry ParseEntry(string value) {
        var parts = value.Split('|');
        return new ReportEntry(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty,
            parts.ElementAtOrDefault(3) ?? string.Empty,
            parts.ElementAtOrDefault(4) ?? string.Empty);
    }

    private static void ValidateMessagePipeConsumers(
        SmokeValidationResult result,
        IReadOnlyCollection<ReportEntry> entries,
        IReadOnlyList<string> consumerStrings) {
        var brokers = entries
            .Where(e => string.Equals(e.Kind, "MessageBroker", StringComparison.Ordinal))
            .ToList();

        foreach (var rawConsumer in consumerStrings) {
            var consumer = ParseConsumer(rawConsumer);
            var isSatisfied = brokers.Any(broker =>
                string.Equals(broker.MessageType, consumer.MessageType, StringComparison.Ordinal) &&
                IsScopeReachable(consumer.Scope, broker.Scope));

            if (!isSatisfied) {
                result.AddError(
                    $"Type '{GetSimpleName(consumer.Service)}' in scope '{GetSimpleName(consumer.Scope)}' depends on MessagePipe {consumer.Role}<{GetSimpleName(consumer.MessageType)}>, but no reachable broker registration exists.");
            }
        }
    }

    private static ConsumerEntry ParseConsumer(string value) {
        var parts = value.Split('|');
        return new ConsumerEntry(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty,
            parts.ElementAtOrDefault(3) ?? string.Empty);
    }

    private static void ValidateLoggerConsumers(
        SmokeValidationResult result,
        IReadOnlyList<string> loggerRootStrings,
        IReadOnlyList<string> loggerConsumerStrings) {
        var loggerRoots = loggerRootStrings
            .Select(ParseLoggerRoot)
            .ToList();

        foreach (var loggerConsumer in loggerConsumerStrings.Select(ParseLoggerConsumer)) {
            var hasReachableRoot = loggerRoots.Any(root =>
                root.HasLoggerFactory &&
                root.HasLoggerAdapter &&
                IsScopeReachable(loggerConsumer.Scope, root.Scope));

            if (!hasReachableRoot) {
                result.AddError(
                    $"Type '{GetSimpleName(loggerConsumer.Service)}' in scope '{GetSimpleName(loggerConsumer.Scope)}' depends on ILogger<{GetSimpleName(loggerConsumer.CategoryType)}>, but no reachable root logging setup exists.");
            }
        }
    }

    private static LoggerRootEntry ParseLoggerRoot(string value) {
        var parts = value.Split('|');
        return new LoggerRootEntry(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            bool.TryParse(parts.ElementAtOrDefault(1), out var hasLoggerFactory) && hasLoggerFactory,
            bool.TryParse(parts.ElementAtOrDefault(2), out var hasLoggerAdapter) && hasLoggerAdapter);
    }

    private static LoggerConsumerEntry ParseLoggerConsumer(string value) {
        var parts = value.Split('|');
        return new LoggerConsumerEntry(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty);
    }

    private static string GetRegistrationMethodName(string scopeKey) {
        var name = scopeKey;
        if (name.Contains('.', StringComparison.Ordinal)) {
            name = name.Split('.').Last();
        }

        if (name.EndsWith("LifetimeScope", StringComparison.Ordinal)) {
            name = name[..^"LifetimeScope".Length];
            if (string.IsNullOrEmpty(name)) {
                return "RegisterGlobal";
            }
        }

        return $"Register{name}";
    }

    private static bool IsScopeReachable(string currentScope, string targetScope) {
        if (string.Equals(currentScope, targetScope, StringComparison.Ordinal)) {
            return true;
        }

        return targetScope is "LifetimeScope" or "ProjectLifetimeScope" or "GlobalLifetimeScope";
    }

    private static string GetSimpleName(string value) {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('.', StringComparison.Ordinal)) {
            return value;
        }

        return value.Split('.').Last();
    }

    private readonly record struct ReportEntry(
        string Scope,
        string Service,
        string Lifetime,
        string Kind,
        string MessageType);

    private readonly record struct ConsumerEntry(
        string Scope,
        string Service,
        string Role,
        string MessageType);

    private readonly record struct LoggerRootEntry(
        string Scope,
        bool HasLoggerFactory,
        bool HasLoggerAdapter);

    private readonly record struct LoggerConsumerEntry(
        string Scope,
        string Service,
        string CategoryType);
}
