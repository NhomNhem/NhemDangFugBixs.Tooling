## 1. Preparation & Backup

- [x] 1.1 Check GameFeelUnity Unity version (must be 2021.3+)
- [x] 1.2 Check GameFeelUnity VContainer version
- [x] 1.3 Backup existing NhemDangFugBixs DLLs if present
- [x] 1.4 Create test folder structure in GameFeelUnity

## 2. Deploy v3.0 DLLs

- [x] 2.1 Copy NhemDangFugBixs.Runtime.dll to GameFeelUnity\Assets\Plugins\
- [x] 2.2 Copy NhemDangFugBixs.Generators.dll to GameFeelUnity\Assets\Plugins\Analyzers\
- [x] 2.3 Copy NhemDangFugBixs.Analyzers.dll to GameFeelUnity\Assets\Plugins\Analyzers\
- [x] 2.4 Verify .meta files exist for all DLLs
- [x] 2.5 Wait for Unity to recompile

## 3. Create Test LifetimeScopes

- [x] 3.1 Create GameLifetimeScope.cs (parent scope)
- [x] 3.2 Create GameplayLifetimeScope.cs (child scope)
- [x] 3.3 Setup parent-child hierarchy in Configure()
- [ ] 3.4 Verify scopes compile without errors

## 4. Create Test Services

- [x] 4.1 Create GameService with `[AutoRegisterIn<GameLifetimeScope>]`
- [x] 4.2 Create AudioService with `[AutoRegisterIn<GameLifetimeScope>]`
- [x] 4.3 Create EnemySpawner with `[AutoRegisterIn<GameplayLifetimeScope>]`
- [x] 4.4 Create PlayerController with `[AutoRegisterIn<GameplayLifetimeScope>]`
- [ ] 4.5 Verify parent-child injection (EnemySpawner injects GameService)

## 5. Validate Generated Code

- [ ] 5.1 Check generated VContainerRegistration.g.cs exists
- [ ] 5.2 Verify RegisterGame() method generated
- [ ] 5.3 Verify RegisterGameplay() method generated
- [ ] 5.4 Verify convention-based naming (not RegisterGameLifetimeScope)
- [ ] 5.5 Verify EntryPoint detection for ITickable/IInitializable

## 6. Runtime Testing

- [ ] 6.1 Create test scene with GameplayLifetimeScope
- [ ] 6.2 Run scene and verify no DI errors
- [ ] 6.3 Verify GameService.Tick() is called
- [ ] 6.4 Verify EnemySpawner.Initialize() is called
- [ ] 6.5 Verify PlayerController.Tick() is called

## 7. Migration Testing

- [x] 7.1 Test `[Obsolete]` warning on legacy `[AutoRegister]`
- [x] 7.2 Document any compilation errors
- [ ] 7.3 Verify migration guide accuracy

## 8. Document Results

- [ ] 8.1 List all issues encountered (if any)
- [ ] 8.2 Capture screenshots of errors (if any)
- [ ] 8.3 Note Unity version and VContainer version tested
- [ ] 8.4 Recommend fixes for any bugs found
- [ ] 8.5 Update CHANGELOG if fixes needed before release

## 9. GameFeelUnity Migration (NEW)

- [x] 9.1 Identify all files using `[AutoRegister]`
- [x] 9.2 Create migration guide (MIGRATION-GAMEFEELUNITY.md)
- [ ] 9.3 Migrate HealthPresenter.cs
- [ ] 9.4 Migrate PlayerAnimationPresenter.cs
- [ ] 9.5 Migrate BulletPresenter.cs
- [ ] 9.6 Migrate PlayerHungerPresenter.cs
- [ ] 9.7 Migrate PlayerPresenter.cs
- [ ] 9.8 Migrate EnemyPresenter.cs
- [ ] 9.9 Migrate EnemyPoolManager.cs
- [ ] 9.10 Migrate MapPoolManager.cs
- [ ] 9.11 Migrate DeterministicWorldGenerator.cs
- [ ] 9.12 Verify build succeeds with 0 errors
