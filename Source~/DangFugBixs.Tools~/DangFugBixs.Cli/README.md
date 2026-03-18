# DangFugBixs.Cli - DI Validation Tool

A command-line tool for validating VContainer dependency injection setup in Unity projects.

## 🚀 Features

- **Preflight Validation**: Clean + build + validate orchestration
- **Runtime Resolve Smoke**: Headless validation of DI container resolution
- **Multiple Output Formats**: Text and JSON support
- **CI/CD Ready**: Perfect for automated validation pipelines

## 📦 Installation

```bash
dotnet tool install -g DangFugBixs.Cli
```

## 💡 Usage

### Preflight Validation (Recommended)
```bash
dotnet-di-smoke preflight MyUnityProject.csproj
```

### With Options
```bash
# Clean build + validate
dotnet-di-smoke preflight MyUnityProject.csproj --clean

# With runtime resolve smoke test
dotnet-di-smoke preflight MyUnityProject.csproj --resolve-smoke

# JSON output
dotnet-di-smoke preflight MyUnityProject.csproj --format json --output report.json
```

### Direct Assembly Validation
```bash
dotnet-di-smoke validate bin/Debug/net10.0/MyGame.dll
```

## 🎯 Commands

| Command | Description |
|---------|-------------|
| `preflight <project.csproj>` | Full validation (clean + build + validate) |
| `validate <assembly.dll>` | Validate existing assembly |

## ⚙️ Options

| Option | Description | Default |
|--------|-------------|---------|
| `--format <text|json>` | Output format | `text` |
| `--output <file>` | Save report to file | - |
| `--clean` | Clean generated files before build | `false` |
| `--resolve-smoke` | Run runtime resolve smoke test | `false` |

## 📊 What Gets Validated

### Static Analysis
- ✅ VContainerRegistration generation
- ✅ Scope registration methods
- ✅ RegistrationReport metadata consistency
- ✅ MessagePipe broker reachability
- ✅ ILogger root infrastructure

### Runtime Resolve (Phase 1)
- ✅ Container build success
- ✅ EntryPoint resolution (ITickable, IInitializable, etc.)
- ✅ Pure C# type resolution
- ⚠️ MonoBehaviour types (skipped in Phase 1)

## 🎓 Examples

### Basic Validation
```bash
cd MyUnityProject
dotnet-di-smoke preflight MyUnityProject.csproj
```

**Output:**
```
🚀 Preflight Validation
======================

🔨 Building project...
✅ Build succeeded

🔍 Running DI smoke validation...

✅ Preflight validation PASSED
```

### With Resolve Smoke Test
```bash
dotnet-di-smoke preflight MyUnityProject.csproj --resolve-smoke
```

**Output:**
```
🚀 Preflight Validation
======================

🔨 Building project...
✅ Build succeeded

🔍 Running DI smoke validation...

✅ Preflight validation PASSED

🔍 Running runtime resolve smoke test...
Runtime Resolve Smoke Test
==========================

Container Built: ✅ Yes
Total Services: 15
Resolved: 12
Skipped: 3

Services:
  ✅ GameService
  ✅ AudioService
  ⏭️ [Skipped] PlayerView (MonoBehaviour)
```

### CI/CD Integration
```bash
# In GitHub Actions or Azure Pipelines
dotnet tool install -g DangFugBixs.Cli
dotnet-di-smoke preflight MyGame.csproj --format json --output validation-report.json

# Exit code 0 = pass, 1 = fail
```

## 📝 Requirements

- .NET 10.0+
- Unity 2021.3+ (for project validation)
- VContainer 1.0+ (in target project)

## 🔗 Links

- [GitHub Repository](https://github.com/NhomNhem/NhemDangFugBixs.Tooling)
- [NuGet Package](https://www.nuget.org/packages/DangFugBixs.Cli)
- [Documentation](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/blob/master/README.md)

## 📄 License

This project is part of the NhemDangFugBixs Tooling collection.
