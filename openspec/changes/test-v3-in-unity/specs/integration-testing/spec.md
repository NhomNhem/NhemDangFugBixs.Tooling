## ADDED Requirements

### Requirement: Type-Safe Scope Registration in Unity
The generator SHALL correctly process `[AutoRegisterIn(typeof(TScope))]` attributes in a Unity project.

#### Scenario: Generic attribute generates registration
- **WHEN** user writes `[AutoRegisterIn(typeof(GameplayLifetimeScope))]` on a class in Unity
- **THEN** generator creates registration in `RegisterGameplay()` method

#### Scenario: Convention-based naming works
- **WHEN** scope type is `GameplayLifetimeScope`
- **THEN** generated method is `RegisterGameplay()` (not `RegisterGameplayLifetimeScope()`)

### Requirement: Parent-Child Scope Injection in Unity
The system SHALL support parent scope services being injected into child scope services.

#### Scenario: Child injects parent service
- **WHEN** child scope service has constructor parameter of parent scope service type
- **THEN** VContainer resolves the dependency successfully at runtime

#### Scenario: Generated code compiles in Unity
- **WHEN** Unity compiles the generated VContainerRegistration.g.cs
- **THEN** no compilation errors related to method calls or types

### Requirement: EntryPoint Detection in Unity
The generator SHALL auto-detect VContainer lifecycle interfaces in Unity project.

#### Scenario: ITickable detected as entry point
- **WHEN** class with `[AutoRegisterIn(typeof(TScope))]` implements `ITickable`
- **THEN** generator uses `RegisterEntryPoint<T>()` instead of `Register<T>()`

#### Scenario: IInitializable detected as entry point
- **WHEN** class with `[AutoRegisterIn(typeof(TScope))]` implements `IInitializable`
- **THEN** generator uses `RegisterEntryPoint<T>()`
