# Capability: conflict-detection-analyzer (ND005)

## Requirement

- **Goal**: Prevent types from being registered both automatically (via `[AutoRegisterIn]`) and manually (via `builder.Register<T>`).
- **Detection**:
  - Scan all `InvocationExpression` nodes in C# files.
  - Target methods: `Register`, `RegisterEntryPoint`, `RegisterComponent`, `RegisterFactory`, `RegisterComponentOnNewGameObject`, `RegisterComponentInHierarchy`.
  - Check the generic argument `T` of these methods.
  - If `T` is decorated with `[AutoRegisterIn]`, report a diagnostic error.
- **Diagnostics**:
  - **ID**: ND005
  - **Severity**: Error
  - **Message**: "Conflict! Type '{0}' is already marked for auto-registration via [AutoRegisterIn]. Remove the manual registration in '{1}' or remove the attribute from '{0}' to resolve this ambiguity."

## Verification

- **Unit Test**:
  - Given a class `MyService` with `[AutoRegisterIn]`.
  - Given a `LifetimeScope` calling `builder.Register<MyService>()`.
  - Expect a compile-time error `ND005`.
- **Positive Test**:
  - Given a class `ManualService` WITHOUT `[AutoRegisterIn]`.
  - Given a `LifetimeScope` calling `builder.Register<ManualService>()`.
  - Expect no errors.
