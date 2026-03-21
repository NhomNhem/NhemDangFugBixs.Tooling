## Why

v5.x delivered 15 diagnostics, CLI tooling, and enhanced reports - but at a cost: build times increased for large projects (100+ services), error messages lack actionable guidance, and maintenance burden grows with each feature. v6.0.0 focuses on **performance optimization**, **developer experience**, and **sustainability** rather than new features.

## What Changes

- **Performance Optimization**: Optimize existing incremental generation pipeline, analyzer hot path optimization, reduced compilation overhead for large projects
- **Enhanced Error Messages**: Diagnostic messages include code snippets, documentation links, and one-click fixes
- **Developer Documentation**: Troubleshooting guide, performance tuning guide, migration guides for common scenarios
- **Code Fix Providers**: Expand from 4 to 9 providers by adding ND111, ND112, ND113, ND006, ND005
- **Maintenance Improvements**: CONTRIBUTING.md, issue templates, automated performance regression tests

**BREAKING**: None - v6.0.0 is fully backward compatible with v5.x

## Capabilities

### New Capabilities
- `performance-optimization`: Incremental pipeline tuning, analyzer caching, build time benchmarks
- `enhanced-diagnostics`: Actionable error messages with code snippets and docs links
- `code-fix-expansion`: Auto-fix providers for common diagnostics (ND005, ND006, ND111, ND112, ND113)
- `developer-documentation`: Troubleshooting guide, performance tuning, migration guides

### Modified Capabilities
- None (all changes are additive improvements, no spec-level behavior changes)

## Impact

**Affected Areas:**
- `Source~/DangFugBixs.Generators~/`: Existing incremental pipeline tuning, caching layer
- `Source~/DangFugBixs.Analyzers~/`: Optimized symbol resolution, cached analysis results
- `Source~/DangFugBixs.Analyzers~/CodeFixes/`: 5 new Code Fix Providers
- `Source~/DangFugBixs.Tools~/`: Performance benchmarking tools
- `docs/`: New troubleshooting and performance guides

**Dependencies:**
- No new external dependencies
- Uses Roslyn incremental generator APIs (already available)

**Migration:**
- No migration required
- Existing `[AutoRegisterIn]` usage unchanged
- Performance improvements are automatic
