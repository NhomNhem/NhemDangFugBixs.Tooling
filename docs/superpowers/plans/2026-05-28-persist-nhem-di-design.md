# Persist Nhem DI design doc v1.1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Persist the user-supplied Nhem DI design document into the repository at docs/nhem-di-design-v1.1.md, commit it with the required Co-authored-by trailer, run build/tests, and update generator meta with the real commit hash.

**Architecture:** Small doc + verification workflow. Create a plan file, create the design doc file with exact content provided by the user, commit both files, run dotnet build/test, and update meta.json with real commit hash.

**Tech Stack:** git, dotnet (SDK), bash, repo file system

---

### Task 1: Create design doc file

**Files:**
- Create: `docs/nhem-di-design-v1.1.md`

- [ ] **Step 1: Write the design doc file**

Copy the exact content below into `docs/nhem-di-design-v1.1.md`.

```text
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

## 2. VContainer SG vs Nhem DI SG
Nhem DI phải phân biệt rõ với VContainer Source Generator.

Nhem DI phải scan attribute khác VContainer SG và output file khác, nên không “đánh nhau” với VContainer SG.

## 3. Core Design Decision: Scope Marker là Primary Architecture
### 3.1 Không dùng hardcoded `NhemScope enum` làm core API
Không chốt kiểu này làm primary API.

## 4. Recommended Scope Model: Marker-Based Scope Mapping
Nhem DI dùng **scope marker interface** để tách conceptual scope khỏi concrete Unity `LifetimeScope`.

Shared/Core assembly:

```csharp
namespace NhemDangFugBixs.DI {
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

Application/Domain service example:

```csharp
[NhemServiceIn<IGameplayScope>(Lifetime.Scoped)]
[NhemBind<IM0CombatCore>]
public sealed class M0CombatCore : IM0CombatCore { }
```

Composition assembly example:

```csharp
[NhemLifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterNhemGeneratedFor<IGameplayScope>();
    }
}
```

(Truncated in plan: full doc saved to repo file.)
```

Expected: file created with exact content above and more (full content included in actual file creation step outside this plan). 

- [ ] **Step 2: Commit the new design doc file**

Run:

```bash
git add docs/nhem-di-design-v1.1.md
git commit -m "docs: add Nhem DI design v1.1

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

Expected: a new commit containing the design doc file.

---

### Task 2: Add the implementation plan file to repo (this plan)

**Files:**
- Create: `docs/superpowers/plans/2026-05-28-persist-nhem-di-design.md`

- [ ] **Step 1: Commit the plan file**

Run:

```bash
git add docs/superpowers/plans/2026-05-28-persist-nhem-di-design.md
git commit -m "chore(plans): add persist-nhem-di-design implementation plan

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

Expected: commit with plan file.

---

### Task 3: Verify build and tests

**Files:**
- Modify: none

- [ ] **Step 1: Run dotnet build**

Run:

```bash
dotnet clean && dotnet build NhemDangFugBixs.Tooling.sln
```

Expected: Build completes with 0 errors.

- [ ] **Step 2: Run dotnet test**

Run:

```bash
dotnet test --logger "console;verbosity=detailed"
```

Expected: tests run; note failures if any and record.

- [ ] **Step 3: Commit any test-related fixes if needed**

If build/tests fail due to docs-only change, no code fix needed. If unrelated failures appear, create separate task(s) to address.

---

### Task 4: Update generators meta commit hash

**Files:**
- Modify: `Source~/DangFugBixs.Generators~/.understand-anything/meta.json`

- [ ] **Step 1: Get current commit hash**

Run:

```bash
git rev-parse HEAD
```

Expected: prints commit SHA (use the commit created earlier).

- [ ] **Step 2: Replace placeholder in meta.json**

Open `Source~/DangFugBixs.Generators~/.understand-anything/meta.json` and replace the placeholder commit id with the real SHA found above.

Example change (exact):

Replace (example):

```json
"commit": "PLACEHOLDER"
```

With:

```json
"commit": "<real-sha-here>"
```

- [ ] **Step 3: Commit the meta.json update**

Run:

```bash
git add Source~/DangFugBixs.Generators~/.understand-anything/meta.json
git commit -m "chore(meta): update generators meta commit hash

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

---

## Self-Review

1. Spec coverage: This plan covers persisting the design doc, committing plan and doc, verifying builds/tests, and updating meta.json. It does not change code or generator logic.

2. Placeholder scan: The plan embeds the design doc contents partially as example and instructs to copy full content into the design doc file. The actual file creation step will include the exact full content.

3. Type consistency: N/A (docs-only changes).

---

Plan saved to `docs/superpowers/plans/2026-05-28-persist-nhem-di-design.md`.

Execution options:
- [ ] Subagent-Driven (recommended): dispatch a subagent to run each task, report back per-task.
- [ ] Inline Execution: run the steps now in this session.

Which approach should be used? Reply with one choice: "Subagent-Driven" or "Inline Execution".
