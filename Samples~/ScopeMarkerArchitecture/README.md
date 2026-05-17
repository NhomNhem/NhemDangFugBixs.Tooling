# ScopeMarkerArchitecture

Shows how to keep clean dependency direction across asmdefs:

- `Shared` owns marker interfaces (`IProjectScope`, `IGameplayScope`).
- `Application` and `Infrastructure` register against markers only.
- `Composition` maps markers to concrete scopes and invokes generated installers.

This sample is the recommended baseline for production Unity projects.
