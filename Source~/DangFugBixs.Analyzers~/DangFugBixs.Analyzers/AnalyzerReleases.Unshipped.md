## Release 8.0.0

### DI Architecture Ownership

| Rule ID | Owner | Severity | Notes |
|---------|-------|----------|-------|
| ND006 | Analyzer | Warning | Reports cross-scope dependencies only when current-compilation graph evidence proves both services and scopes. |
| ND005 | Analyzer | Error | Reports duplicate manual VContainer registrations for auto-registered services; generated registration routes and generated installer bodies are ignored. |
| NHEM_DI_011 | Analyzer | Warning | Reports missing generated registration calls only when the current compilation proves the composition root and expected marker. |
| NDFG014-class missing mapping | Smoke validation | Error in CI gate | Project-wide missing mapping checks require Unity assembly/reference evidence and are not reported by per-compilation analyzers. |

## Release 5.2.0

### New Rules

| Rule ID | Category | Severity | Notes |
|---------|----------|----------|-------|
| ND111 | Design | Warning | Missing contract registration (interfaces not registered). |
| ND112 | Design | Warning | Duplicate contract registration (same interface registered multiple times). |
| ND113 | Design | Error | Scene view binding mismatch (Presenter injects unregistered View interface). |

## Release 5.1.0

### New Rules

| Rule ID | Category | Severity | Notes |
|---------|----------|----------|-------|
| ND108 | Design | Warning | EntryPoint type must use .AsSelf() or implement an interface. |
| ND110 | Design | Error | View interface injection requires Component registration (MonoBehaviour). |

## Release 1.1.0

### New Rules

| Rule ID | Category | Severity | Notes |
|---------|----------|----------|-------|
| ND006 | Design | Warning | Reports invalid cross-scope dependencies. |
| ND008 | Design | Warning | Reports missing reachable MessagePipe broker registrations. |
