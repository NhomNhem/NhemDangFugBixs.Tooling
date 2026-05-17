# Non-Goals: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This document explicitly defines features and capabilities that are **out of scope** for the roadmap from 7.2.2 through 8.0.0. These non-goals help maintain focus on the core vision of NhemDangFugBixs.Tooling as a compile-time DI architecture guardrail.

## Core Vision

**NhemDangFugBixs.Tooling should become a compile-time DI architecture guardrail for Unity + VContainer.**

### Core Principle
- Services declare intent
- Composition owns registration
- Diagnostics protect architecture

### Product Positioning
The package should evolve into Unity DI architecture tooling, not a magic runtime framework.

## Explicit Non-Goals

### 1. RegisterInstance Generation

**Status**: Out of scope for all roadmap phases

**Rationale**:
- RegisterInstance is typically used for configuration, runtime data, or Unity objects
- These patterns are context-dependent and cannot be reliably inferred from attributes
- Manual registration provides better control over instance lifetime and disposal
- Adding RegisterInstance generation would require complex heuristics that could lead to incorrect behavior

**Alternatives**:
- Users should continue to manually call `builder.RegisterInstance()` for configuration and runtime data
- Consider providing documentation and examples for common RegisterInstance patterns
- Future enhancement could provide a migration assistant that suggests RegisterInstance patterns, but not generate them

### 2. Addressables Integration

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Addressables is a complex Unity system with its own lifecycle and loading patterns
- Integrating Addressables with DI requires understanding project-specific asset management strategies
- The tooling should remain focused on VContainer registration, not broader Unity asset management
- Addressables integration would significantly expand scope and complexity

**Alternatives**:
- Users should manually integrate Addressables with VContainer using standard patterns
- Consider providing documentation examples for common Addressables + VContainer patterns
- Future enhancement could provide a separate package or extension for Addressables integration

### 3. Prefab Factory Generation

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Prefab instantiation patterns vary significantly between projects
- Prefab factories often require project-specific configuration and context
- The tooling should focus on service registration, not Unity object creation patterns
- Prefab factory generation would require understanding Unity prefab references and instantiation patterns

**Alternatives**:
- Users should continue to manually create prefab factories
- Consider providing documentation examples for common prefab factory patterns
- VContainer's built-in prefab factory support should be used directly

### 4. Pooling Integration

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Pooling is a complex optimization strategy with project-specific requirements
- Pooling patterns vary significantly (object pooling, memory pooling, etc.)
- The tooling should remain focused on DI registration, not performance optimization patterns
- Pooling integration would require understanding project-specific pooling strategies

**Alternatives**:
- Users should integrate pooling manually with VContainer registration
- Consider providing documentation examples for common pooling patterns
- Future enhancement could provide a separate package or extension for pooling integration

### 5. LifetimeScope MonoBehaviour Generation

**Status**: Out of scope for all roadmap phases

**Rationale**:
- LifetimeScope is a Unity MonoBehaviour that users typically customize heavily
- Generated LifetimeScopes would conflict with user customization
- The tooling should focus on service registration, not Unity component generation
- LifetimeScope generation would require understanding Unity scene hierarchy and composition

**Alternatives**:
- Users should continue to manually create and customize LifetimeScope MonoBehaviours
- The tooling already provides `[LifetimeScopeFor]` for scope marker mapping
- Consider providing templates or snippets for common LifetimeScope patterns

### 6. Partial Injection into User LifetimeScope

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Partial injection into user code is fragile and can break with user changes
- Users should have full control over their LifetimeScope implementation
- The tooling should generate separate registration files, not modify user code
- Partial injection would require complex source manipulation that is error-prone

**Alternatives**:
- Users should call `builder.RegisterGeneratedFor<TScope>()` in their Configure() method
- The tooling already provides clear documentation on how to integrate generated registration
- Consider providing code snippets or templates for common LifetimeScope patterns

### 7. Scene Auto Setup

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Scene setup is project-specific and varies significantly between projects
- Auto scene setup would require understanding project structure and scene composition
- The tooling should remain focused on DI registration, not Unity scene management
- Scene auto setup would be fragile and could break with project changes

**Alternatives**:
- Users should manually set up scenes with required LifetimeScopes
- Consider providing documentation examples for scene setup patterns
- Future enhancement could provide a separate package or extension for scene setup automation

### 8. MessagePipe Auto-Magic

**Status**: Out of scope for all roadmap phases

**Rationale**:
- MessagePipe is a separate library with its own patterns and conventions
- Auto-magic MessagePipe integration would require understanding MessagePipe's API
- The tooling should remain focused on VContainer registration, not third-party library integration
- MessagePipe integration would significantly expand scope

