# NhemDangFugBixs.VContainer.SourceGenerator

Roslyn Source Generator, diagnostics, and tooling that wire up [VContainer](https://github.com/hadashiA/VContainer) dependency registration inside Unity projects. The tooling auto-registers types decorated with [AutoRegister] / [AutoRegisterIn], enforces scope safety, and ships analyzers so editors can detect duplicate or invalid registrations early.

## Quick start
1. Build the gesam toolchain:
   `ash
   dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
   `
2. Perform a preflight check that runs the generator and analyzers against a Unity-friendly project:
   `ash
   dotnet di-smoke preflight MyGame.csproj
   `
3. Optionally run the smoke validator against an emitted assembly:
   `ash
   dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll --format json
   `

## Repository layout
- Source~: C# solution containing the CLI, generators, runtime DLLs, analyzers, and shared libraries.
- Runtime/, Editor/, Analyzers/: Unity package folders that get copied into deploy/ before publishing and into the deploy branch for consumers.
- website/ and docs/: documentation site managed with Docusaurus; CI builds the site to keep doc artifacts in sync.
- .github/workflows: GitHub Actions definitions for CI validation, docs deploy, CLI publish, and Unity package deployment.

## Package deployment
- deploy.yml runs on pushes to main/master, builds the .NET projects with .NET 8, strips npm dependencies from package.json, copies runtime/editor/analyzer assets plus .meta files into deploy/, and pushes that directory to the deploy branch using peaceiris/actions-gh-pages@v4.
- Add a concurrency guard keyed by the target branch (e.g., using a deploy-... group) so that overlapping deploy jobs cancel previous runs.
- The Unity-ready deploy branch is the source of truth for installations; consumers should always point Unity Package Manager at ?branch=deploy.

## Installing
1. In Unity Package Manager choose **Add package > Add package from git URL**.
2. Paste https://github.com/NhomNhem/NhemDangFugBixs.Tooling.git?path=/&branch=deploy.
3. Unity imports the runtime DLLs from Runtime/, the [AutoRegister] generators, and the analyzer DLLs in Analyzers/.
4. After import, run dotnet di-smoke preflight against your Unity-targeted project to detect duplicate registrations before entering Play mode.

## Troubleshooting duplicate registrations
**Common causes**
- Old generated .g.cs files linger in Generated/ or Assets/Plugins/Analyzers/.
- A type with the same full name exists in multiple assemblies (duplicated asmdef).
- [AutoRegister]/[AutoRegisterIn] attributes use overlapping scopes or AllowedAssemblies is misconfigured.

**Quick fixes**
1. Upgrade to generator v6.0.1+ for improved dedupe filtering.
2. Delete stale generated files (**/Generated/*.g.cs) from the Unity project and rebuild.
3. Run dotnet di-smoke preflight to catch duplicates before Play Mode.
4. Consolidate duplicate implementations or move shared logic into common asmdef references.

## Building, testing, and docs
- Build everything: dotnet build Source~/NhemDangFugBixs.Tooling.sln --no-restore -c Release
- Run tests: dotnet test Source~/NhemDangFugBixs.Tooling.sln --no-build -c Release
- Docs: cd website && npm ci && npm run build

## Release process
1. Update package.json version, CHANGELOG.md, and any release notes.
2. Tag the commit as X.Y.Z.
3. Pushing the tag triggers publish-nuget.yml to pack and push the CLI, while deploy.yml refreshes the deploy branch with the new artifacts.
4. Optionally, publish GitHub release notes, attach CLI artifacts, and update the website (triggered via deploy-docs.yml).

## Contributing
1. Follow the build/test/docs commands above.
2. Run dotnet format Source~/NhemDangFugBixs.Tooling.sln --verify-no-changes before committing.
3. If you change Unity assets, keep .meta files in sync and include them when copying to deploy/.
4. Document any workflow changes in CONTRIBUTING.md and describe how to trigger or cancel deployments.

## License
Part of the NhemDangFugBixs Tooling collection. See LICENSE for details.
