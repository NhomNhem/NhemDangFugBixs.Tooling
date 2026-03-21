# Contributing to NhemDangFugBixs.Tooling

Thank you for considering contributing to NhemDangFugBixs.Tooling! This document provides guidelines for contributing to the project.

## 🚀 Getting Started

### Prerequisites

- .NET SDK 10.0 or later
- IDE: Visual Studio 2022, Rider, or VS Code
- Unity 2021.3+ (for testing in Unity projects)
- Git

### Setting Up Development Environment

1. **Clone the repository**
   ```bash
   git clone https://github.com/NhomNhem/NhemDangFugBixs.Tooling.git
   cd NhemDangFugBixs.Tooling
   ```

2. **Open the solution**
   ```bash
   # Open in Rider/Visual Studio
   Source~/NhemDangFugBixs.Tooling.sln
   ```

3. **Build the solution**
   ```bash
   dotnet build Source~/NhemDangFugBixs.Tooling.sln
   ```

4. **Run tests**
   ```bash
   dotnet test Source~/NhemDangFugBixs.Tooling.sln
   ```

### Project Structure

```
NhemDangFugBixs.Tooling/
├── Source~/                                # Hidden from Unity
│   ├── DangFugBixs.Generators~/           # Roslyn Source Generators
│   │   ├── VContainerAutoRegisterGenerator.cs
│   │   ├── Analyzers/ClassAnalyzer.cs
│   │   └── Emitters/RegistrationEmitter.cs
│   ├── DangFugBixs.Analyzers~/            # Roslyn Analyzers & Code Fixes
│   │   ├── Rules/                         # Diagnostic rules (ND001-ND113)
│   │   └── CodeFixes/                     # Code fix providers
│   ├── DangFugBixs.Runtime~/              # Runtime attributes
│   ├── DangFugBixs.Common~/               # Shared models
│   └── DangFugBixs.Tools~/                # CLI tools (di-smoke)
├── Runtime/                                # Compiled runtime DLL
├── Analyzers/                              # Compiled analyzer DLLs
└── Editor/                                 # Unity editor extensions
```

## 🔧 Development Workflow

### Adding a New Diagnostic

1. **Create the diagnostic rule** in `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`
   ```csharp
   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class MyNewRule : DiagnosticAnalyzer {
       public const string NDXXXId = "NDXXX";
       
       public static readonly DiagnosticDescriptor NDXXX = new(
           NDXXXId,
           "Short Title",
           "Problem description. " +
           "Fix: solution steps. " +
           "Docs: https://docs.nhemdangfugbixs.com/diagnostics/NDXXX",
           "Design",
           DiagnosticSeverity.Warning,
           isEnabledByDefault: true,
           description: "Detailed explanation of what this diagnostic detects.");
       
       public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
           => ImmutableArray.Create(NDXXX);
       
       public override void Initialize(AnalysisContext context) {
           context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
           context.EnableConcurrentExecution();
           context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
       }
       
       private static void AnalyzeSymbol(SymbolAnalysisContext context) {
           // Your analysis logic here
       }
   }
   ```

2. **Add tests** in `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/`
   ```csharp
   public class MyNewRuleTests {
       [Fact]
       public async Task ValidCode_NoDiagnostic() {
           var code = @"
               // Valid code that should not trigger diagnostic
           ";
           await VerifyAnalyzerAsync(code);
       }
       
       [Fact]
       public async Task InvalidCode_ReportsDiagnostic() {
           var code = @"
               // Invalid code that should trigger diagnostic
           ";
           await VerifyAnalyzerAsync(code, 
               Verify.Diagnostic(MyNewRule.NDXXXId));
       }
   }
   ```

