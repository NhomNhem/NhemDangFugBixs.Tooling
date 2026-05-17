# Migration Plan: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This migration plan outlines the migration path for users from version 7.x through 8.0.0, focusing on the breaking changes in 8.0.0 where legacy exposure flags are removed.

## Migration Timeline

### Phase 1: Preparation (7.2.2 - 7.8.0)
- Introduce canonical explicit style
- Add diagnostic warnings for mixed style
- Provide migration assistant
- Gather user feedback

### Phase 2: Obsolescence Warning (8.0.0)
- Mark legacy flags as obsolete
- Strengthen diagnostic to error
- Provide comprehensive migration guide
- Migration assistant with auto-fix

### Phase 3: Removal (9.0.0 - Future)
- Remove legacy flags entirely
- Remove obsolescence warnings
- Canonical API only

## Migration Paths

### Path A: Early Adopter (Recommended)

**Who**: New projects or projects early in development

**When**: Anytime during 7.2.2 - 7.8.0

**Steps**:
1. Use canonical explicit style from the start
2. Follow sample patterns
3. No migration needed for 8.0.0

**Benefits**:
- No breaking changes
- Clean API from the start
- Best practices established early

---

### Path B: Gradual Migration

**Who**: Existing projects with some legacy flag usage

**When**: During 7.2.2 - 7.8.0

**Steps**:
1. Review NHEM_DI_060 warnings
2. Identify services using legacy flags
3. Convert to canonical explicit style incrementally
4. Verify with migration assistant
5. Complete before 8.0.0

**Benefits**:
- Spread migration effort over time
- Test changes incrementally
- Less risky than big bang migration

---

### Path C: Late Migration

**Who**: Existing projects with extensive legacy flag usage

**When**: During 8.0.0 release

**Steps**:
1. Upgrade to 8.0.0
2. Address obsolescence warnings
3. Use migration assistant with auto-fix
4. Review and verify changes
5. Test thoroughly

**Benefits**:
- One-time migration effort
- Automated assistance available
- Clear deadline provides motivation

---

## Specific Migrations

### Migration 1: Legacy Flags to Explicit Attributes

#### Before (7.x)
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true, AsSelf = true)]
public sealed class CombatService : ICombatService, ITickable
{
}
```

#### After (8.0.0)
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<ICombatService>]
[As<ITickable>]
[AsSelf]
public sealed class CombatService : ICombatService, ITickable
{
}
```

#### Migration Steps
1. Remove `AsImplementedInterfaces` flag
2. Remove `AsSelf` flag
3. Add `[As]` attribute for each interface
4. Add `[AsSelf]` attribute if self-registration needed
5. Verify generated output is identical

#### Automation
- Migration assistant detects pattern
- Migration assistant suggests conversion
- Code fix provider applies conversion
- Review suggested changes before applying

---

### Migration 2: Manual Registration to Attributes

#### Before (Manual VContainer)
```csharp
builder.Register<MemoryStateService>(Lifetime.Scoped)
    .As<IMemoryStateService>();
```

#### After (Attribute-Driven)
```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IMemoryStateService>]
public sealed class MemoryStateService : IMemoryStateService
{
}
```

#### Migration Steps
1. Identify manual registrations
2. Determine scope and lifetime
3. Add `[AutoRegisterIn]` attribute
4. Add `[As]` attributes
5. Remove manual registration from Configure()

#### Automation
- Migration assistant detects manual registration patterns
- Migration assistant suggests attribute conversion
- Manual review required (not auto-fix)
- Consider context (config, runtime data, scene objects)

---

### Migration 3: Mixed Style to Canonical Style

#### Before (Mixed Style - Triggers NHEM_DI_060)
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = false, AsSelf = false)]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

#### After (Canonical Style)
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

#### Migration Steps
1. Review NHEM_DI_060 warning
2. Remove legacy flags from `[AutoRegisterIn]`
3. Keep explicit `[As]` attributes
4. Verify generated output

#### Automation
- NHEM_DI_060 diagnostic identifies mixed style
- Code fix provider removes legacy flags
- Review suggested changes before applying

