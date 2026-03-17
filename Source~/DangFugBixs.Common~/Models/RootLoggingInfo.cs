namespace NhemDangFugBixs.Common.Models;

internal readonly struct RootLoggingInfo {
    public string ScopeName { get; }
    public bool HasLoggerFactory { get; }
    public bool HasLoggerAdapter { get; }

    public RootLoggingInfo(string scopeName, bool hasLoggerFactory, bool hasLoggerAdapter) {
        ScopeName = scopeName;
        HasLoggerFactory = hasLoggerFactory;
        HasLoggerAdapter = hasLoggerAdapter;
    }
}
