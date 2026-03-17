using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace NhemDangFugBixs.DiSmokeValidation.Tests;

public class ReflectionSmokeValidatorTests {
    [Fact]
    public void Validate_MissingAssembly_ReportsError() {
        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = "missing.dll" });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Assembly not found", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_TwoInjectorTypes_ReportsError() {
        var assemblyPath = BuildAssembly("""
namespace Demo.One {
    public static class VContainerRegistration {
        public static void RegisterGame(object builder) {}
    }
}

namespace Demo.Two {
    public static class VContainerRegistration {
        public static void RegisterGame(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 2;
        public static readonly string[] Scopes = { "GameLifetimeScope", "UiLifetimeScope" };
        public static readonly string[] Entries = {
            "GameLifetimeScope|Demo.One.ServiceA|Singleton|Standard|",
            "UiLifetimeScope|Demo.Two.ServiceB|Singleton|Standard|"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Multiple generated VContainerRegistration types", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_CompleteGraph_Passes() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterGlobal(object builder) {}
        public static void RegisterGame(object builder) {}
        public static void RegisterAll(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 2;
        public static readonly string[] Scopes = { "LifetimeScope", "GameLifetimeScope" };
        public static readonly string[] Entries = {
            "LifetimeScope|Demo.RootLoggerFactory|Singleton|Standard|",
            "GameLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.True(result.IsSuccess, result.ToHumanReadableText());
    }

    [Fact]
    public void Validate_MessagePipeBrokerInSameScope_Passes() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterGame(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 1;
        public static readonly string[] Scopes = { "GameLifetimeScope" };
        public static readonly string[] Entries = {
            "GameLifetimeScope|Demo.PlayerJoined|Singleton|MessageBroker|Demo.PlayerJoined",
            "GameLifetimeScope|Demo.PlayerSubscriber|Scoped|Standard|"
        };
        public static readonly string[] Consumers = {
            "GameLifetimeScope|Demo.PlayerSubscriber|Subscriber|Demo.PlayerJoined"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.True(result.IsSuccess, result.ToHumanReadableText());
    }

    [Fact]
    public void Validate_MessagePipeBrokerInRootScope_Passes() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterProject(object builder) {}
        public static void RegisterGameplay(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 2;
        public static readonly string[] Scopes = { "ProjectLifetimeScope", "GameplayLifetimeScope" };
        public static readonly string[] Entries = {
            "ProjectLifetimeScope|Demo.PlayerJoined|Singleton|MessageBroker|Demo.PlayerJoined",
            "GameplayLifetimeScope|Demo.PlayerSubscriber|Scoped|Standard|"
        };
        public static readonly string[] Consumers = {
            "GameplayLifetimeScope|Demo.PlayerSubscriber|Subscriber|Demo.PlayerJoined"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.True(result.IsSuccess, result.ToHumanReadableText());
    }

    [Fact]
    public void Validate_DuplicateEntries_ReportError() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterGame(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 1;
        public static readonly string[] Scopes = { "GameLifetimeScope" };
        public static readonly string[] Entries = {
            "GameLifetimeScope|Demo.PlayerService|Scoped|Standard|",
            "GameLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Duplicate registration entry found", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_MissingScopeMethod_ReportError() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterGlobal(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 2;
        public static readonly string[] Scopes = { "LifetimeScope", "GameLifetimeScope" };
        public static readonly string[] Entries = {
            "LifetimeScope|Demo.RootLoggerFactory|Singleton|Standard|",
            "GameLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Missing registration method for scope: RegisterGame", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_MissingMessagePipeBroker_ReportError() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterGameplay(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 1;
        public const int ScopeCount = 1;
        public static readonly string[] Scopes = { "GameplayLifetimeScope" };
        public static readonly string[] Entries = {
            "GameplayLifetimeScope|Demo.PlayerSubscriber|Scoped|Standard|"
        };
        public static readonly string[] Consumers = {
            "GameplayLifetimeScope|Demo.PlayerSubscriber|Subscriber|Demo.PlayerJoined"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Type 'PlayerSubscriber' in scope 'GameplayLifetimeScope' depends on MessagePipe Subscriber<PlayerJoined>", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_ILoggerRootSetup_Passes() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterProject(object builder) {}
        public static void RegisterGameplay(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 2;
        public const int ScopeCount = 2;
        public static readonly string[] Scopes = { "ProjectLifetimeScope", "GameplayLifetimeScope" };
        public static readonly string[] Entries = {
            "ProjectLifetimeScope|Demo.LoggingSetup|Singleton|Standard|",
            "GameplayLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
        public static readonly string[] LoggerRoots = {
            "ProjectLifetimeScope|True|True"
        };
        public static readonly string[] LoggerConsumers = {
            "GameplayLifetimeScope|Demo.PlayerService|Demo.PlayerService"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.True(result.IsSuccess, result.ToHumanReadableText());
    }

    [Fact]
    public void Validate_MissingILoggerFactory_ReportError() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterProject(object builder) {}
        public static void RegisterGameplay(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 1;
        public const int ScopeCount = 1;
        public static readonly string[] Scopes = { "GameplayLifetimeScope" };
        public static readonly string[] Entries = {
            "GameplayLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
        public static readonly string[] LoggerRoots = {
            "ProjectLifetimeScope|False|True"
        };
        public static readonly string[] LoggerConsumers = {
            "GameplayLifetimeScope|Demo.PlayerService|Demo.PlayerService"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("depends on ILogger<PlayerService>", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_MissingILoggerAdapter_ReportError() {
        var assemblyPath = BuildAssembly("""
namespace Demo {
    public static class VContainerRegistration {
        public static void RegisterProject(object builder) {}
        public static void RegisterGameplay(object builder) {}
    }
}

namespace NhemDangFugBixs.Generated.Demo {
    public static class RegistrationReport {
        public const int ServiceCount = 1;
        public const int ScopeCount = 1;
        public static readonly string[] Scopes = { "GameplayLifetimeScope" };
        public static readonly string[] Entries = {
            "GameplayLifetimeScope|Demo.PlayerService|Scoped|Standard|"
        };
        public static readonly string[] LoggerRoots = {
            "ProjectLifetimeScope|True|False"
        };
        public static readonly string[] LoggerConsumers = {
            "GameplayLifetimeScope|Demo.PlayerService|Demo.PlayerService"
        };
    }
}
""");

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(new SmokeValidationOptions { AssemblyPath = assemblyPath });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("depends on ILogger<PlayerService>", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_JsonOutput_ContainsMachineReadableStatus() {
        var result = new SmokeValidationResult();
        result.AddError("broken graph");

        var json = result.ToJson();

        Assert.Contains("\"success\": false", json, StringComparison.Ordinal);
        Assert.Contains("broken graph", json, StringComparison.Ordinal);
    }

    private static string BuildAssembly(string source) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var assemblyName = Path.GetRandomFileName();
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            [
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var tempPath = Path.Combine(Path.GetTempPath(), assemblyName + ".dll");
        var emitResult = compilation.Emit(tempPath);
        if (!emitResult.Success) {
            var diagnostics = string.Join(Environment.NewLine, emitResult.Diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException(diagnostics);
        }

        return tempPath;
    }
}
