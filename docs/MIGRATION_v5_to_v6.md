# Migration Guide: v5.x → v6.0.0

## Overview

v6.0.0 is **100% backward compatible** with v5.x. No code changes are required. This guide helps you take advantage of new features.

## What's New in v6.0.0

### Enhanced Diagnostics

All 15 diagnostics now include:
- **Documentation links** - Click to learn more about each error
- **Fix instructions** - Clear remediation steps in error messages
- **Detailed descriptions** - Better IDE tooltip integration

### Expanded Code Fix Providers

v6.0.0 includes **9 Code Fix Providers** (up from 3 in v5.1.0):

| Diagnostic | Fix Action | v5.1.0 | v6.0.0 |
|------------|------------|--------|--------|
| ND001 | Add/remove attribute | ❌ | ✅ |
| ND005 | Remove duplicate registration | ❌ | ✅ |
| ND006 | Add scope bridge or move registration | ❌ | ✅ |
| ND008 | Add MessagePipe broker | ✅ | ✅ |
| ND009 | Add ILogger infrastructure | ✅ | ✅ |
| ND110 | Register Component | ✅ | ✅ |
| ND111 | Enable AsImplementedInterfaces | ❌ | ✅ |
| ND112 | Remove duplicate contract | ❌ | ✅ |
| ND113 | Add view registration | ❌ | ✅ |

### Developer Resources

- **CONTRIBUTING.md** - Complete contribution guide
- **Issue Templates** - Bug report, performance regression, false positive, feature request

## Upgrade Steps

### 1. Update Package Version

**Unity Package Manager (UPM):**
```json
{
  "dependencies": {
    "com.nhemdangfugbixs.tooling": "6.0.0"
  }
}
```

**Or via Git URL:**
```
https://github.com/NhomNhem/NhemDangFugBixs.Tooling.git#v6.0.0
```

### 2. Rebuild Project

```bash
# In Unity: Assets → Reimport All
# Or via CLI:
dotnet build
```

### 3. Verify Analyzers Are Active

Open Unity and check:
- ✅ Errors appear in Error List window
- ✅ Warnings have 💡 lightbulb icon for fixes
- ✅ Hover shows enhanced error messages

### 4. Take Advantage of New Features

#### Use Code Fixes

When you see a diagnostic with a 💡 icon:

**Visual Studio:**
1. Click the lightbulb or press `Ctrl+.`
2. Select the suggested fix
3. Click "Apply"

**Rider:**
1. Press `Alt+Enter`
2. Select the suggested fix
3. Press `Enter`

#### Click Documentation Links

Error messages now include docs links:
```
ND111: Missing contract registration
Type 'MyService' implements interface 'IMyService' but it will not be registered.
Fix: set AsImplementedInterfaces = true or add explicit AsTypes contracts.
Docs: https://docs.nhemdangfugbixs.com/diagnostics/ND111
```

Click the link to learn more about the diagnostic.

## Breaking Changes

**None!** v6.0.0 has zero breaking changes.

All existing code continues to work without modifications.

## Known Issues

### Documentation Links (Temporary)

Documentation links currently point to a site under construction:
```
https://docs.nhemdangfugbixs.com/diagnostics/NDXXX
```

**Workaround:** Check the diagnostic message for fix instructions. Full docs site coming soon.

## Common Migration Scenarios

### Scenario 1: Already Using Code Fixes

**v5.1.0:**
```csharp
// Error: ND008 (MessagePipe broker missing)
// Available Fix: Add broker registration
```

**v6.0.0:**
```csharp
// Error: ND008 (MessagePipe broker missing)
// Enhanced message with docs link
// Same fix available
```

**Action Required:** None. Fixes work the same way.

### Scenario 2: Manually Fixing Diagnostics

**v5.1.0:**
```csharp
// Error: ND111 (Missing contract registration)
// "Type 'MyService' implements interface 'IMyService' but it will not be registered"
// User must manually add AsImplementedInterfaces = true
```

**v6.0.0:**
```csharp
// Error: ND111 (Missing contract registration)
// Enhanced message: "Fix: set AsImplementedInterfaces = true..."
// 💡 Code Fix Available!
```

**Action Required:** None. But now you can use the one-click fix.

### Scenario 3: Double Registration Errors

**v5.1.0:**
```csharp
// Error: ND005 (Registration conflict)
// User must manually find and remove duplicate registration
[AutoRegisterIn(typeof(GameScope))]
public class MyService { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<MyService>(Lifetime.Singleton); // ❌ Remove this manually
    }
}
```

**v6.0.0:**
```csharp
// Error: ND005 (Registration conflict)
// 💡 Code Fix: "Remove manual registration"
[AutoRegisterIn(typeof(GameScope))]
public class MyService { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        // After applying fix, this line is automatically removed
    }
}
```

**Action Required:** None. Use the code fix!

## Performance Notes

v6.0.0 has the same performance characteristics as v5.1.0:
- ✅ No measurable build time increase
- ✅ No analyzer slowdown
- ✅ Same memory usage

Enhanced error messages are pre-computed at compile time and have zero runtime cost.

## Rollback Instructions

If you need to rollback to v5.1.0:

```json
{
  "dependencies": {
    "com.nhemdangfugbixs.tooling": "5.1.0"
  }
}
```

Then rebuild your project. All generated code remains compatible.

## FAQ

### Q: Do I need to update my code?

**A:** No. v6.0.0 is 100% backward compatible.

### Q: Will my existing diagnostics change?

**A:** No. Diagnostic IDs and logic are unchanged. Only messages are enhanced.

### Q: Do code fixes work in Unity Editor?

**A:** Code fixes require an IDE (Visual Studio or Rider). Unity's built-in editor doesn't support Roslyn code fixes.

### Q: What if a documentation link is broken?

**A:** Links point to a site under construction. Fix instructions are still in the error message. Full docs site coming soon.

### Q: Can I disable enhanced messages?

**A:** No, but you can suppress specific diagnostics using `#pragma warning disable NDXXX` if needed.

### Q: Does this work with Unity 2020.x?

**A:** v6.0.0 requires Unity 2021.3+. For older Unity versions, use v5.1.0.

## Getting Help

- **GitHub Issues**: [Report bugs or false positives](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/issues)
- **Discussions**: [Ask questions](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/discussions)
- **CONTRIBUTING.md**: [Contribute to the project](./CONTRIBUTING.md)

## Next Steps

After upgrading:
1. ✅ Try the new code fixes on existing diagnostics
2. ✅ Review enhanced error messages
3. ✅ Report any issues or feedback
4. ✅ Star the repo if you find it useful! ⭐

---

**Welcome to v6.0.0! Enjoy faster, more productive development with enhanced diagnostics and one-click fixes.** 🎉
