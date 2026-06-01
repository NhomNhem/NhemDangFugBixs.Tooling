using System.Reflection;

namespace DangFugBixs.Cli;

public static class RegistrationReportReader {
    public static RegistrationReportSnapshot Load(string assemblyPath) {
        if (string.IsNullOrWhiteSpace(assemblyPath)) {
            throw new InvalidOperationException("Assembly path is required.");
        }

        if (!File.Exists(assemblyPath)) {
            throw new FileNotFoundException("Assembly not found.", assemblyPath);
        }

        var assembly = Assembly.LoadFrom(assemblyPath);
        var reportType = assembly
            .GetTypes()
            .FirstOrDefault(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name == "RegistrationReport");

        if (reportType == null) {
            throw new InvalidOperationException("No generated RegistrationReport type was found.");
        }

        var scopes = ReadStringArrayField(reportType, "Scopes") ?? Array.Empty<string>();
        var entries = (ReadStringArrayField(reportType, "Entries") ?? Array.Empty<string>())
            .Select(ParseEntry)
            .ToArray();
        var consumers = (ReadStringArrayField(reportType, "Consumers") ?? Array.Empty<string>())
            .Select(ParseConsumer)
            .ToArray();
        var loggerRoots = (ReadStringArrayField(reportType, "LoggerRoots") ?? Array.Empty<string>())
            .Select(ParseLoggerRoot)
            .ToArray();
        var loggerConsumers = (ReadStringArrayField(reportType, "LoggerConsumers") ?? Array.Empty<string>())
            .Select(ParseLoggerConsumer)
            .ToArray();
        var generatedInstallers = (ReadStringArrayField(reportType, "GeneratedInstallers") ?? Array.Empty<string>())
            .Select(ParseGeneratedInstaller)
            .ToArray();
        var graphEvidence = (ReadStringArrayField(reportType, "GraphEvidence") ?? Array.Empty<string>())
            .Select(ParseGraphEvidence)
            .ToArray();
        var markdown = ReadStringField(reportType, "Markdown") ?? string.Empty;

        return new RegistrationReportSnapshot(
            assemblyPath,
            scopes,
            entries,
            consumers,
            loggerRoots,
            loggerConsumers,
            generatedInstallers,
            graphEvidence,
            markdown);
    }

    private static string[]? ReadStringArrayField(Type reportType, string fieldName) {
        return reportType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string[];
    }

    private static string? ReadStringField(Type reportType, string fieldName) {
        return reportType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
    }

    private static RegistrationReportEntry ParseEntry(string value) {
        var parts = value.Split('|');
        return new RegistrationReportEntry(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty,
            parts.ElementAtOrDefault(3) ?? string.Empty,
            parts.ElementAtOrDefault(4) ?? string.Empty);
    }

    private static RegistrationReportConsumer ParseConsumer(string value) {
        var parts = value.Split('|');
        return new RegistrationReportConsumer(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty,
            parts.ElementAtOrDefault(3) ?? string.Empty);
    }

    private static RegistrationLoggerRoot ParseLoggerRoot(string value) {
        var parts = value.Split('|');
        return new RegistrationLoggerRoot(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            bool.TryParse(parts.ElementAtOrDefault(1), out var hasLoggerFactory) && hasLoggerFactory,
            bool.TryParse(parts.ElementAtOrDefault(2), out var hasLoggerAdapter) && hasLoggerAdapter);
    }

    private static RegistrationLoggerConsumer ParseLoggerConsumer(string value) {
        var parts = value.Split('|');
        return new RegistrationLoggerConsumer(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty);
    }

    private static RegistrationGeneratedInstaller ParseGeneratedInstaller(string value) {
        var parts = value.Split('|');
        return new RegistrationGeneratedInstaller(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            bool.TryParse(parts.ElementAtOrDefault(2), out var isNoOp) && isNoOp);
    }

    private static RegistrationGraphEvidence ParseGraphEvidence(string value) {
        var parts = value.Split('|');
        return new RegistrationGraphEvidence(
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty,
            parts.ElementAtOrDefault(3) ?? string.Empty,
            parts.ElementAtOrDefault(4) ?? string.Empty);
    }
}
