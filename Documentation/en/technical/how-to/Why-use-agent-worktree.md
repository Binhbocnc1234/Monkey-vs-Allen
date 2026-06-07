---
type: technical
audience: [developer]
status: active
language: en
description: Explains why agent tasks should use Git worktrees instead of sharing one working folder with developers.
related:
  - how_to_documentation
---

# Introduction

This document explains why agent work should use a separate Git worktree instead of sharing the developer's current project folder.

The goal is not to replace Git branches. A worktree still uses a branch. The difference is that a worktree gives the agent a separate folder, while branch-only work keeps developer and agent edits in the same folder.

**Purpose:**
- Protect developer edits that are not committed yet.
- Let developer and agent work at the same time.
- Make agent output easier to review and discard.
- Reduce accidental file restore, overwrite, or cleanup mistakes.

---

# The Problem With One Shared Folder

When developer and agent share one project folder, they also share one working tree. Git can separate commit history by branch, but it cannot separate uncommitted files inside the same folder.

This creates common failure modes:

| Failure mode | Why it happens |
|--------------|----------------|
| Agent edits a file developer is also editing | Both operate on the same physical file. |
| Agent cannot switch branch | Git blocks checkout because dirty files would be overwritten. |
| Agent stages unrelated files | The same `git status` includes developer edits and agent edits. |
| Agent restores deleted content | Agent sees a deleted file as a change and may treat it as something to "fix". |
| Developer loses flow | Developer must commit, stash, or pause before agent can work safely. |

Branch-only workflow is safe when the folder is clean and only one actor is editing. It becomes fragile when developer and agent work concurrently.

---

# Why Worktree Helps

A Git worktree gives the agent a separate folder checked out to its own branch. Developer keeps the main folder. Agent works in the worktree folder.

```text
Monkey vs Allen/                         developer workspace
Monkey vs Allen.worktrees/task-name/     agent workspace
```

The important separation is physical, not only historical. Each side has its own files on disk.

| Concern | Shared folder with branches | Separate worktree |
|---------|-----------------------------|-------------------|
| Commit history | Separated by branch | Separated by branch |
| Files on disk | Shared | Separate |
| Dirty developer edits | Block or mix with agent work | Stay in developer folder |
| Agent review | Possible but noisier | Clean branch diff |
| Parallel work | Risky | Normal |
| Discarding agent task | Manual cleanup in shared folder | Remove worktree and branch |

---

# Recommended Workflow

**Preconditions:**
- Developer is in the main repo folder.
- Main repo `git status --short` is clean, or dirty files are intentionally developer-owned.
- Agent work does not need uncommitted developer changes unless those changes are committed first.

**Steps:**
1. Create a dedicated worktree for the agent task.

```powershell
python .\tools\create_agent_worktree.py <task-slug> --base master
```

2. Start the agent inside the worktree path printed by the tool.

```powershell
cd "..\Monkey vs Allen.worktrees\<task-slug>"
codex
```

3. Let the agent edit, validate, and commit inside that worktree.

```powershell
git status --short
git add <files>
git commit -m "<task summary>"
```

4. Review the agent branch from the main repo.

```powershell
git diff master..agent/<task-slug>
```

5. Merge the branch after review.

```powershell
git switch master
git merge agent/<task-slug>
```

6. Clean up the worktree when the task is done.

```powershell
python .\tools\cleanup_agent_worktree.py "..\Monkey vs Allen.worktrees\<task-slug>" --branch agent/<task-slug>
```

**Expected results:**
- Developer can keep using the main folder.
- Agent changes are isolated to a task branch and task folder.
- Review uses normal Git diff and merge.

---

# When To Use Worktree

Use a worktree for:

- Documentation edits.
- Multi-file code changes.
- Refactors.
- Generated or formatted output.
- Tasks that may run tools which touch many files.
- Any task where developer may continue working in the main folder.

Use the shared folder only for read-only investigation or very small direct edits when developer explicitly wants that.

---

# Error Handling

| Case | Action |
|------|--------|
| Agent needs uncommitted developer changes | Commit those changes first, or create the worktree from a branch containing them. |
| Worktree branch conflicts with `master` | Resolve conflict during merge after reviewing agent changes. |
| Worktree path already exists | List existing worktrees and remove the finished one before retrying. |
| Agent edited the main folder by mistake | Stop, inspect `git status`, and move or discard only agent-owned changes. |
| Task is no longer needed | Remove the worktree and delete the task branch. |

---

# Verification

The worktree workflow is correct when:

- `git worktree list` shows a separate folder for the agent task.
- Developer edits remain in the main folder.
- Agent edits appear only on `agent/<task-slug>`.
- `git diff master..agent/<task-slug>` shows only the task scope.
- The main folder stays clean unless developer intentionally changes it.