**Alternatives**:
- Users should manually integrate MessagePipe with VContainer using standard patterns
- Consider providing documentation examples for common MessagePipe + VContainer patterns
- Future enhancement could provide a separate package or extension for MessagePipe integration

### 9. Runtime Reflection Scanning

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Runtime reflection scanning is slow and violates compile-time philosophy
- The tooling should use source generation and Roslyn analyzers, not runtime reflection
- Runtime scanning would introduce performance overhead at startup
- Runtime scanning would make the tooling dependent on runtime behavior

**Alternatives**:
- Continue using source generation for compile-time registration
- Continue using Roslyn analyzers for compile-time validation
- Project-level validation (di-smoke) can inspect compiled assemblies without runtime reflection

### 10. Full Loaded Unity Assembly Scanning

**Status**: Out of scope for all roadmap phases

**Rationale**:
- Scanning all loaded Unity assemblies is slow and resource-intensive
- The tooling should only scan directly referenced assemblies
- Full assembly scanning would violate the principle of minimal overhead
- Full assembly scanning would make the tooling dependent on Unity's assembly loading order

**Alternatives**:
- Continue using direct reference scanning for source generation
- di-smoke can inspect asmdef structure without scanning all assemblies
- Users can explicitly reference assemblies they want scanned

## Principles Guiding Non-Goals

### 1. Compile-Time Philosophy
The tooling should operate at compile time using source generation and Roslyn analyzers, not at runtime using reflection.

### 2. Minimal Scope
The tooling should focus on DI registration and architecture validation, not broader Unity patterns or third-party integrations.

### 3. User Control
The tooling should generate code that users integrate, not modify user code directly or make assumptions about project structure.

### 4. Performance
The tooling should have minimal performance impact on compilation and runtime.

### 5. Reliability
The tooling should be reliable and deterministic, not dependent on fragile runtime behavior or complex heuristics.

### 6. Maintainability
The tooling should be maintainable and not become a monolithic framework that tries to do everything.

## Future Considerations

### Potential Future Enhancements (Not Roadmap Commitments)

These features may be considered in future roadmaps but are **not committed** for 7.2.2-8.0.0:

#### Code Fix Providers
- Auto-fix for duplicate contract exposure
- Auto-fix for obsolete flags (planned for 8.0.0)
- Auto-fix for visibility issues

#### Enhanced Migration Assistant
- Auto-fix for manual registrations (currently report-only)
- Batch migration for entire projects
- Undo/redo support for migrations

#### Additional Diagnostics
- Circular dependency detection
- Unused service detection
- Scope isolation validation

#### Enhanced DI Report
- Visual graph visualization
- Dependency tree view
- Real-time updates

#### Performance Optimizations
- Incremental analyzer execution
- Cached analysis results
- Parallel generation

#### Additional Sample Patterns
- MessagePipe integration samples (documentation only)
- Addressables integration samples (documentation only)
- Prefab factory patterns (documentation only)

### Decision Framework for Future Features

When considering future features, evaluate against:

1. **Does it align with compile-time philosophy?**
   - If it requires runtime reflection, it's likely a non-goal

2. **Does it fit within DI architecture guardrail scope?**
   - If it's broader Unity or third-party integration, it's likely a non-goal

3. **Does it maintain user control?**
   - If it modifies user code or makes assumptions, it's likely a non-goal

4. **Does it have acceptable performance?**
   - If it introduces significant overhead, it's likely a non-goal

5. **Is it reliable and deterministic?**
   - If it depends on fragile runtime behavior, it's likely a non-goal

6. **Is it maintainable?**
   - If it significantly expands scope, it's likely a non-goal

## Communication Strategy

### Documenting Non-Goals

- Non-goals are documented in this file
- Non-goals are referenced in the roadmap and proposal
- Non-goals are explained in issue responses when requested

### Responding to Feature Requests

When users request features that are non-goals:

1. Acknowledge the request
2. Explain why it's a non-goal (refer to this document)
3. Provide alternatives or workarounds
4. Consider if the feature should be reconsidered for future roadmaps

### Revisiting Non-Goals

Non-goals may be revisited if:

1. Core vision changes
2. User community provides compelling use cases
3. Technical constraints are removed
4. New information emerges that changes the evaluation

Any reconsideration should follow the decision framework above.

## Conclusion

These non-goals help maintain focus on the core vision of NhemDangFugBixs.Tooling as a compile-time DI architecture guardrail. By explicitly defining what is out of scope, we can ensure the tooling remains focused, maintainable, and aligned with its core principles.

Users should understand that these non-goals are intentional design decisions, not limitations to be worked around. The alternatives provided ensure users can still achieve their goals through manual configuration or separate packages/extensions.
