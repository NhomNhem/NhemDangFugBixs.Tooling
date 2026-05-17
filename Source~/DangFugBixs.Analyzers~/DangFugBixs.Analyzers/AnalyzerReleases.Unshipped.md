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
