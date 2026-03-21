# Key Features

- Module/Installer Pattern: Decouple registration logic into IVContainerInstaller classes
- Cross-Layer Discovery: Register services across assemblies using Identity Types
- Type-Safe Scopes: Register services to specific LifetimeScope types with compile-time safety
- Unified Master Registration: Single call to register all discovered services
- Robust Code Emission: Uses partial classes and global:: prefixes
- Compile-Time Validation: Roslyn analyzers catch errors before runtime
- Code Fix Providers: IDE auto-fix for common DI mistakes
- Preflight CLI: dotnet di-smoke command for validation
- Runtime Resolve Smoke: Headless validation of DI container resolution
