# NhemDangFugBixs.VContainer.SourceGenerator

Roslyn source generators, analyzers, and Unity package assets that wire up [VContainer](https://github.com/hadashiA/VContainer) dependency registration with safer defaults. The package auto-registers types decorated with `[AutoRegister]` and `[AutoRegisterIn]`, validates scope usage, and ships analyzers to catch duplicate or invalid registrations early.

## Installing

### OpenUPM

Preferred for production projects once the package is published to the registry:

```bash
openupm add com.nhemdangfugbixs.tooling
```

### Git URL fallback

If you need the Git-based package before or alongside OpenUPM, add the Unity-ready branch:

```text
https://github.com/NhomNhem/NhemDangFugBixs.Tooling.git?path=/&branch=deploy
```

The `deploy` branch stays minimal for Unity Package Manager imports. The source branch keeps the package self-contained so release tags are also suitable for registry publishing workflows such as OpenUPM.

## Quick Start
1. Build the toolchain:
   ```bash
   dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
   ```
2. Run the preflight validator against a Unity-friendly project:
   ```bash
   dotnet di-smoke preflight MyGame.csproj
   ```
3. Optionally validate an emitted assembly:
   ```bash
   dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll --format json
   ```

## Repository Layout
- `Source~/`: C# solution containing the CLI, generators, analyzers, and supporting libraries.
- `Runtime/`, `Editor/`, `Analyzers/`: Unity package content that must remain release-ready in source tags.
- `website/` and `docs/`: documentation sources and Docusaurus site, excluded from the `deploy` branch package output.
- `.github/workflows/`: validation, release, docs, and deploy automation.

## Package Deployment
- `deploy.yml` builds the .NET projects, filters `package.json` down to UPM-safe fields, copies Unity assets into `deploy/`, and publishes that directory to the `deploy` branch.
- Source tags must still contain a valid package manifest plus Unity assets so OpenUPM or any tag-based packaging flow can consume the repository without relying on the `deploy` workflow output.
- The `deploy` branch remains the smallest Unity import surface, while the source branch remains release-ready for registry automation.

## Building, Testing, and Docs
- Build everything: `dotnet build Source~/NhemDangFugBixs.Tooling.sln --no-restore -c Release`
- Run tests: `dotnet test Source~/NhemDangFugBixs.Tooling.sln --no-build -c Release`
- Build docs: `cd website && npm ci && npm run build`

## Release Process
1. Update `package.json`, `CHANGELOG.md`, and release notes together.
2. Run CI and release-readiness validation.
3. Create a tag that exactly matches `package.json.version`, for example `v6.0.4`.
4. Push the tag to trigger release packaging and docs deployment.

## Troubleshooting Duplicate Registrations
Common causes:
- Old generated `.g.cs` files remain in `Generated/` or `Assets/Plugins/Analyzers/`.
- A type with the same full name exists in multiple assemblies.
- `[AutoRegister]` or `[AutoRegisterIn]` annotations overlap scopes, or `AllowedAssemblies` is misconfigured.

Quick fixes:
1. Upgrade to generator `v6.0.1` or newer for improved dedupe filtering.
2. Delete stale generated files under `**/Generated/*.g.cs` in the Unity project and rebuild.
3. Run `dotnet di-smoke preflight` before Play Mode.
4. Consolidate duplicate implementations or move shared logic into common asmdef references.

## License
Released under the ISC license. See [LICENSE](LICENSE).
