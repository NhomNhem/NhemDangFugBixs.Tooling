# GitHub Actions Setup Guide

## 🔑 Required Secrets

### 1. NuGet API Key (Required for NuGet Publish)

1. Go to https://nuget.org/account/apikeys
2. Click **Create** 
3. Name: `GitHub Actions`
4. Select scopes: **Push**
5. Copy the API key
6. Go to your GitHub repository → **Settings** → **Secrets and variables** → **Actions**
7. Click **New repository secret**
8. Add:
   - **Name:** `NUGET_API_KEY`
   - **Value:** (paste your API key)

### 2. GitHub Token (Auto-generated)

The `GITHUB_TOKEN` secret is automatically available in GitHub Actions. No setup needed.

---

## 🚀 How to Release

### Method 1: Push Tag (Recommended)

```bash
# 1. Update version in code
# 2. Commit changes
git commit -m "release: v5.2.0"

# 3. Create and push tag
git tag v5.2.0
git push origin v5.2.0
```

This triggers:
- ✅ `publish-nuget.yml` → Publishes to NuGet
- ✅ `create-release.yml` → Creates GitHub Release

### Method 2: Manual Dispatch

1. Go to **Actions** tab in GitHub
2. Select workflow: **Publish CLI to NuGet** or **Create GitHub Release**
3. Click **Run workflow**
4. Enter version (e.g., `5.2.0` or `v5.2.0`)
5. Click **Run workflow**

---

## 📦 Package Output

After successful release:

| Artifact | Location |
|----------|----------|
| **NuGet Package** | https://www.nuget.org/packages/DangFugBixs.Cli/5.2.0 |
| **GitHub Release** | https://github.com/NhomNhem/NhemDangFugBixs.Tooling/releases/tag/v5.2.0 |
| **Build Artifacts** | GitHub Actions → Latest run → Artifacts |

---

## 🔍 Workflow Status

| Workflow | Trigger | Output |
|----------|---------|--------|
| **CI Validation** | Every push/PR | Build ✅, Tests ✅, CLI ✅ |
| **Publish NuGet** | Tag push (v*) | NuGet package published |
| **Create Release** | Tag push (v*) | GitHub Release with notes |

---

## 🐛 Troubleshooting

### Package Already Exists
```bash
# Delete old version from NuGet first
dotnet nuget delete DangFugBixs.Cli 5.2.0 -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json

# Then push again
```

### Workflow Fails
1. Go to **Actions** tab
2. Select failed workflow
3. Check logs for error details
4. Fix issue and re-run

### Missing NuGet API Key
```
Error: NU1301: The local source doesn't have a valid API key
```
→ Add `NUGET_API_KEY` secret in repository settings

---

## 📝 Version Naming

Follow semantic versioning:

| Version | When to Use |
|---------|-------------|
| `5.2.0` | New features (minor) |
| `5.2.1` | Bug fixes (patch) |
| `6.0.0` | Breaking changes (major) |

---

## 🎯 Example Release Flow

```bash
# 1. Make changes
git add -A
git commit -m "feat: Add multi-contract support"

# 2. Update version (if needed)
# Edit CHANGELOG.md, package.json

# 3. Create release commit
git commit -m "release: v5.2.0"

# 4. Tag and push
git tag v5.2.0
git push origin master --tags

# 5. Watch GitHub Actions
# https://github.com/NhomNhem/NhemDangFugBixs.Tooling/actions
```

---

## ✅ Checklist Before Release

- [ ] All tests passing locally
- [ ] CHANGELOG.md updated
- [ ] Version numbers updated (package.json, .csproj)
- [ ] README.md reflects new features
- [ ] Build succeeds: `dotnet build`
- [ ] Tests pass: `dotnet test`