---

## Migration Tools

### NHEM_DI_060 Diagnostic

**Purpose**: Warn about mixed exposure style

**Severity**: Warning in 7.x, Error in 8.0.0

**Usage**:
- Review warnings in Unity Console
- Click warning to navigate to code
- Apply code fix or manually fix

**Migration Support**:
- Identifies services needing migration
- Provides code fix for common cases
- Explains canonical style

---

### Migration Assistant

**Purpose**: Detect manual registration patterns and suggest attribute-driven conversion

**Access**:
- Unity Editor: Tools/Nhem/Migration Assistant
- CLI: `nhem migrate-report --project <UnityProjectRoot>`

**Features**:
- Detects manual registration patterns
- Suggests attribute conversion
- Export report as JSON
- Auto-fix for simple cases (8.0.0)

**Migration Support**:
- Identifies manual registrations
- Provides conversion suggestions
- Automates conversion for common patterns
- Does not modify risky registrations

---

### Code Fix Providers

**Purpose**: Automatically fix code issues

**Available Fixes**:
- Remove legacy flags (mixed style)
- Convert legacy flags to explicit attributes
- Convert manual registrations (simple cases)

**Usage**:
- Light bulb in IDE
- Apply fix with one click
- Review before applying

**Migration Support**:
- Automates common conversions
- Reduces manual effort
- Safe by default (review required)

---

## Migration Validation

### Pre-Migration Checks

1. **Backup Project**
   - Create git branch
   - Commit current state
   - Test rollback procedure

2. **Run di-smoke**
   - Validate project structure
   - Identify potential issues
   - Review warnings

3. **Run Migration Assistant**
   - Generate migration report
   - Review suggested changes
   - Estimate effort

4. **Review NHEM_DI_060 Warnings**
   - Identify services using mixed style
   - Prioritize critical services
   - Plan migration order

### Post-Migration Validation

1. **Generator Tests**
   - Verify generated code is correct
   - Check for duplicate registrations
   - Verify contract ordering

2. **Analyzer Tests**
   - Verify no new diagnostics
   - Verify NHEM_DI_060 resolved
   - Check for other warnings

3. **Unity Compile**
   - Verify project compiles
   - Check for Unity errors
   - Verify no missing references

4. **Runtime Tests**
   - Verify DI container builds correctly
   - Verify services resolve correctly
   - Verify lifecycle methods called

5. **Integration Tests**
   - Verify game functionality
   - Verify scene loading
   - Verify DI integration

---

## Migration Scenarios

### Scenario 1: Small Project (< 50 services)

**Approach**: Big bang migration

**Steps**:
1. Upgrade to 8.0.0
2. Run migration assistant with auto-fix
3. Review and apply changes
4. Test thoroughly
5. Commit

**Time Estimate**: 1-2 hours

---

### Scenario 2: Medium Project (50-200 services)

**Approach**: Incremental migration

**Steps**:
1. Start during 7.x
2. Migrate by scope or feature
3. Test each increment
4. Complete before 8.0.0
5. Upgrade to 8.0.0
6. Final validation

**Time Estimate**: 4-8 hours

---

### Scenario 3: Large Project (> 200 services)

**Approach**: Gradual migration with team coordination

**Steps**:
1. Start during 7.x
2. Create migration plan
3. Assign migration by team/feature
4. Use feature branches
5. Merge incrementally
6. Continuous integration testing
7. Complete before 8.0.0
8. Upgrade to 8.0.0
9. Final validation

**Time Estimate**: 16-40 hours (team effort)

---

## Common Migration Issues

### Issue 1: Configuration Services

**Problem**: Configuration services using RegisterInstance cannot be converted to attributes

**Solution**: Keep manual registration for configuration services

**Example**:
```csharp
// Keep this manual
builder.RegisterInstance(gameConfig);
```

---

### Issue 2: Runtime Data Services

**Problem**: Runtime data services using RegisterInstance cannot be converted to attributes

