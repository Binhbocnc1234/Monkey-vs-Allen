---
name: agent-branch
description: Use when Codex needs to make non-trivial edits in this repo with a shared-directory Git branch workflow instead of worktrees. Keeps an agent task branch synchronized with master and merges completed agent work back into master after review.
---

# Agent Branch Workflow

Use a normal Git branch in the shared repository directory. Do not create a worktree by default.

## Rules

- Branch name: `agent/<task-slug>`.
- Always inspect `git status --short` before switching branches, merging, or editing.
- Treat existing dirty files as user-owned unless you created them in the current task.
- Do not switch branches with dirty user-owned changes. Ask the user to commit, stash, or approve the next step.
- While working on an agent branch, keep it current by merging `master` into it.
- Merge the agent branch back into `master` only after the user asks for integration or approves the completed task.
- If a merge conflict appears, stop and report the conflicting files. Resolve only when the user intent is clear.

## Start

```powershell
git status --short
git switch master
git switch -c agent/<task-slug>
git merge master
```

If `master` has changed after the branch was created:

```powershell
git switch agent/<task-slug>
git merge master
```

## Work

1. Make scoped edits.
2. Run the relevant checks.
3. Commit only the agent-owned files:

```powershell
git status --short
git add <files>
git commit -m "<task summary>"
```

Before review or handoff, sync from `master` again:

```powershell
git switch agent/<task-slug>
git merge master
```

## Integrate

After review/approval:

```powershell
git switch master
git merge agent/<task-slug>
```

If the agent branch remains active after integration, sync it from `master`:

```powershell
git switch agent/<task-slug>
git merge master
```

Deprecated worktree plan: `Documentation/plan/worktree-setup.md`.
