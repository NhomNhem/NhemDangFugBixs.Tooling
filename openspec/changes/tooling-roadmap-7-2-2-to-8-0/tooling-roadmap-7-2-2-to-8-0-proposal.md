# Proposal: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Change Name
`tooling-roadmap-7-2-2-to-8-0`

## Overview

This proposal outlines the evolution of NhemDangFugBixs.Tooling from version 7.2.2 through 8.0.0, transforming it into a comprehensive compile-time DI architecture guardrail for Unity + VContainer.

## Vision Statement

**NhemDangFugBixs.Tooling should become a compile-time DI architecture guardrail for Unity + VContainer.**

### Core Principle
- Services declare intent
- Composition owns registration
- Diagnostics protect architecture

### Product Positioning
The package should evolve into Unity DI architecture tooling, not a magic runtime framework.

## Current State

### Completed Milestones
- **7.2.0**: Introduced composition-only generation mode
- **7.2.1**: Cleaned up registration exposure style by making explicit `[As]` / `[AsSelf]` canonical

### Current Capabilities
- Source-generated VContainer registrations
- Roslyn analyzers for contract validation
- Scope marker mapping across Unity asmdef boundaries
- CLI and Unity Editor diagnostics
- Composition-only generation mode for service assemblies

## Problem Statement

While NhemDangFugBixs.Tooling provides strong foundation capabilities, there are gaps in:

1. **Package Integration Diagnostics**: Real Unity projects can accidentally load stale cached package DLLs, causing version mismatches
2. **Analyzer Coverage**: Common architecture mistakes are not caught at compile time
3. **Project-Level Validation**: Roslyn analyzers cannot inspect Unity asmdef structure or package integration
4. **Visibility**: Developers lack visibility into the DI composition graph
5. **Learning Curve**: Limited scenario-focused samples make it hard to learn patterns
6. **Performance**: No performance guarantees for large projects
7. **Migration Barrier**: Existing projects with manual VContainer registration have no migration path
8. **API Clarity**: Legacy exposure flags coexist with canonical explicit attributes, creating confusion

## Proposed Solution

This roadmap addresses these gaps through a series of incremental releases:

### Phase 7.2.2: Import and Package Diagnostics
Add Unity Editor diagnostics menu to detect version mismatches and package integration issues.

### Phase 7.3.0: Analyzer Maturity
Expand analyzer coverage to catch common architecture violations reliably.

### Phase 7.4.0: DI Smoke Project-Level Validation
Provide CLI tool for project-level validation beyond Roslyn boundaries.

### Phase 7.5.0: DI Report and Graph Viewer
Make composition graph visible through JSON report and Unity Editor UI.

### Phase 7.6.0: Sample Suite
Provide focused scenario-based samples to teach concepts.

### Phase 7.7.0: Performance and Scalability Gates
Ensure tooling scales to large projects with performance benchmarking.

### Phase 7.8.0: Migration Assistant
Help users migrate from manual VContainer registration to attribute-driven composition.

### Phase 8.0.0: Clean API Breaking Release
Finalize canonical API by removing legacy exposure flags.

## Non-Goals

The following features are explicitly out of scope:

- RegisterInstance generation
- Addressables integration
- Prefab factory generation
- Pooling integration
- LifetimeScope MonoBehaviour generation
- Partial injection into user LifetimeScope
- Scene auto setup
- MessagePipe auto-magic
- Runtime reflection scanning
- Full loaded Unity assembly scanning

## Benefits

### For Developers
- Earlier detection of architecture violations
- Clear visibility into DI composition
- Easier onboarding through samples
- Migration path from manual registration
- Performance guarantees for large projects
- Cleaner, more explicit API

### For Projects
- Reduced runtime DI errors
- Better architecture enforcement
- Improved debugging capabilities
- Scalable tooling for large codebases
- Consistent patterns across teams

### For Package Maintainers
- Reduced support burden
- Clearer API surface
- Better test coverage
- Performance regression detection
- Easier onboarding for new contributors

## Risks

### Technical Risks
- Performance degradation with large projects
- Analyzer false positives blocking valid code
- di-smoke tool complexity and maintenance burden
- Migration assistant accuracy on complex patterns

### Adoption Risks
- Breaking changes in 8.0.0 may cause user friction
- Users may not adopt canonical style before 8.0.0
- Sample suite maintenance overhead
- Documentation keeping pace with features

## Mitigation Strategies

### Technical Mitigations
- Performance benchmarking in CI/CD
- Analyzer test coverage and user feedback loops
- di-smoke tool modular design for maintainability
- Migration assistant report-only initially, auto-fix later

### Adoption Mitigations
- Clear migration guides and deprecation warnings
- Gradual obsolescence before hard removal in 8.0.0
- Sample suite automated validation in release gate
- Documentation reviewed with each release

## Success Criteria

### Technical Success
- All analyzer tests pass with zero false positives
- di-smoke validation runs in under 5 seconds for medium projects
- DI report generation completes in under 2 seconds
- Generator performance meets or exceeds baseline benchmarks
- Zero breaking changes before 8.0.0

### User Experience Success
- Stale package cache issues reduced by 90%
- Architecture violations caught at compile time vs runtime
- Time to debug DI issues reduced by 50%
- Sample suite adoption rate tracked
- Migration assistant usage feedback positive

## Release Strategy

### Minor Releases (7.2.2 - 7.8.0)
- Additive features only
- No breaking changes
- Backward compatibility maintained
- Incremental improvements to diagnostics and tooling

### Major Release (8.0.0)
- Breaking API changes
- Legacy flag removal
- Canonical API enforcement
- Comprehensive migration guide
- Long deprecation period for legacy patterns

## Timeline

### Estimated Duration
- 7.2.2: 1-2 weeks
- 7.3.0: 2-3 weeks
- 7.4.0: 2-3 weeks
- 7.5.0: 2-3 weeks
- 7.6.0: 2-3 weeks
- 7.7.0: 1-2 weeks
- 7.8.0: 3-4 weeks

**Total**: 13-20 weeks (approximately 3-5 months)

### Dependencies
- 7.2.2: No dependencies
- 7.3.0: Can proceed in parallel with 7.2.2
- 7.4.0: Depends on 7.3.0 (uses analyzer diagnostics)
- 7.5.0: Depends on 7.4.0 (uses di-smoke data)
- 7.6.0: Can proceed in parallel with 7.4.0-7.5.0
- 7.7.0: Can proceed in parallel with 7.4.0-7.6.0
- 7.8.0: Depends on 7.3.0-7.8.0 (builds on all features)

## Deliverables

### Documentation
- roadmap.md
- proposal.md
- design.md
- tasks.md
- per-version acceptance criteria
- non-goals document
- risk register
- release gate impact document
- migration plan

### Implementation
- Unity Editor diagnostics menu (7.2.2)
- Expanded analyzer diagnostics (7.3.0)
- di-smoke CLI tool (7.4.0)
- DI report generation and viewer (7.5.0)
- Sample suite (7.6.0)
- Performance benchmarking (7.7.0)
- Migration assistant (7.8.0)
- Canonical API cleanup (8.0.0)

## Approval Required

This proposal requires approval from:
- Package maintainers
- Technical stakeholders
- User community feedback

## Next Steps

1. Review and approve this proposal
2. Review detailed design documents for each phase
3. Prioritize phases based on community feedback
4. Begin implementation with highest-priority phase
5. Iterate based on user feedback during each phase
