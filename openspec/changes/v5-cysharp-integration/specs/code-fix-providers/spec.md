## ADDED Requirements

### Requirement: ND008 Auto-Fix (MessagePipe Broker)
The system SHALL provide a Code Fix Provider for ND008 diagnostics.

#### Scenario: Suggest broker registration snippet
- **WHEN** ND008 is triggered (missing broker registration)
- **THEN** the IDE SHALL offer a code fix:
  1. "Register MessagePipe broker in {ScopeName}"
  2. On apply, insert code snippet:
     ```csharp
     [AutoRegisterMessageBrokerIn(typeof({ScopeName}))]
     public class {MessageTypeName}Broker { }
     ```

#### Scenario: Manual registration alternative
- **WHEN** user prefers manual registration
- **THEN** the code fix SHALL offer:
  1. "Add manual broker registration to scope"
  2. On apply, insert:
     ```csharp
     builder.RegisterMessageBroker<{MessageTypeName}>(Lifetime.Singleton);
     ```

### Requirement: ND009 Auto-Fix (ILogger Root)
The system SHALL provide a Code Fix Provider for ND009 diagnostics.

#### Scenario: Suggest LoggerFactory registration
- **WHEN** ND009 is triggered (missing ILoggerFactory)
- **THEN** the IDE SHALL offer a code fix:
  1. "Add LoggerFactory registration to {ScopeName}"
  2. On apply, insert code snippet:
     ```csharp
     builder.Register<LoggerFactory>(Lifetime.Singleton).As<ILoggerFactory>();
     builder.Register<LoggerAdapter<{CategoryType}>>(Lifetime.Singleton);
     ```

#### Scenario: Navigate to root scope
- **WHEN** user wants to fix in root scope manually
- **THEN** the code fix SHALL offer:
  1. "Navigate to {RootScopeName}"
  2. On apply, open root scope file at `Configure()` method

### Requirement: ND110 Auto-Fix (View Component)
The system SHALL provide a Code Fix Provider for ND110 diagnostics.

#### Scenario: Suggest Component registration
- **WHEN** ND110 is triggered (View not registered as Component)
- **THEN** the IDE SHALL offer a code fix:
  1. "Add [AutoRegisterIn] to {ViewTypeName}"
  2. On apply, insert attribute on View class:
     ```csharp
     [AutoRegisterIn(typeof({ScopeName}))]
     public class {ViewTypeName} : MonoBehaviour, I{ViewName} { }
     ```

#### Scenario: Suggest Installer registration
- **WHEN** user prefers Installer pattern
- **THEN** the code fix SHALL offer:
  1. "Register Component in Installer"
  2. On apply, insert:
     ```csharp
     builder.RegisterComponentInHierarchy<I{ViewName}>();
     ```
