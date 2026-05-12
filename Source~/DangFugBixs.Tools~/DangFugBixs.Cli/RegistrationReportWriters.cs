using System.Text;
using System.Text.Json;

namespace DangFugBixs.Cli;

public static class RegistrationReportWriters {
    public static string WriteList(RegistrationReportSnapshot snapshot, string? scopeFilter = null) {
        var entries = FilterEntries(snapshot, scopeFilter);
        var sb = new StringBuilder();
        var title = string.IsNullOrWhiteSpace(scopeFilter) ? "All scopes" : scopeFilter;
        sb.AppendLine($"DI services for: {title}");
        sb.AppendLine();

        foreach (var group in entries.GroupBy(e => e.Scope).OrderBy(g => g.Key, StringComparer.Ordinal)) {
            sb.AppendLine($"[{group.Key}]");
            foreach (var entry in group.OrderBy(e => e.Service, StringComparer.Ordinal)) {
                sb.AppendLine($"- {GetSimpleName(entry.Service)} | {entry.Lifetime} | {entry.Kind}");
            }
            sb.AppendLine();
        }

        if (entries.Count == 0) {
            sb.AppendLine("No matching services found.");
        }

        return sb.ToString().TrimEnd();
    }

    public static string WriteMermaidGraph(RegistrationReportSnapshot snapshot, string? scopeFilter = null) {
        var entries = FilterEntries(snapshot, scopeFilter);
        var consumers = FilterConsumers(snapshot, scopeFilter);
        var sb = new StringBuilder();
        sb.AppendLine("flowchart TD");

        foreach (var group in entries.GroupBy(e => e.Scope).OrderBy(g => g.Key, StringComparer.Ordinal)) {
            sb.AppendLine($"    subgraph {SanitizeId(group.Key)}[\"{Escape(group.Key)}\"]");
            foreach (var entry in group.OrderBy(e => e.Service, StringComparer.Ordinal)) {
                var nodeId = SanitizeId(entry.Scope + "_" + entry.Service);
                sb.AppendLine($"        {nodeId}[\"{Escape(GetSimpleName(entry.Service))}\\n{Escape(entry.Lifetime)}\\n{Escape(entry.Kind)}\"]");
            }
            sb.AppendLine("    end");
        }

        foreach (var consumer in consumers.OrderBy(c => c.Service, StringComparer.Ordinal)) {
            var serviceId = SanitizeId(consumer.Scope + "_" + consumer.Service);
            var eventId = SanitizeId("event_" + consumer.MessageType);
            sb.AppendLine($"    {eventId}([\"{Escape(GetSimpleName(consumer.MessageType))}\"])");
            sb.AppendLine($"    {serviceId} -->|{Escape(consumer.Role)}| {eventId}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string WriteJson(RegistrationReportSnapshot snapshot, string? scopeFilter = null) {
        var entries = FilterEntries(snapshot, scopeFilter);
        var consumers = FilterConsumers(snapshot, scopeFilter);
        var loggerRoots = string.IsNullOrWhiteSpace(scopeFilter)
            ? snapshot.LoggerRoots
            : snapshot.LoggerRoots.Where(root => string.Equals(root.Scope, scopeFilter, StringComparison.OrdinalIgnoreCase)).ToArray();
        var loggerConsumers = string.IsNullOrWhiteSpace(scopeFilter)
            ? snapshot.LoggerConsumers
            : snapshot.LoggerConsumers.Where(consumer => string.Equals(consumer.Scope, scopeFilter, StringComparison.OrdinalIgnoreCase)).ToArray();

        return JsonSerializer.Serialize(new {
            assemblyPath = snapshot.AssemblyPath,
            scopeFilter,
            scopes = entries.Select(entry => entry.Scope).Distinct(StringComparer.Ordinal).OrderBy(scope => scope, StringComparer.Ordinal).ToArray(),
            entries,
            consumers,
            loggerRoots,
            loggerConsumers
        }, new JsonSerializerOptions {
            WriteIndented = true
        });
    }

    public static string WriteMarkdown(RegistrationReportSnapshot snapshot, string? scopeFilter = null) {
        if (string.IsNullOrWhiteSpace(scopeFilter)) {
            return snapshot.Markdown;
        }

        var entries = FilterEntries(snapshot, scopeFilter);
        var sb = new StringBuilder();
        sb.AppendLine($"# DI Registration Report - {scopeFilter}");
        sb.AppendLine();
        sb.AppendLine("| Service | Lifetime | Kind |");
        sb.AppendLine("| :--- | :--- | :--- |");
        foreach (var entry in entries.OrderBy(e => e.Service, StringComparer.Ordinal)) {
            sb.AppendLine($"| {GetSimpleName(entry.Service)} | {entry.Lifetime} | {entry.Kind} |");
        }
        return sb.ToString().TrimEnd();
    }

    private static List<RegistrationReportEntry> FilterEntries(RegistrationReportSnapshot snapshot, string? scopeFilter) {
        var entries = snapshot.Entries.ToList();
        if (!string.IsNullOrWhiteSpace(scopeFilter)) {
            entries = entries
                .Where(entry => string.Equals(entry.Scope, scopeFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return entries;
    }

    private static List<RegistrationReportConsumer> FilterConsumers(RegistrationReportSnapshot snapshot, string? scopeFilter) {
        var consumers = snapshot.Consumers.ToList();
        if (!string.IsNullOrWhiteSpace(scopeFilter)) {
            consumers = consumers
                .Where(consumer => string.Equals(consumer.Scope, scopeFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return consumers;
    }

    private static string GetSimpleName(string value) {
        return string.IsNullOrWhiteSpace(value) || !value.Contains('.', StringComparison.Ordinal)
            ? value
            : value.Split('.').Last();
    }

    private static string SanitizeId(string value) {
        return new string(value.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
    }

    private static string Escape(string value) {
        return value.Replace("\"", "\\\"");
    }
}
