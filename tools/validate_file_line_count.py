from __future__ import annotations

import argparse
from pathlib import Path
import subprocess
import sys


DEFAULT_EXTENSIONS = (".cs", ".md")
DEFAULT_EXCLUDES = (
    ".git/",
    "Monkey vs Allen project/",
    "Raw assets/",
    "Releases/",
)


def normalize_path(path: str) -> str:
    return path.replace("\\", "/")


def tracked_files(root: Path) -> list[str]:
    result = subprocess.run(
        ["git", "ls-files"],
        cwd=root,
        capture_output=True,
        text=True,
        check=False,
    )
    if result.returncode != 0:
        raise RuntimeError(result.stderr.strip() or "git ls-files failed")
    return [line.strip() for line in result.stdout.splitlines() if line.strip()]


def parse_extensions(value: str) -> tuple[str, ...]:
    extensions: list[str] = []
    for item in value.split(","):
        item = item.strip().lower()
        if not item:
            continue
        extensions.append(item if item.startswith(".") else f".{item}")
    return tuple(extensions)


def is_excluded(path: str, excludes: tuple[str, ...]) -> bool:
    normalized = normalize_path(path)
    return any(normalized == item.rstrip("/") or normalized.startswith(item) for item in excludes)


def count_lines(path: Path) -> int:
    with path.open("r", encoding="utf-8-sig", errors="ignore") as file:
        return sum(1 for _ in file)


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Validate tracked file line counts.")
    parser.add_argument("--root", default=".", help="Repository root.")
    parser.add_argument("--max-lines", type=int, default=250, help="Maximum allowed lines per file.")
    parser.add_argument(
        "--extensions",
        default=",".join(DEFAULT_EXTENSIONS),
        help="Comma-separated extensions to check.",
    )
    parser.add_argument(
        "--exclude",
        action="append",
        default=list(DEFAULT_EXCLUDES),
        help="Path prefix to exclude. Can be passed more than once.",
    )
    args = parser.parse_args()

    root = Path(args.root).resolve()
    if not root.exists():
        print(f"Root not found: {root}")
        return 1

    extensions = parse_extensions(args.extensions)
    excludes = tuple(normalize_path(item).rstrip("/") + "/" for item in args.exclude)
    issues: list[tuple[int, str]] = []

    try:
        candidates = tracked_files(root)
    except RuntimeError as error:
        print(error)
        return 1

    for relative_path in candidates:
        if is_excluded(relative_path, excludes):
            continue
        path = root / relative_path
        if path.suffix.lower() not in extensions or not path.is_file():
            continue

        line_count = count_lines(path)
        if line_count > args.max_lines:
            issues.append((line_count, normalize_path(relative_path)))

    if issues:
        print("Lines\tMax\tPath")
        for line_count, path in sorted(issues, key=lambda item: (item[1], item[0])):
            print(f"{line_count}\t{args.max_lines}\t{path}")
        return 2

    checked = ", ".join(extensions)
    print(f"All tracked {checked} files are within {args.max_lines} lines.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
