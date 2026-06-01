## 1. Runtime Attributes

- [x] 1.1 Add `MessagePipeType` enum to `DangFugBixs.Runtime` (Publisher, Subscriber, Handler)
- [x] 1.2 Create `MessagePipeBrokerInfo` model in `DangFugBixs.Common` for tracking broker metadata
- [x] 1.3 Update `ServiceInfo.cs` to include `MessagePipeType` and `BrokerMessageType` fields
- [x] 1.4 Add `ReportMetadata` model for DI visualizer report fields

## 2. MessagePipe Generator Support

- [x] 2.1 Update `ClassAnalyzer.ExtractInfo()` to detect `[AutoRegisterMessageBrokerIn]` attribute
- [x] 2.2 Implement MessagePipe type detection (`IPublisher<T>`, `ISubscriber<T>`) using symbol matching
- [x] 2.3 Update `RegistrationEmitter` to emit `builder.RegisterMessageBroker<T>()` for broker services
- [x] 2.4 Ensure MessagePipe registration follows scope mapping (same scope as service registration)
- [x] 2.5 Handle multiple broker attributes on same type (emit for each scope)

## 3. DI Visualizer Report

- [x] 3.1 Create `ReportEmitter.cs` for generating `RegistrationReport.g.cs`
- [x] 3.2 Emit const fields: `ServiceCount`, `ScopeCount`, `Scopes[]`, `Entries[]`
- [x] 3.3 Emit MessagePipe broker metadata: `Brokers[]` array with message type info
- [x] 3.4 Emit logger metadata: `LoggerRoots[]`, `LoggerConsumers[]` for ND009 validation
- [x] 3.5 Update generator to call `ReportEmitter` after code emission

## 4. Cross-Scope Analyzer (ND006)

- [x] 4.1 Create `ND006` diagnostic descriptor in `DangFugBixs.Analyzers`
- [x] 4.2 Build scope graph from `[AutoRegisterIn]` and `[LifetimeScopeFor]` attributes
- [x] 4.3 Implement cross-scope dependency traversal logic
- [x] 4.4 Detect invalid scope dependencies and report ND006
- [x] 4.5 Handle parent→child scope validity (VContainer hierarchy)
- [x] 4.6 Handle identity mapping suppression (`[LifetimeScopeFor]` bridge)

## 5. Shared Semantic Helpers

- [x] 5.1 Move `SemanticScopeUtils` to `DangFugBixs.Common` (shared by generator and analyzers)
- [x] 5.2 Add `IsScopeReachable()` helper for scope graph traversal
- [x] 5.3 Add `TryGetMessagePipeDependency()` for symbol-based detection
- [x] 5.4 Update generator to use shared `SemanticScopeUtils`
- [x] 5.5 Update analyzers (ND006, ND008, ND009) to use shared helpers

## 6. Testing

- [x] 6.1 Create generator tests for MessagePipe broker registration
- [x] 6.2 Create generator tests for report emission (verify `RegistrationReport.g.cs` fields)
- [x] 6.3 Create ND006 analyzer tests for invalid cross-scope dependencies
- [x] 6.4 Create ND006 tests for valid parent→child dependencies
- [x] 6.5 Create ND006 tests for identity mapping suppression
- [x] 6.6 Update smoke validation tests to verify report metadata
- [x] 6.7 Run all tests and verify 100% pass rate

## 7. Documentation & Release

- [x] 7.1 Update `CHANGELOG.md` with v5.0.0 release notes
- [x] 7.2 Update `README.md` with MessagePipe usage examples
- [x] 7.3 Document ND006 diagnostic code in README
- [x] 7.4 Document DI Visualizer report usage
- [x] 7.5 Update `package.json` version to 5.0.0
- [x] 7.6 Run full build pipeline (`dotnet build`)
- [x] 7.7 Create GitHub release v5.0.0 with changelog

---

**All v5.0.0 tasks complete!** Released on 2026-03-18.

## 8. v5.1.0 Tooling Enhancements

### Preflight CLI
- [x] 8.1 Create `DangFugBixs.Cli` project structure
- [x] 8.2 Implement `PreflightCommand` with build + smoke orchestration
- [x] 8.3 Add `--format`, `--output`, `--clean` CLI flags
- [x] 8.4 Integrate with existing `DiSmokeValidation` tool
- [x] 8.5 Add unit tests for CLI orchestration

### Runtime Resolve Smoke
- [x] 8.6 Create `ResolveValidator` class with reflection-based resolution
- [x] 8.7 Implement MonoBehaviour filtering (skip Unity types)
- [x] 8.8 Add scope hierarchy validation (parent→child)
- [x] 8.9 Implement coverage statistics reporting
- [x] 8.10 Add unit tests with mock services

### Code Fix Providers
- [x] 8.11 Create `MessagePipeBrokerCodeFixProvider` for ND008
- [x] 8.12 Create `ILoggerRootCodeFixProvider` for ND009
- [x] 8.13 Create `ViewComponentCodeFixProvider` for ND110
- [x] 8.14 Add unit tests for each code fix provider
- [x] 8.15 Test in Visual Studio / Rider IDE

### Release v5.1.0
- [x] 8.16 Update `CHANGELOG.md` with v5.1.0 release notes
- [x] 8.17 Update `README.md` with Preflight CLI usage
- [x] 8.18 Document Code Fix Providers in README
- [x] 8.19 Update `package.json` version to 5.1.0
- [x] 8.20 Run full build pipeline and all tests
- [x] 8.21 Create GitHub release v5.1.0

---

**v5.1.0 Status:** ✅ COMPLETE - Released 2026-03-18

## 9. v5.2.0 Enhanced Diagnostics & Reports

### New Diagnostics
- [x] 9.1 Add ND111 diagnostic (Missing Contract Registration)
- [x] 9.2 Add ND112 diagnostic (Duplicate Contract Registration)
- [x] 9.3 Add ND113 diagnostic (Scene View Binding Mismatch)

### Enhanced Reports
- [x] 9.4 Update ReportEmitter with summary statistics
- [x] 9.5 Add scope-based grouping with service counts
- [x] 9.6 Add EntryPoint detection with indicators
- [x] 9.7 Add Contracts display (explicit vs auto-detected)
- [x] 9.8 Add Entry Points summary section

### Release v5.2.0
- [x] 9.9 Update CHANGELOG.md with v5.2.0 release notes
- [x] 9.10 Update README.md with new diagnostics
- [x] 9.11 Update package.json version to 5.2.0
- [x] 9.12 Run full build pipeline and all tests
- [x] 9.13 Create GitHub release v5.2.0

---

**v5.2.0 Status:** ✅ COMPLETE - Released 2026-03-18

---

## ✅ Summary

| Version | Status | Release Date | Key Features |
|---------|--------|--------------|--------------|
| **v5.0.0** | ✅ Complete | 2026-03-18 | MessagePipe, ND006, DI Visualizer |
| **v5.1.0** | ✅ Complete | 2026-03-18 | CLI, Code Fixes, Smoke Test |
| **v5.2.0** | ✅ Complete | 2026-03-18 | ND111-113, Enhanced Reports |

**All v5.x tasks complete!** Ready to archive this change and start v6.0.0 planning.
