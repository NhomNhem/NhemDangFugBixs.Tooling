# Project Overview

Nhem Dang Fug Bixs Tooling is a high-performance C# Source Generator and Roslyn Analyzer suite designed for Unity projects using VContainer. It automates dependency injection registration through type-safe attributes, reducing boilerplate and preventing runtime errors with compile-time validation.

## Main Technologies
- C# / .NET Standard 2.0
- Roslyn Source Generators
- Roslyn Analyzers
- VContainer

## Architecture
- `Source~`: Hidden source directory (ignored by Unity) containing C# projects
- `Runtime/`: Compiled runtime DLLs
- `Analyzers/`: Compiled analyzer DLLs
- `Editor/`: Unity-specific editor extensions
- `GameFeelUnity-Test-Files/`: Examples and test scenarios
