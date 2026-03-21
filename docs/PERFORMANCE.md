# Performance Tuning Guide

This guide helps you optimize build times and analyzer performance for large Unity projects using NhemDangFugBixs.Tooling.

## Performance Baseline

Typical performance characteristics:

| Project Size | Services | Build Time | Analyzer Time | Memory |
|--------------|----------|------------|---------------|--------|
| **Small** | 20 | <2s | <1s | <50MB |
| **Medium** | 100 | <5s | <2s | <100MB |
| **Large** | 200+ | <10s | <3s | <150MB |

## Quick Wins

### 1. Use Incremental Compilation

**Unity Project Settings:**
```
Edit → Project Settings → Editor
☑ Enable incremental GC
☑ Asset Pipeline v2 (recommended)
```

**Benefits:**
- 3-5x faster rebuilds
- Only changed files recompile
- Reduced analyzer overhead

### 2. Optimize Assembly Definitions

**Split large assemblies:**
```
Assets/
├── Core.asmdef (20 services)
├── Gameplay.asmdef (50 services)
├── UI.asmdef (30 services)
└── Data.asmdef (40 services)
```

**Benefits:**
- Parallel compilation
- Isolated rebuilds
- Faster incremental builds

**Anti-pattern:**
```
Assets/
└── Assembly-CSharp (200 services) // ❌ Everything in one assembly
```

### 3. Use Script Compilation Defines

**For development builds:**
```csharp
#if !DEBUG
[AutoRegisterIn(typeof(GameScope))]
#endif
public class ExpensiveDebugService { }
```

**Benefits:**
- Fewer services in debug builds
- Faster iteration time
- Conditional registration

## Generator Performance

### Incremental Generation

The generator uses Roslyn's incremental API to avoid unnecessary recomputation:

**What Triggers Regeneration:**
- ✅ Adding/removing `[AutoRegisterIn]` attribute
- ✅ Changing scope parameter
- ✅ Modifying lifetime parameter
- ✅ Adding/removing services

**What DOESN'T Trigger:**
- ❌ Changing method implementations
- ❌ Adding comments
- ❌ Renaming private members
- ❌ Formatting changes

### Monitoring Generator Time

**Enable MSBuild diagnostic logging:**
```bash
dotnet build /v:detailed | Select-String "VContainerAutoRegisterGenerator"
```

**Look for:**
```
VContainerAutoRegisterGenerator executed in 234ms
```

### Reducing Generator Overhead

**Option 1: Fewer Assemblies in AllowedAssemblies List**

Current (aggressive):
```csharp
private static readonly HashSet<string> AllowedAssemblies = new() {
    "Assembly-CSharp",
    "Core", "Services", "Gameplay", "Data", "Runtime",
    "GameFeel_Shared", "GameFeel_Core", "GameFeel_Services",
    // ... 10+ assemblies
};
```

Optimized (selective):
```csharp
private static readonly HashSet<string> AllowedAssemblies = new() {
    "Assembly-CSharp",  // Only main assembly
};
```

**Benefits:**
- 2-3x faster generation
- Less memory usage
- Simpler dependency graph

**Trade-off:** Multi-assembly projects need manual registration in other assemblies.

**Option 2: Disable Unused Features**

If you're not using certain features, comment them out:
```csharp
// Disable scene injection if not needed
// var sceneServices = context.SyntaxProvider
//     .CreateSyntaxProvider(...);
```

## Analyzer Performance

### Hot Paths

The analyzers run on every compilation. Most expensive operations:

1. **Symbol Resolution** (60% of time)
   - `GetScopeSymbol()` - 25%
   - `GetTypeInfo()` - 20%
   - `GetAttributes()` - 15%

2. **Semantic Analysis** (30% of time)
   - Interface checks
   - Scope reachability
   - Cross-layer validation

3. **Diagnostic Reporting** (10% of time)
   - Message formatting
   - Location tracking

### Reducing Analyzer Time

**Option 1: Disable Non-Critical Diagnostics**

Create `.editorconfig`:
```ini
[*.cs]
# Disable warnings in development
dotnet_diagnostic.ND002.severity = none  # Interface recommendation
dotnet_diagnostic.ND003.severity = none  # Constructor check
dotnet_diagnostic.ND108.severity = none  # EntryPoint AsSelf

# Keep errors enabled
dotnet_diagnostic.ND001.severity = error  # Invalid target
dotnet_diagnostic.ND005.severity = error  # Conflict
dotnet_diagnostic.ND113.severity = error  # View binding
```

**Benefits:**
- 20-30% faster analysis
- Less IDE noise during development
- Re-enable for PR builds

**Option 2: Scope Analysis to Specific Folders**

```ini
# Only analyze gameplay code
[Gameplay/**/*.cs]
dotnet_analyzer_diagnostic.category-Usage.severity = warning

# Skip third-party code
[ThirdParty/**/*.cs]
dotnet_analyzer_diagnostic.severity = none
```

**Option 3: Parallel Analysis**

