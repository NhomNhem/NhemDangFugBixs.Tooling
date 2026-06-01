# Migration Guide

## Upgrade to 8.0.0

Version 8.0 makes the DI architecture model shared across generation, analyzers, smoke validation, and reports. The public marker-based workflow stays the same, but generated routes are now deterministic for every mapped scope.

### Required checks

1. Keep `[LifetimeScopeFor<TScope>]` on each composition root that owns a marker scope.
2. Call the generated route from `Configure()`:

   ```csharp
   using NhemDangFugBixs.VContainer;

   [LifetimeScopeFor<IGameplayScope>]
   public sealed class GameplayLifetimeScope : LifetimeScope
   {
       protected override void Configure(IContainerBuilder builder)
       {
           builder.RegisterGeneratedFor<IGameplayScope>();
       }
   }
   ```

3. Remove manual bridge classes that define generated registration routes by hand.
4. Refresh Unity generated `.csproj` files after changing package versions or PackageCache hashes.
5. Run `di-smoke` across the project as the CI gate for project-wide asmdef and mapping checks.

### Behavior changes

- Every mapped scope gets a generated `RegisterGeneratedFor<TScope>()` route.
- Scopes with no discovered services get a no-op generated installer.
- Composition assemblies can register services discovered from referenced feature assemblies.
- Analyzer diagnostics report only facts proven in the current compilation.
- `di-smoke` owns project-wide checks such as missing composition targets and assembly reference gaps.

### Diagnostic ownership

| Code | Owner | Meaning |
| --- | --- | --- |
| `ND005` | Analyzer | Manual registration duplicates an auto/generated registration. |
| `ND006` | Analyzer | Proven cross-scope dependency targets an unreachable scope. |
| `NHEM_DI_011` | Analyzer | Proven composition root is missing its generated registration call. |
| `NDFG014`-class mapping gaps | `di-smoke` / CI | Project-wide service scope has no reachable composition target. |

### Common fixes

- Add `using NhemDangFugBixs.VContainer;` when `RegisterGeneratedFor<TScope>()` does not resolve.
- Ensure the composition asmdef references the service asmdefs it should compose.
- Keep marker interfaces in shared/contracts assemblies instead of composition assemblies.
- Delete stale generated files and rebuild after moving services between asmdefs.
