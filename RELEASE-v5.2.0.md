# v5.2.0 Release Checklist

## Pre-Release

- [ ] Delete test packages from NuGet
  - [x] Delete DangFugBixs.Cli.5.2.0-test (manual via NuGet.org)
  - [ ] Verify deletion at https://www.nuget.org/packages/DangFugBixs.Cli

- [ ] Clean up test tags
  - [x] Delete local tag: `git tag -d v5.2.0-test`
  - [x] Delete remote tag: `git push origin --delete v5.2.0-test`

## Release Preparation

- [ ] Update version numbers
  - [ ] `package.json` → `5.2.0`
  - [ ] `DangFugBixs.Cli.csproj` → `5.2.0`
  - [ ] `CHANGELOG.md` → Add v5.2.0 section

- [ ] Update CHANGELOG.md with v5.2.0 features
  - [ ] Multi-Contract Registration Support
  - [ ] Detailed Diagnostics (ND111, ND112)
  - [ ] Scene View Binding Detection (ND113)
  - [ ] Enhanced Reports (scope-based, diff mode)
  - [ ] CI/CD Integration templates

- [ ] Final testing
  - [ ] Run all tests: `dotnet test`
  - [ ] Test CLI locally: `dotnet-di-smoke preflight`
  - [ ] Test in GameFeelUnity project

## Release Day

- [ ] Create release commit
  ```bash
  git commit -m "release: v5.2.0 - Multi-Contract Support & Enhanced Diagnostics"
  ```

- [ ] Tag release
  ```bash
  git tag v5.2.0
  git push origin v5.2.0
  ```

- [ ] GitHub Actions will automatically:
  - [ ] Build & Test
  - [ ] Publish to NuGet
  - [ ] Create GitHub Release

## Post-Release

- [ ] Verify NuGet package
  - [ ] Check at https://www.nuget.org/packages/DangFugBixs.Cli/5.2.0
  - [ ] Install test: `dotnet tool install -g DangFugBixs.Cli`

- [ ] Verify GitHub Release
  - [ ] Check at https://github.com/NhomNhem/NhemDangFugBixs.Tooling/releases/tag/v5.2.0
  - [ ] Release notes complete
  - [ ] Assets uploaded

- [ ] Announce release
  - [ ] Update README with v5.2.0 features
  - [ ] Share with team
  - [ ] Update documentation

## v5.2.0 Features Summary

### Multi-Contract Registration
- `[AutoRegisterIn(typeof(Scope), As = [typeof(IService1), typeof(IService2)])]`
- Auto-detect all interfaces with `AsImplementedInterfaces = true`
- Explicit contract binding with `AsTypes`

### Enhanced Diagnostics
- **ND111**: Missing contract registration
- **ND112**: Duplicate contract registration
- **ND113**: Scene view binding mismatch

### CLI Enhancements
- `--resolve-smoke` flag for runtime validation
- `--diff` mode for comparing reports
- Scope-based report grouping

### CI/CD Integration
- GitHub Actions templates
- Azure DevOps pipelines
- Automated NuGet publish on tag

---

**Release Target Date:** 2026-03-25
**Release Manager:** @NhomNhem
