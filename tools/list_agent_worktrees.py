from __future__ import annotations

from pathlib import Path
import subprocess
import sys


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    repo_result = subprocess.run(
        ["git", "rev-parse", "--show-toplevel"],
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if repo_result.returncode != 0:
        print(repo_result.stderr.rstrip(), file=sys.stderr)
        return repo_result.returncode

    repo = Path(repo_result.stdout.strip())
    result = subprocess.run(
        ["git", "worktree", "list", "--porcelain"],
        cwd=repo,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        print(result.stderr.rstrip(), file=sys.stderr)
        return result.returncode

    print(result.stdout.rstrip())
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
