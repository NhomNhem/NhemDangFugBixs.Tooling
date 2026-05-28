# Nhem DI — Design Docs v1.1
> VContainer convention + source-generator + analyzer + reporting layer
> Target: Unity 2021.3+, IL2CPP-safe, asmdef-friendly, OpenUPM/GitHub public release

## 1. Product Identity
**Nhem DI** là compile-time workflow tooling cho Unity + VContainer. Nhem DI không thay thế VContainer. Nó nằm phía trên VContainer để:

- Giảm boilerplate registration.
- Sinh installer code ổn định, dễ review.
- Bắt lỗi DI sớm bằng analyzer diagnostics.
- Hỗ trợ scope marker architecture qua asmdef boundaries.
- Sinh report cho human + AI coding agents.
- Hỗ trợ CLI/editor preflight cho Unity projects.

Public positioning:

Nhem DI is a compile-time convention and reporting layer for VContainer in Unity. It helps developers and AI coding agents generate safe registrations, detect DI mistakes at compile-time, and produce readable DI reports for architecture review.

---

## 2. VContainer SG vs Nhem DI SG
Nhem DI phải phân biệt rõ với VContainer Source Generator. Nhem DI phải scan attribute khác VContainer SG và output file khác, nên không “đánh nhau” với VContainer SG.

Kết luận: VContainer SG emit injector. Nhem DI SG emit installer/report. Hai source generator chạy song song, không overlap.

---

## 3. Core Design Decision: Scope Marker là Primary Architecture

### 3.1 Không dùng hardcoded `NhemScope enum` làm core API

Không chốt kiểu này làm primary API:

```csharp
public enum NhemScope { Project, Game, Gameplay, UI, Scene, }
```

Lý do:

- Hardcode scope names theo package author.
- Không scale tốt cho nhiều game/project.
- Không hợp public package.
- Dễ ép architecture vào user.

Có thể giữ named-scope mode cho prototype, nhưng không dùng enum fixed làm architecture chính.

---

## 4. Recommended Scope Model: Marker-Based Scope Mapping
Nhem DI dùng **scope marker interface** để tách conceptual scope khỏi concrete Unity `LifetimeScope`.

### Shared/Core assembly

```csharp
namespace NhemDangFugBixs.DI {
    /// <summary>
    /// Marker interface for project-defined DI scopes. 
    /// This interface has no runtime behavior. 
    /// It exists so analyzers and generators can validate scope usage. 
    /// </summary>
    public interface INhemScopeMarker { }
}
```

Project-defined markers:

```csharp
using NhemDangFugBixs.DI;
namespace GlassRefrain.Shared.DI.Scopes {
    public interface IGameScope : INhemScopeMarker { }
    public interface IGameplayScope : INhemScopeMarker { }
    public interface IUIScope : INhemScopeMarker { }
}
```

### Application/Domain service

```csharp
[NhemServiceIn<IGameplayScope>(Lifetime.Scoped)]
[NhemBind<IM0CombatCore>]
public sealed class M0CombatCore : IM0CombatCore { }
```

### Composition assembly

```csharp
[NhemLifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterNhemGeneratedFor<IGameplayScope>();
    }
}
```

This keeps dependency direction clean:

Shared <- Application Shared <- Infrastructure Shared <- Composition Application <- Composition Infrastructure <- Composition

Avoid:

Application -> Composition

---

## 5. Why `IGameplayScope : INhemScopeMarker`?
`INhemScopeMarker` không có runtime behavior. Nó là compile-time architecture label. Nó dùng để:

1. Ràng buộc generic API: where TScope : INhemScopeMarker
2. Ngăn user dùng nhầm service interface làm scope: NhemServiceIn<IBackendClient> => invalid.
3. Giúp analyzer phân biệt scope marker với interface bình thường.
4. Giúp report/CLI group services theo scope.
5. Giữ architecture direction sạch qua asmdef boundaries.

Example constraint:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class NhemServiceInAttribute<TScope> : Attribute where TScope : INhemScopeMarker {
    public Lifetime Lifetime { get; }
    public NhemServiceInAttribute(Lifetime lifetime) { Lifetime = lifetime; }
}
```

Invalid usage:

```csharp
[NhemServiceIn<IBackendClient>(Lifetime.Singleton)]
public sealed class Foo { }
```

Expected diagnostic:

```text
NDI012 InvalidScopeMarker IBackendClient is used as a scope marker but does not implement INhemScopeMarker.
```

---

## 6. Scope Marker Generation Decision

### 6.1 Do not let SG silently write real files
Roslyn Source Generator must not silently write real `.cs` files into `Assets/` or the repository. Reason:

- SG runs during IDE/compiler/Unity import.
- Writing real files can cause import loops.
- It dirties git unexpectedly.
- It creates unstable IDE/CI behavior.
- SG should stay pure: source input -> generated compilation output.

### 6.2 Do not generate public marker types from concrete LifetimeScope as recommended path
Avoid recommended flow where SG generates IGameplayScope from GameplayLifetimeScope because that creates dependency inversion problems.

### 6.3 Correct solution: Scope Scaffold
Nhem DI should provide a scaffold tool that creates real source files. CLI:

```bash
nhem-di scope create Gameplay \
 --namespace GlassRefrain.Shared.DI.Scopes \
 --out Assets/_Project/Code/Shared/DI/Scopes