Analyzers already use `context.EnableConcurrentExecution()`, but you can tune thread count:

```bash
# Use fewer threads on slow machines
dotnet build /m:2

# Use all cores on fast machines
dotnet build /m
```

## Unity-Specific Optimizations

### 1. Disable Domain Reload

**Unity 2019.3+:**
```
Edit → Project Settings → Editor
☑ Enter Play Mode Options
  ☑ Disable Domain Reload
  ☐ Disable Scene Reload (optional)
```

**Benefits:**
- 10x faster Enter Play Mode
- Generators don't re-run
- Analyzers skip re-analysis

**Trade-off:** Must handle static state reset manually.

### 2. Use Synchronous Compilation

**During active development:**
```
Edit → Preferences → External Tools
☐ Auto Refresh
```

**Manual refresh when needed:**
```
Assets → Refresh (Ctrl+R)
```

**Benefits:**
- No interruptions during coding
- Batch multiple changes
- Faster workflow

### 3. Optimize Import Settings

**For generated files:**
```
Generated/** → Inspector
☐ Include in build
☐ Validate references
```

**Benefits:**
- Faster import
- Reduced validation overhead

## CI/CD Optimization

### GitHub Actions

**Cache NuGet packages:**
```yaml
- uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

**Parallel test execution:**
```yaml
- name: Test
  run: dotnet test --no-build --parallel
```

### Build Time Monitoring

**Track build times over time:**
```bash
# Add to CI
SECONDS=0
dotnet build
echo "Build took $SECONDS seconds"
```

**Set thresholds:**
```yaml
- name: Check build time
  run: |
    if [ $BUILD_TIME -gt 300 ]; then
      echo "Build took too long!"
      exit 1
    fi
```

## Profiling

### Generator Profiling

**Enable diagnostic output:**
```csharp
// In VContainerAutoRegisterGenerator.cs
private static void Execute(...) {
    var sw = Stopwatch.StartNew();
    
    // ... generation logic
    
    sw.Stop();
    context.ReportDiagnostic(Diagnostic.Create(
        new DiagnosticDescriptor("PERF001", "Performance", 
            $"Generation took {sw.ElapsedMilliseconds}ms", 
            "Performance", DiagnosticSeverity.Info, true),
        Location.None));
}
```

### Analyzer Profiling

**Use Visual Studio Diagnostic Tools:**
```
Analyze → Performance Profiler → CPU Usage
Filter to: NhemDangFugBixs.Analyzers
```

**Look for:**
- Hot paths (>10% time)
- Redundant symbol lookups
- Expensive LINQ queries

## Benchmarking

### Manual Benchmarking

**Create test project:**
```csharp
// Generate N services
for (int i = 0; i < 200; i++) {
    File.WriteAllText($"Service{i}.cs", $$"""
        [AutoRegisterIn(typeof(GameScope))]
        public class Service{{i}} : IService{{i}} {
            public Service{{i}}(IDependency dep) { }
        }
        """);
}
```

**Measure:**
```bash
Measure-Command { dotnet build }
```

### Automated Benchmarking

**BenchmarkDotNet:**
```csharp
[MemoryDiagnoser]
public class GeneratorBenchmarks {
    [Benchmark]
    public void Generate_200_Services() {
        // ... run generator
    }
}
```

## Troubleshooting Slow Builds

### Step 1: Identify Bottleneck

**Check MSBuild log:**
```bash
dotnet build /v:detailed > build.log
```

**Look for:**
```
Task "Csc"
  Generator: VContainerAutoRegisterGenerator: 1234ms  ← Slow
  Analyzer: ConflictCheckRule: 567ms  ← Slow
```

### Step 2: Isolate Problem

**Disable generators:**
```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>false</EmitCompilerGeneratedFiles>
</PropertyGroup>
```

**Disable analyzers:**
```xml
<PropertyGroup>
  <RunAnalyzers>false</RunAnalyzers>
</PropertyGroup>
```

### Step 3: Fix or Workaround

If generator is slow:
- Reduce AllowedAssemblies list
- Split into smaller assemblies
- Disable unused features

If analyzer is slow:
- Disable non-critical diagnostics
- Use `.editorconfig` scoping
- Report performance issue

## FAQ

### Q: Why is first build slow?

**A:** Cold start includes:
- NuGet package restore
- Full semantic analysis
- All generators running
- Populating caches

**Solution:** Subsequent builds are 3-5x faster.

### Q: Why does Unity freeze during compilation?

**A:** Unity's synchronous compilation blocks the UI thread.

**Solution:** Use "Auto Refresh = false" and manual refresh.

### Q: Can I disable analyzers in Unity?

**A:** Yes, but diagnostics won't show in IDE.

**Solution:** Keep enabled, disable specific rules via `.editorconfig`.

### Q: How to optimize for 1000+ services?

**A:** At this scale, consider:
- Multi-assembly architecture
- Selective AllowedAssemblies
- Disable analyzer hot paths
- Use cached builds

---

**Need help optimizing?** [Open a performance issue on GitHub](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/issues)
