# OpenSpec Directory Structure

## Overview

This document describes how to organize the OpenSpec changes for NhemDangFugBixs.Tooling into the proper directory structure.

## Current State

All files have been created in the repository root (`I:\unityVers\NhemDangFugBixs.Tooling\`) because the `openspec/` directory is gitignored.

## Target Structure

### Change 1: registration-exposure-api-cleanup-7-2-1

**Directory**: `openspec/changes/registration-exposure-api-cleanup-7-2-1/`

**Files**:
- `proposal.md` → Move from `registration-exposure-api-cleanup-7-2-1-proposal.md`
- `design.md` → Move from `registration-exposure-api-cleanup-7-2-1-design.md`
- `tasks.md` → Move from `registration-exposure-api-cleanup-7-2-1-tasks.md`
- `migration-guide.md` → Move from `registration-exposure-api-cleanup-7-2-1-migration-guide.md`
- `docs-update-plan.md` → Move from `registration-exposure-api-cleanup-7-2-1-docs-update-plan.md`
- `test-matrix.md` → Move from `registration-exposure-api-cleanup-7-2-1-test-matrix.md`
- `release-checklist.md` → Move from `registration-exposure-api-cleanup-7-2-1-release-checklist.md`
- `acceptance-criteria.md` → Move from `registration-exposure-api-cleanup-7-2-1-acceptance-criteria.md`
- `release-gate-updates.md` → Move from `registration-exposure-api-cleanup-7-2-1-release-gate-updates.md`
- `validation-plan.md` → Move from `registration-exposure-api-cleanup-7-2-1-validation-plan.md`
- `specs/generator-behavior-spec-delta.md` → Move from `registration-exposure-api-cleanup-7-2-1-generator-spec-delta.md`
- `specs/analyzer-diagnostics-spec-delta.md` → Move from `registration-exposure-api-cleanup-7-2-1-analyzer-spec-delta.md`

### Change 2: tooling-roadmap-7-2-2-to-8-0

**Directory**: `openspec/changes/tooling-roadmap-7-2-2-to-8-0/`

**Files**:
- `proposal.md` → Move from `tooling-roadmap-7-2-2-to-8-0-proposal.md`
- `roadmap.md` → Move from `tooling-roadmap-7-2-2-to-8-0-roadmap.md`
- `design.md` → Move from `tooling-roadmap-7-2-2-to-8-0-design.md`
- `tasks.md` → Move from `tooling-roadmap-7-2-2-to-8-0-tasks.md`
- `acceptance-criteria.md` → Move from `tooling-roadmap-7-2-2-to-8-0-acceptance-criteria.md`
- `non-goals.md` → Move from `tooling-roadmap-7-2-2-to-8-0-non-goals.md`
- `risk-register.md` → Move from `tooling-roadmap-7-2-2-to-8-0-risk-register.md`
- `release-gate-impact.md` → Move from `tooling-roadmap-7-2-2-to-8-0-release-gate-impact.md`
- `migration-plan.md` → Move from `tooling-roadmap-7-2-2-to-8-0-migration-plan.md`
- `spec-deltas.md` → Move from `tooling-roadmap-7-2-2-to-8-0-spec-deltas.md`
- `validation-plan.md` → Move from `tooling-roadmap-7-2-2-to-8-0-validation-plan.md`

## Migration Commands

### Step 1: Force-Add OpenSpec Directories

Since `openspec/` is gitignored, use `git add -f` to force-add the specific directories:

```bash
git add -f openspec/changes/registration-exposure-api-cleanup-7-2-1
git add -f openspec/changes/tooling-roadmap-7-2-2-to-8-0
git add -f OPENSPEC-STRUCTURE.md
```

### Step 2: Create Directory Structure

```bash
# Create directories
mkdir -p openspec/changes/registration-exposure-api-cleanup-7-2-1/specs
mkdir -p openspec/changes/tooling-roadmap-7-2-2-to-8-0
```

### Step 3: Move Files for registration-exposure-api-cleanup-7-2-1

```bash
# Move proposal
mv registration-exposure-api-cleanup-7-2-1-proposal.md openspec/changes/registration-exposure-api-cleanup-7-2-1/proposal.md

# Move design
mv registration-exposure-api-cleanup-7-2-1-design.md openspec/changes/registration-exposure-api-cleanup-7-2-1/design.md

# Move tasks
mv registration-exposure-api-cleanup-7-2-1-tasks.md openspec/changes/registration-exposure-api-cleanup-7-2-1/tasks.md

# Move migration guide
mv registration-exposure-api-cleanup-7-2-1-migration-guide.md openspec/changes/registration-exposure-api-cleanup-7-2-1/migration-guide.md

# Move spec deltas
mv registration-exposure-api-cleanup-7-2-1-generator-spec-delta.md openspec/changes/registration-exposure-api-cleanup-7-2-1/specs/generator-behavior-spec-delta.md
mv registration-exposure-api-cleanup-7-2-1-analyzer-spec-delta.md openspec/changes/registration-exposure-api-cleanup-7-2-1/specs/analyzer-diagnostics-spec-delta.md

# Move docs update plan
mv registration-exposure-api-cleanup-7-2-1-docs-update-plan.md openspec/changes/registration-exposure-api-cleanup-7-2-1/docs-update-plan.md

# Move test matrix
mv registration-exposure-api-cleanup-7-2-1-test-matrix.md openspec/changes/registration-exposure-api-cleanup-7-2-1/test-matrix.md

# Move release checklist
mv registration-exposure-api-cleanup-7-2-1-release-checklist.md openspec/changes/registration-exposure-api-cleanup-7-2-1/release-checklist.md

# Move acceptance criteria
mv registration-exposure-api-cleanup-7-2-1-acceptance-criteria.md openspec/changes/registration-exposure-api-cleanup-7-2-1/acceptance-criteria.md

# Move release gate updates
mv registration-exposure-api-cleanup-7-2-1-release-gate-updates.md openspec/changes/registration-exposure-api-cleanup-7-2-1/release-gate-updates.md

# Move validation plan
mv registration-exposure-api-cleanup-7-2-1-validation-plan.md openspec/changes/registration-exposure-api-cleanup-7-2-1/validation-plan.md
```

### Step 4: Move Files for tooling-roadmap-7-2-2-to-8-0

```bash
# Move roadmap
mv tooling-roadmap-7-2-2-to-8-0-roadmap.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/roadmap.md

# Move proposal
mv tooling-roadmap-7-2-2-to-8-0-proposal.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/proposal.md

# Move design
mv tooling-roadmap-7-2-2-to-8-0-design.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/design.md

# Move tasks
mv tooling-roadmap-7-2-2-to-8-0-tasks.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/tasks.md

# Move acceptance criteria
mv tooling-roadmap-7-2-2-to-8-0-acceptance-criteria.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/acceptance-criteria.md

# Move non-goals
mv tooling-roadmap-7-2-2-to-8-0-non-goals.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/non-goals.md

# Move risk register
mv tooling-roadmap-7-2-2-to-8-0-risk-register.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/risk-register.md

# Move release gate impact
mv tooling-roadmap-7-2-2-to-8-0-release-gate-impact.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/release-gate-impact.md

# Move migration plan
mv tooling-roadmap-7-2-2-to-8-0-migration-plan.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/migration-plan.md

# Move spec deltas
mv tooling-roadmap-7-2-2-to-8-0-spec-deltas.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/spec-deltas.md

# Move validation plan
mv tooling-roadmap-7-2-2-to-8-0-validation-plan.md openspec/changes/tooling-roadmap-7-2-2-to-8-0/validation-plan.md
```

### Step 6: Commit OpenSpec Changes Separately

Since these are planning/spec documents, commit them separately from implementation:

```bash
git commit -m "docs(openspec): add 7.2.1 cleanup and 8.0 roadmap proposals"
```

After this, commit implementation/release separately:
```bash
git commit -m "feat(generator): prefer explicit exposure attributes"
git commit -m "chore(package): rebuild shipped binaries"
git commit -m "chore(release): bump version to 7.2.1"
```

### Step 7: Clean Up Root Directory

```bash
# Remove this structure file
rm OPENSPEC-STRUCTURE.md
```

## Verification

After migration, verify the structure:

```bash
# Verify registration-exposure-api-cleanup-7-2-1 structure
ls -la openspec/changes/registration-exposure-api-cleanup-7-2-1/
ls -la openspec/changes/registration-exposure-api-cleanup-7-2-1/specs/

# Verify tooling-roadmap-7-2-2-to-8-0 structure
ls -la openspec/changes/tooling-roadmap-7-2-2-to-8-0/
```

Expected output should show all files in their correct locations.

## File Summary

### registration-exposure-api-cleanup-7-2-1 (13 files)
- proposal.md
- design.md
- tasks.md
- migration-guide.md
- specs/generator-behavior-spec-delta.md
- specs/analyzer-diagnostics-spec-delta.md
- docs-update-plan.md
- test-matrix.md
- release-checklist.md
- acceptance-criteria.md
- release-gate-updates.md
- validation-plan.md

### tooling-roadmap-7-2-2-to-8-0 (11 files)
- roadmap.md
- proposal.md
- design.md
- tasks.md
- acceptance-criteria.md
- non-goals.md
- risk-register.md
- release-gate-impact.md
- migration-plan.md
- spec-deltas.md
- validation-plan.md

## Product Principle

All changes follow the product principle:
- Services declare intent
- Composition owns registration
- Diagnostics protect architecture

## Pre-Commit Gate Checklist

Before committing and finishing for the day, ensure:

- [ ] openspec changes organized under openspec/changes
- [ ] roadmap file count corrected (11 files)
- [ ] specs committed separately from implementation
- [ ] 7.2.1 release gate PASS with Unity sample compile
- [ ] deploy branch package.json shows 7.2.1
- [ ] tag v7.2.1 exists

**Note**: Use `git add -f openspec/changes/...` instead of modifying .gitignore to avoid accidentally opening up cache/temp files.