```

Generated real file:

```csharp
namespace GlassRefrain.Shared.DI.Scopes {
    public interface IGameplayScope : INhemScopeMarker { }
}
```

Optional generated LifetimeScope file:

```csharp
[NhemLifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterNhemGeneratedFor<IGameplayScope>();
    }
}
```

Unity Editor equivalent:

Tools > Nhem DI > Create Scope Marker
Tools > Nhem DI > Generate Missing Scope Markers

Analyzer/code fix equivalent:

NDI020 MissingScopeMarkerFile Quick Fix: Create IGameplayScope marker file

---

## 7. Supported Scope Modes
Nhem DI supports three scope modes.

### Mode 1 — Marker Mode, recommended
For modular projects/asmdef-heavy projects.

```csharp
[NhemServiceIn<IGameplayScope>(Lifetime.Scoped)]
[NhemBind<IM0CombatCore>]
public sealed class M0CombatCore : IM0CombatCore { }
```

```csharp
[NhemLifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) { builder.RegisterNhemGeneratedFor<IGameplayScope>(); }
}
```

### Mode 2 — Named Scope Mode, beginner/prototype
For simple projects.

```csharp
[NhemScope("Game")] public sealed class GameLifetimeScope : LifetimeScope { }
```

```csharp
[NhemService("Game", Lifetime.Singleton)] [NhemBind<IBackendClient>] public sealed class TinyRiftBackendClient : IBackendClient { }
```

Diagnostic required:

```text
NDI010 UnknownNamedScope Service references scope "Gamepaly", but no [NhemScope("Gamepaly")] exists.
```

### Mode 3 — Generated marker prototype mode, optional/future
Allowed only for simple same-assembly projects. SG may generate marker interfaces from declarations.

Rule: Allowed for prototypes. Not recommended for layered asmdef architecture.

---

## 8. Attribute API

### Phase 1 Core

```csharp
[NhemServiceIn<IGameplayScope>(Lifetime.Scoped)]
[NhemBind<IM0CombatCore>]
public sealed class M0CombatCore : IM0CombatCore { }
```

```csharp
[NhemEntryPointIn<IGameplayScope>] public sealed class GameplayTickHandler : ITickable, IDisposable { }
```

```csharp
[NhemIgnore] public sealed class DebugOnlyService { }
```

### Phase 2 Unity Components

```csharp
[NhemComponentIn<IGameplayScope>(ComponentSource.InHierarchy)] public sealed class M0DebugOverlayAdapter : MonoBehaviour { }
```

```csharp
[NhemComponentIn<IGameScope>( ComponentSource.NewGameObject, Name = "AudioManager", Lifetime = Lifetime.Singleton)] public sealed class AudioManager : MonoBehaviour { }
```

### Manual factory

```csharp
[NhemManualFactory( Reason = "Requires authored memory id. Do not resolve string from container.")]
public sealed class M0MemoryState : IM0MemoryState { public M0MemoryState(string memoryId) { } }
```

---

## 9. ComponentSource

```csharp
public enum ComponentSource { InHierarchy, NewGameObject, Instance }
```

Phase 2 support table summarized in doc.

---

## 10. VContainer Registration Surface

Nhem DI targets VContainer APIs: builder.Register<T>(Lifetime), RegisterEntryPoint, RegisterComponentInHierarchy, RegisterComponentOnNewGameObject, RegisterInstance, manual Register lambda.

---

## 11. `WithParameter` Decision
Nhem DI does **not** auto-emit `WithParameter`. Detect and report primitive/config-like ctor params and ask for ManualFactory.

---

## 12. Diagnostics — NDI Series
Core diagnostics enumerated (NDI002..NDI020) with descriptions and severities. NDI003 example message included.

---

## 13. Generated Output
Installer, extension method, C# report, Markdown report examples included.

---

## 14. Case Study: Tiny Rift
Example architecture and expected report.

---

## 15. Case Study: Glass Refrain
Detailed example and validations.

---

## 16. Roadmap
Phase 1: Core Generator + Marker Architecture
Phase 2: Unity Component Safety
Phase 3: Public/OpenUPM Differentiator

---

## 17. Non-Goals
List of non-goals.

---

## 18. Implementation Priorities
Ordered list of priorities (lock runtime API, implement INhemScopeMarker, etc.)

---

## 19. Final Design Decision Chốt lại
Primary architecture: Scope Marker Mode. Recommended: create real marker files in Shared/Core; composition maps markers to LifetimeScope; SG generates installers/reports only; analyzer validates mapping; CLI/editor scaffold creates marker files when needed.

Final one-liner:

Nhem DI should not own your architecture. It should make your architecture compile-time visible, generated, validated, and reportable.
