---
name: agent-worktree
description: Deprecated. Do not use by default in this repo; use .codex/skills/agent-branch/SKILL.md for non-trivial edits.
---

# Agent Worktree

Deprecated. The current project workflow uses shared-directory Git branches instead of worktrees. Use `.codex/skills/agent-branch/SKILL.md`.

Use a dedicated Git worktree for non-trivial edits. Keep the user's main workspace developer-owned.

## Rules

- Do not edit the primary workspace directly for non-trivial work unless the user explicitly asks.
- Treat dirty files in the primary workspace as user-owned changes.
- Do not restore deleted content unless the user explicitly requests restoration.
- Work only inside the assigned worktree after it is created.

## Workflow

1. Inspect the main workspace:

```powershell
git status --short
```

2. If the main workspace is dirty, ask the user to commit/stash if the worktree needs those changes. Use `--allow-dirty` only when the user agrees to create from current `HEAD`.

3. Create a task worktree:

```powershell
python .\tools\create_agent_worktree.py <task-slug>
```

4. Move into the worktree and work there:

```powershell
cd "..\Monkey vs Allen.worktrees\<task-slug>"
git status --short
```

5. Run checks from the worktree. For documentation edits:

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation
```

6. Commit the agent's changes on the task branch:

```powershell
git add <files>
git commit -m "<task summary>"
```

7. Report branch, worktree path, changed files, and verification commands.

## Cleanup

List worktrees:

```powershell
python .\tools\list_agent_worktrees.py
```

Remove a merged/finished worktree:

```powershell
python .\tools\cleanup_agent_worktree.py "<worktree-path>" --branch agent/<task-slug>
```

Discard an unmerged task:

```powershell
python .\tools\cleanup_agent_worktree.py "<worktree-path>" --branch agent/<task-slug> --force
```

Detailed project plan: `Documentation/plan/worktree-setup.md`.
