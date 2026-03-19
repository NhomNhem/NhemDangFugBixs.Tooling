## ADDED Requirements

### Requirement: Automated GitHub Release and Tag Creation
The system SHALL automatically create a new GitHub release and a corresponding git tag upon a successful CI/CD pipeline run on a designated release branch.

#### Scenario: Successful Release and Tag Creation
- **WHEN** a CI/CD pipeline completes successfully on a designated release branch
- **THEN** a new GitHub release is created
- **AND** a git tag matching the release version is pushed to the repository
- **AND** the release notes are populated with changes from `CHANGELOG.md`
