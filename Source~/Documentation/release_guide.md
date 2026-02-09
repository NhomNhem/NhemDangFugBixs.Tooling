# Release Guide & Version Management with Git Tags

To share this package professionally, you should use the Git Tag system. The Unity Package Manager (UPM) fully supports importing packages via Git URLs with specific version tags.

## 1. Prepare for Release
Before creating a Tag, ensure that your `package.json` file in the root folder is updated with the desired version.

```json
"version": "1.0.0"
```

## 2. Git Commands for Release
Open your terminal at the project root and execute the following commands:

### Step 1: Commit all changes
```bash
git add .
git commit -m "release: prepare version v1.0.0"
```

### Step 2: Create a Git Tag
Important: The Tag version should match the version in `package.json`.
```bash
git tag v1.0.0
```

### Step 3: Push to Server (GitHub/GitLab)
```bash
git push origin master --tags
```

---

## 3. Sharing and Importing into Other Projects
Once the tag is pushed to GitHub, you can share the link or use it in your own projects:

### Option A: Import via Package Manager UI
1. Open Unity -> **Window** -> **Package Manager**.
2. Click the **[+]** button in the top left corner.
3. Select **Add package from git URL...**
4. Enter the URL in this format: `https://github.com/user/project-repo.git#v1.0.0`
   * *(Note: The `#v1.0.0` suffix specifies the exact Tag)*.

### Option B: Direct Edit to `manifest.json`
Add the following line to the `dependencies` list in your project's `Packages/manifest.json` file:
```json
"com.nhemdangfugbixs.tooling": "https://github.com/user/project-repo.git#v1.0.0"
```

---

## 4. Why Use Tags?
- **Stability**: While you continue working on the master branch, projects using the `#v1.0.0` version won't be affected by new bugs.
- **Easy Rollback**: If version `v1.1.0` has issues, simply change your URL back to `#v1.0.0` to restore stability instantly.
- **Professionalism**: It helps your team know exactly which version of the tools they are using.
