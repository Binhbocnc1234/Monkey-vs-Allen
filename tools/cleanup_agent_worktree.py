from __future__ import annotations

import argparse
from pathlib import Path
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


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Remove an agent Git worktree and optionally delete its branch.")
    parser.add_argument("worktree_path", help="Path to the worktree to remove.")
    parser.add_argument("--branch", help="Branch to delete after worktree removal.")
    parser.add_argument("--force", action="store_true", help="Force worktree and branch deletion.")
    args = parser.parse_args()

    repo = Path(run_git(["rev-parse", "--show-toplevel"], Path.cwd()).stdout.strip())
    worktree_path = Path(args.worktree_path)
    remove_args = ["worktree", "remove"]
    if args.force:
        remove_args.append("--force")
    remove_args.append(str(worktree_path))
    run_git(remove_args, repo)
    run_git(["worktree", "prune"], repo)

    if args.branch:
        delete_flag = "-D" if args.force else "-d"
        run_git(["branch", delete_flag, args.branch], repo)

    print(f"Removed worktree: {worktree_path}")
    if args.branch:
        print(f"Deleted branch: {args.branch}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
