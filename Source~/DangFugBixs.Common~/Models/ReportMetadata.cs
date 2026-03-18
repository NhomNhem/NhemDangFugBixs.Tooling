namespace NhemDangFugBixs.Common.Models;

/// <summary>
/// Metadata for DI visualizer report generation.
/// </summary>
internal readonly struct ReportMetadata {
    /// <summary>
    /// Total number of services registered.
    /// </summary>
    public int ServiceCount { get; }

    /// <summary>
    /// Total number of scopes.
    /// </summary>
    public int ScopeCount { get; }

    /// <summary>
    /// List of scope names.
    /// </summary>
    public string[] Scopes { get; }

    /// <summary>
    /// List of registration entries in format: "Scope|Service|Lifetime|Kind|MessageType".
    /// </summary>
    public string[] Entries { get; }

    /// <summary>
    /// List of MessagePipe broker entries in format: "Scope|BrokerType|MessageType|BrokerKind".
    /// </summary>
    public string[] Brokers { get; }

    /// <summary>
    /// List of logger root entries in format: "Scope|HasLoggerFactory|HasLoggerAdapter".
    /// </summary>
    public string[] LoggerRoots { get; }

    /// <summary>
    /// List of logger consumer entries in format: "Scope|Service|CategoryType".
    /// </summary>
    public string[] LoggerConsumers { get; }

    public ReportMetadata(
        int serviceCount,
        int scopeCount,
        string[] scopes,
        string[] entries,
        string[] brokers,
        string[] loggerRoots,
        string[] loggerConsumers)
        => (ServiceCount, ScopeCount, Scopes, Entries, Brokers, LoggerRoots, LoggerConsumers) =
           (serviceCount, scopeCount, scopes, entries, brokers, loggerRoots, loggerConsumers);
}
