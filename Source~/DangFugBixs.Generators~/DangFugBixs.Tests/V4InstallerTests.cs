using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NhemDangFugBixs.Generators;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class V4InstallerTests {
    private static readonly MetadataReference RuntimeAssembly = MetadataReference.CreateFromFile(
        typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly.Location);

    [Test]
    public void Installer_DetectionAndInvocation_GeneratesCorrectCode() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class MyInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) { }
}

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class MyService { }
";

        var result = RunGenerator(source);

        var generatedCode = result.GeneratedTrees[0].ToString();
        
        Assert.That(generatedCode, Does.Contain("new global::MyInstaller().Install(builder);"));
        
        int installerPos = generatedCode.IndexOf("new global::MyInstaller().Install(builder);");
        int servicePos = generatedCode.IndexOf("builder.Register<global::MyService>");
        
        Assert.That(installerPos, Is.LessThan(servicePos), "Installer should be invoked before standard service registration");
    }

    [Test]
    public void Installer_Ordering_GeneratesInCorrectOrder() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
[InstallerOrder(10)]
public class LateInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) { }
}

[AutoRegisterIn(typeof(GameLifetimeScope))]
[InstallerOrder(-5)]
public class EarlyInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) { }
}
";

        var result = RunGenerator(source);

        var generatedCode = result.GeneratedTrees[0].ToString();
        
        int earlyPos = generatedCode.IndexOf("new global::EarlyInstaller().Install(builder);");
        int latePos = generatedCode.IndexOf("new global::LateInstaller().Install(builder);");
        
        Assert.That(earlyPos, Is.Not.EqualTo(-1));
        Assert.That(latePos, Is.Not.EqualTo(-1));
        Assert.That(earlyPos, Is.LessThan(latePos), "EarlyInstaller (Order -5) should be invoked before LateInstaller (Order 10)");
    }

    [Test]
    public void Installer_MixedWithCallbacks_GeneratesInCorrectSequence() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class MyInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) { }
}

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class MyCallback : IBuildCallback {
    public void OnBuild(IObjectResolver container) { }
}

[AutoRegisterIn(typeof(GameLifetimeScope))]
public class MyService { }
";

        var result = RunGenerator(source);

        var generatedCode = result.GeneratedTrees[0].ToString();
        
        int installerPos = generatedCode.IndexOf("new global::MyInstaller().Install(builder);");
        int servicePos = generatedCode.IndexOf("builder.Register<global::MyService>");
        int callbackPos = generatedCode.IndexOf("builder.RegisterBuildCallback");
        
        Assert.That(installerPos, Is.LessThan(servicePos), "Installer should be before service");
        Assert.That(servicePos, Is.LessThan(callbackPos), "Service should be before callback registration");
    }

    [Test]
    public void Generator_NoErrors_WhenCompilationIsValid() {
        var source = @"
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope { }

[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime.Transient)]
public class MyService : IMyService { }

public interface IMyService { }
";

        var result = RunGenerator(source);
        
        var diagnostics = result.Diagnostics.ToList();
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        
        Assert.That(errors, Is.Empty, $"Should have no errors, but got: {string.Join(", ", errors.Select(e => e.Id))}");
    }

    private GeneratorDriverRunResult RunGenerator(string source) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Cast<MetadataReference>()
            .ToList();

        references.Add(RuntimeAssembly);

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new VContainerAutoRegisterGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        var runDriver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        return runDriver.GetRunResult();
    }
}
