## Why

The current release process often involves manual steps for creating GitHub releases and tags, which can be error-prone and time-consuming. This change aims to automate the creation of GitHub releases with corresponding version tags, ensuring consistency, reducing manual effort, and streamlining the deployment pipeline. This automation will lead to more reliable and faster release cycles.

## What Changes

- Introduction of an automated process or tool to create GitHub releases and associated git tags.
- Integration with the continuous integration/continuous deployment (CI/CD) pipeline to trigger this automation upon successful build and testing.
- Definition of a consistent versioning strategy that will be applied to the tags and releases.

## Capabilities

### New Capabilities
- `github-release-tagging`: Automates the creation of GitHub releases and corresponding semantic version tags.

### Modified Capabilities
<!-- Existing capabilities whose REQUIREMENTS are changing (not just implementation).
     Only list here if spec-level behavior changes. Each needs a delta spec file.
     Use existing spec names from openspec/specs/. Leave empty if no requirement changes. -->

## Impact

- **CI/CD Pipeline**: New steps for release creation and tagging.
- **GitHub Repository**: Automated creation of releases and tags, improving visibility and traceability of deployments.
- **Development Workflow**: Reduced manual intervention in the release process, allowing developers to focus on feature development.
- **Version Control**: Enforced semantic versioning for releases.