**Solution**: Keep manual registration for runtime data

**Example**:
```csharp
// Keep this manual
builder.RegisterInstance(playerState);
```

---

### Issue 3: Scene Object Services

**Problem**: Scene objects using RegisterComponentInHierarchy cannot always be converted

**Solution**: Use `[RegisterComponentInHierarchy]` attribute, but verify scene setup

**Example**:
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn<IGameplayScope>]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

---

### Issue 4: Complex Lifetime Patterns

**Problem**: Complex lifetime patterns (conditional, scoped to parent) cannot be expressed with attributes

**Solution**: Keep manual registration for complex patterns

**Example**:
```csharp
// Keep this manual if complex
builder.Register<Service>(parentLifetime);
```

---

### Issue 5: Entry Points in Service Assemblies

**Problem**: Entry points in service-only assemblies may require VContainer.Unity reference

**Solution**: Move entry points to composition assembly or add VContainer.Unity reference

**Example**:
```csharp
// Move to composition assembly
[AutoRegisterIn<IGameplayScope>]
[EntryPoint]
public sealed class GameplayLoopEntryPoint : IStartable
{
}
```

---

## Rollback Plan

### If Migration Fails

1. **Revert Git Branch**
   - Rollback to pre-migration commit
   - Verify project state
   - Identify failure cause

2. **Partial Rollback**
   - Rollback specific services
   - Keep successfully migrated services
   - Retry failed migrations

3. **Alternative Approach**
   - Try different migration strategy
   - Use manual migration instead of automated
   - Seek community support

### Rollback Triggers

- Runtime errors after migration
- Generated code is incorrect
- Services fail to resolve
- Integration tests fail
- Performance degradation

---

## Communication Plan

### Pre-Migration Communication

**Timeline**: 6 months before 8.0.0

**Channels**:
- GitHub release notes
- README updates
- Blog post
- Community forums
- Discord/Slack

**Content**:
- Announce 8.0.0 breaking changes
- Explain benefits of canonical style
- Provide migration timeline
- Share migration guide
- Solicit feedback

---

### During Migration Communication

**Timeline**: 3 months before 8.0.0

**Channels**:
- GitHub issues
- Community forums
- Discord/Slack

**Content**:
- Answer migration questions
- Share migration experiences
- Document common issues
- Provide support

---

### Post-Migration Communication

**Timeline**: At 8.0.0 release

**Channels**:
- GitHub release notes
- Blog post
- Community forums

**Content**:
- Announce 8.0.0 release
- Highlight migration tools
- Share success stories
- Provide support resources

---

## Support Resources

### Documentation

- Migration guide (docs/migration-7-to-8.md)
- README.md updates
- Sample suite (canonical style)
- API documentation

### Tools

- Migration assistant (Unity Editor)
- Migration assistant (CLI)
- Code fix providers
- NHEM_DI_060 diagnostic
- di-smoke validation

### Community

- GitHub issues
- GitHub discussions
- Community forums
- Discord/Slack

---

## Success Criteria

### Migration Success Metrics

- **Adoption Rate**: % of projects using canonical style before 8.0.0
- **Migration Completion**: % of projects successfully migrated by 8.0.0
- **User Satisfaction**: Positive feedback on migration process
- **Bug Reports**: Low number of migration-related bug reports
- **Support Requests**: Manageable number of migration support requests

### Target Metrics

- **Adoption Rate**: 50% by 7.8.0
- **Migration Completion**: 90% by 8.0.0 + 3 months
- **User Satisfaction**: 80% positive feedback
- **Bug Reports**: < 5 migration-related bugs in first month
- **Support Requests**: < 20 migration support requests in first month

---

## Conclusion

This migration plan provides a comprehensive path for users to migrate from 7.x to 8.0.0. By offering multiple migration paths, providing migration tools, and communicating clearly, we can minimize disruption and ensure a smooth transition to the canonical API.

The key to successful migration is early adoption, gradual migration where possible, and comprehensive testing. The migration assistant and code fix providers will significantly reduce the manual effort required for migration.
