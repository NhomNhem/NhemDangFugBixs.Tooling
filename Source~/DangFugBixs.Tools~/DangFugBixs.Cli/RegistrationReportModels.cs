namespace DangFugBixs.Cli;

public sealed record RegistrationReportEntry(
    string Scope,
    string Service,
    string Lifetime,
    string Kind,
    string MessageType);

public sealed record RegistrationReportConsumer(
    string Scope,
    string Service,
    string Role,
    string MessageType);

public sealed record RegistrationLoggerRoot(
    string Scope,
    bool HasLoggerFactory,
    bool HasLoggerAdapter);

public sealed record RegistrationLoggerConsumer(
    string Scope,
    string Service,
    string CategoryType);

public sealed record RegistrationGeneratedInstaller(
    string Scope,
    string Installer,
    bool IsNoOp);

public sealed record RegistrationGraphEvidence(
    string Scope,
    string Service,
    string SourceAssembly,
    string CompositionAssembly,
    string ReferencePath);

public sealed record RegistrationReportSnapshot(
    string AssemblyPath,
    IReadOnlyList<string> Scopes,
    IReadOnlyList<RegistrationReportEntry> Entries,
    IReadOnlyList<RegistrationReportConsumer> Consumers,
    IReadOnlyList<RegistrationLoggerRoot> LoggerRoots,
    IReadOnlyList<RegistrationLoggerConsumer> LoggerConsumers,
    IReadOnlyList<RegistrationGeneratedInstaller> GeneratedInstallers,
    IReadOnlyList<RegistrationGraphEvidence> GraphEvidence,
    string Markdown);
