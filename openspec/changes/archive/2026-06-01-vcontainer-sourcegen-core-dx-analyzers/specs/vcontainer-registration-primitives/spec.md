## ADDED Requirements

### Requirement: Support generic and non-generic AutoRegisterIn scope declarations
The attributes package SHALL support both `AutoRegisterIn<TScope>` and `AutoRegisterIn(Type scopeMarkerType)` forms with equivalent semantic output.

#### Scenario: Generic marker form
- **WHEN** a service uses `[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]`
- **THEN** the generator SHALL produce a scoped registration for the implementation

#### Scenario: Type marker form
- **WHEN** a service uses `[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]`
- **THEN** the generator SHALL produce equivalent scoped registration behavior

### Requirement: Support explicit contract and self binding primitives
The attributes package SHALL support `As<TContract>` and `AsSelf` for explicit contract exposure.

#### Scenario: Explicit contract only
- **WHEN** service metadata indicates `As<ICombatCoreService>` and no `AsSelf`
- **THEN** generated registration SHALL expose the implementation via the declared contract only

### Requirement: Support component-in-hierarchy registration intent
The attributes package SHALL support `RegisterComponentInHierarchy` intent and emit `RegisterComponentInHierarchy<T>()` for eligible component registrations.

#### Scenario: MonoBehaviour component registration
- **WHEN** a component registration intent targets a type derived from `MonoBehaviour`
- **THEN** generated code SHALL emit `builder.RegisterComponentInHierarchy<T>()`

### Requirement: Support keyed registration intent
The attributes package SHALL support `Keyed` registration metadata and emit `.Keyed(...)` in generated registration chains.

#### Scenario: Enum key
- **WHEN** a service is decorated with keyed enum metadata
- **THEN** generated registration SHALL include `.Keyed(<enum-value>)`

#### Scenario: String key
- **WHEN** a service is decorated with keyed string metadata
- **THEN** generated registration SHALL include `.Keyed("<key>")`