3. **Create Code Fix Provider** (optional) in `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/CodeFixes/`
   ```csharp
   [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyNewCodeFixProvider)), Shared]
   public sealed class MyNewCodeFixProvider : CodeFixProvider {
       public override ImmutableArray<string> FixableDiagnosticIds 
           => ImmutableArray.Create(MyNewRule.NDXXXId);
       
       public override FixAllProvider GetFixAllProvider() 
           => WellKnownFixAllProviders.BatchFixer;
       
       public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
           // Your code fix logic here
       }
   }
   ```

4. **Update documentation** in README.md and add diagnostic docs

### Modifying the Generator

1. **Modify generator logic** in `Source~/DangFugBixs.Generators~/`
2. **Test in Sandbox** project: `Source~/DangFugBixs.Generators~/DangFugBixs.Sandbox/`
3. **Add tests** in `Source~/DangFugBixs.Generators~/DangFugBixs.Tests/`
4. **Verify incremental generation** still works correctly

### Performance Considerations

- **Use incremental generation APIs** - avoid full recomputation
- **Cache expensive operations** - scope symbol resolution, type checks
- **Profile hot paths** - measure analyzer performance
- **Memory limits** - keep caches under 200MB

## 📋 Pull Request Guidelines

### Before Submitting

- [ ] All tests pass: `dotnet test`
- [ ] Code builds without errors: `dotnet build`
- [ ] No new warnings introduced
- [ ] Code follows existing style conventions
- [ ] XML docs added for public APIs
- [ ] Tests added for new functionality
- [ ] README updated if needed

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Added unit tests
- [ ] Tested in Unity project
- [ ] Verified backward compatibility

## Related Issues
Fixes #(issue number)
```

### Commit Message Format

Use conventional commits:
```
feat: add ND114 diagnostic for missing dependencies
fix: correct scope reachability check for nested scopes
docs: update CONTRIBUTING.md with code fix guidelines
test: add tests for ND113 code fix provider
chore: update project dependencies
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Testing in Unity

1. **Set up local development**
   Create `Source~/LocalBuild.props`:
   ```xml
   <Project>
     <PropertyGroup>
       <NhemUnityProjectRoot>C:\Path\To\Your\Unity\Project</NhemUnityProjectRoot>
     </PropertyGroup>
   </Project>
   ```

2. **Build and deploy**
   ```bash
   dotnet build Source~/NhemDangFugBixs.Tooling.sln
   # DLLs automatically copied to Unity project
   ```

3. **Test in Unity**
   - Open Unity project
   - Verify analyzers appear in Error List
   - Test code fixes work
   - Check generated code

## 📝 Code Style

### C# Conventions

- Use C# 12 features where appropriate
- Prefer `var` when type is obvious
- Use nullable reference types
- Keep methods under 50 lines when possible
- Use `_camelCase` for private fields
- Use `PascalCase` for public members

### Diagnostic Message Format

Always include:
1. **Problem description** - what's wrong?
2. **Fix instructions** - how to resolve it
3. **Documentation link** - where to learn more
4. **Description parameter** - detailed explanation

Example:
```csharp
"Type '{0}' has problem X. " +
"Fix: do Y or Z. " +
"Docs: https://docs.nhemdangfugbixs.com/diagnostics/NDXXX"
```

## 🐛 Reporting Issues

### Bug Reports

Use the bug report template and include:
- **Steps to reproduce**
- **Expected behavior**
- **Actual behavior**
- **Environment** (Unity version, .NET SDK version)
- **Code sample** (minimal reproduction)

### Feature Requests

Explain:
- **Use case** - why is this needed?
- **Proposed solution** - how should it work?
- **Alternatives** - what other approaches were considered?

## 📚 Resources

- [Roslyn Analyzer Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)
- [VContainer Documentation](https://vcontainer.hadashikick.jp/)
- [Project README](./README.md)

## 📧 Contact

- **Issues**: [GitHub Issues](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/issues)
- **Discussions**: [GitHub Discussions](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/discussions)

## 📄 License

By contributing, you agree that your contributions will be licensed under the same license as the project (MIT).

---

Thank you for contributing! 🎉
