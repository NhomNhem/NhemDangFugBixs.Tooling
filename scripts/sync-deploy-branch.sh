#!/usr/bin/env bash
set -euo pipefail

REMOTE_NAME="${1:-origin}"
SOURCE_BRANCH="${2:-}"
TARGET_BRANCH="${3:-deploy}"

if [[ -z "$SOURCE_BRANCH" ]]; then
  echo "[Nhem] sync-deploy: missing source branch"
  exit 1
fi

if [[ "$SOURCE_BRANCH" != "main" && "$SOURCE_BRANCH" != "master" ]]; then
  echo "[Nhem] sync-deploy: source branch '$SOURCE_BRANCH' is not main/master, skipping."
  exit 0
fi

ROOT_DIR="$(git rev-parse --show-toplevel)"
CURRENT_HEAD="$(git rev-parse HEAD)"
SHORT_SHA="$(git rev-parse --short HEAD)"
TMP_DIR="$(mktemp -d)"
WORKTREE_DIR="$TMP_DIR/deploy-worktree"

cleanup() {
  if [[ -d "$WORKTREE_DIR" ]]; then
    git worktree remove "$WORKTREE_DIR" --force >/dev/null 2>&1 || true
  fi
  rm -rf "$TMP_DIR"
}
trap cleanup EXIT

echo "[Nhem] sync-deploy: preparing worktree for $TARGET_BRANCH"
git fetch "$REMOTE_NAME" "$TARGET_BRANCH" >/dev/null 2>&1 || true

if git show-ref --verify --quiet "refs/remotes/$REMOTE_NAME/$TARGET_BRANCH"; then
  git worktree add --detach "$WORKTREE_DIR" "refs/remotes/$REMOTE_NAME/$TARGET_BRANCH" >/dev/null
else
  git worktree add --detach "$WORKTREE_DIR" "$CURRENT_HEAD" >/dev/null
  (
    cd "$WORKTREE_DIR"
    git checkout --orphan "$TARGET_BRANCH" >/dev/null
  )
fi

(
  cd "$WORKTREE_DIR"

  # Clean current worktree content.
  git rm -rf . >/dev/null 2>&1 || true
  find . -mindepth 1 -maxdepth 1 \
    ! -name ".git" \
    ! -name "." \
    -exec rm -rf {} +

  copy_if_exists() {
    local source="$1"
    local target="$2"
    if [[ -e "$ROOT_DIR/$source" ]]; then
      mkdir -p "$(dirname "$target")"
      cp -a "$ROOT_DIR/$source" "$target"
      return 0
    fi
    return 1
  }

  # Root files required by the package.
  copy_if_exists "package.json" "package.json"
  copy_if_exists "README.md" "README.md"
  copy_if_exists "CHANGELOG.md" "CHANGELOG.md"
  copy_if_exists "Third Party Notices.md" "Third Party Notices.md"
  copy_if_exists "LICENSE" "LICENSE" || copy_if_exists "LICENSE.md" "LICENSE.md"

  # Unity package folders.
  copy_if_exists "Runtime" "Runtime"
  copy_if_exists "Editor" "Editor"
  copy_if_exists "Analyzers" "Analyzers"
  copy_if_exists "Tests" "Tests"

  # Normalize folder names for package consumers.
  if ! copy_if_exists "Samples" "Samples"; then
    copy_if_exists "Samples~" "Samples" || true
  fi

  if ! copy_if_exists "Documentation" "Documentation"; then
    copy_if_exists "Documentation~" "Documentation" || true
  fi

  # Remove AI/dev-only artifacts from deploy branch.
  rm -rf .agent .codex openspec scripts Source~ .github .githooks .vscode || true
  rm -f AGENTS.md QWEN.md GEMINI.md .openspec.yaml CONTRIBUTING.md || true
  find . -type f \( -name "AGENTS.md" -o -name "QWEN.md" -o -name "GEMINI.md" -o -name ".openspec.yaml" \) -delete

  git add -A

  if git diff --cached --quiet; then
    echo "[Nhem] sync-deploy: no deploy changes to commit."
    exit 0
  fi

  git -c user.name="nhemdangfugbixs-bot" -c user.email="bot@nhemdangfugbixs.local" \
    commit -m "chore(deploy): sync from $SOURCE_BRANCH @ $SHORT_SHA" >/dev/null

  echo "[Nhem] sync-deploy: pushing $TARGET_BRANCH to $REMOTE_NAME"
  NHEM_SKIP_DEPLOY_HOOK=1 git push "$REMOTE_NAME" "HEAD:refs/heads/$TARGET_BRANCH"
)

echo "[Nhem] sync-deploy: done."
