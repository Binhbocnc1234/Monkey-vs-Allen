from __future__ import annotations

import argparse
from pathlib import Path
import re
import subprocess
import sys


def run_git(args: list[str], cwd: Path, check: bool = True) -> subprocess.CompletedProcess[str]:
    result = subprocess.run(
        ["git", *args],
        cwd=cwd,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if check and result.returncode != 0:
        if result.stdout:
            print(result.stdout.rstrip())
        if result.stderr:
            print(result.stderr.rstrip(), file=sys.stderr)
        raise SystemExit(result.returncode)
    return result


def slugify(value: str) -> str:
    slug = re.sub(r"[^A-Za-z0-9._-]+", "-", value.strip()).strip("-")
    return slug.lower() or "task"


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Create a task-specific Git worktree for agent work.")
    parser.add_argument("task", help="Task slug, such as docs-frontmatter.")
    parser.add_argument("--base", default="master", help="Base ref for the new branch. Default: master.")
    parser.add_argument("--branch-prefix", default="agent", help="Branch prefix. Default: agent.")
    parser.add_argument("--worktrees-root", help="Directory that stores agent worktrees. Default: sibling '<repo>.worktrees'.")
    parser.add_argument("--allow-dirty", action="store_true", help="Allow creation when the main workspace is dirty.")
    args = parser.parse_args()

    repo = Path(run_git(["rev-parse", "--show-toplevel"], Path.cwd()).stdout.strip())
    task = slugify(args.task)
    branch = f"{args.branch_prefix.rstrip('/')}/{task}"

    default_root = repo.parent / f"{repo.name}.worktrees"
    worktrees_root = Path(args.worktrees_root) if args.worktrees_root else default_root
    worktree_path = worktrees_root / task

    status = run_git(["status", "--short"], repo).stdout.strip()
    if status and not args.allow_dirty:
        print("Main workspace is dirty. Commit/stash relevant changes first, or pass --allow-dirty if intentional.", file=sys.stderr)
        print(status)
        return 2

    existing_branch = run_git(["branch", "--list", branch], repo).stdout.strip()
    if existing_branch:
        print(f"Branch already exists: {branch}", file=sys.stderr)
        return 3

    if worktree_path.exists():
        print(f"Worktree path already exists: {worktree_path}", file=sys.stderr)
        return 4

    worktrees_root.mkdir(parents=True, exist_ok=True)
    run_git(["worktree", "add", "-b", branch, str(worktree_path), args.base], repo)

    print(f"Created worktree: {worktree_path}")
    print(f"Branch: {branch}")
    print(f"Next:")
    print(f"  cd \"{worktree_path}\"")
    print("  git status --short")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
