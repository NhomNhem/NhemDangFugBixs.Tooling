# Codex Workflow — Release Checklist

Use this before tagging a release.

## Checklist

- [ ] `package.json` version updated.
- [ ] `CHANGELOG.md` updated.
- [ ] `README.md` updated.
- [ ] `Documentation~` updated.
- [ ] `Samples~` validate.
- [ ] Runtime asmdef compiles.
- [ ] Editor asmdef compiles.
- [ ] Generator tests pass.
- [ ] Analyzer tests pass.
- [ ] CLI tests pass.
- [ ] Unity package layout validated.
- [ ] Deploy branch workflow checked.

## Commands

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

## Tag

```bash
git tag vX.Y.Z
git push origin vX.Y.Z
```
