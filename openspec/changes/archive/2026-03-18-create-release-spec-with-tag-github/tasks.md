## 1. GitHub Action Workflow Setup

- [x] 1.1 Create a new GitHub Actions workflow file (e.g., `.github/workflows/release.yml`).
- [x] 1.2 Configure the workflow to trigger upon push to designated release branches (e.g., `main`, `release/*`).
- [x] 1.3 Add initial jobs to the workflow, including checkout, setup-node/setup-dotnet (if applicable), and dependency installation.

## 2. Versioning and Tagging Logic

- [x] 2.1 Implement a script or use an existing GitHub Action (e.g., `conventional-changelog-action`, `semantic-release`) to determine the next semantic version.
- [x] 2.2 Add a step to the workflow to create the Git tag based on the determined version.
- [x] 2.3 Add a step to the workflow to push the newly created Git tag to the repository.

## 3. GitHub Release Creation

- [x] 3.1 Integrate `actions/create-release` into the workflow.
- [x] 3.2 Configure the `actions/create-release` step to use the generated tag.
- [x] 3.3 Implement logic to generate release notes, pulling content from `CHANGELOG.md` or recent commit messages/PR titles.
- [x] 3.4 Populate the GitHub Release body with the generated release notes.

## 4. Testing and Validation

- [x] 4.1 Implement a dry-run mode for the release workflow to validate its configuration without actually creating a release.
- [x] 4.2 Add unit/integration tests for any custom scripts used in versioning or release note generation.
- [x] 4.3 Configure branch protection rules to ensure the release workflow only runs on approved merges.
