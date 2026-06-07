from __future__ import annotations

import argparse
from pathlib import Path
import re
import sys


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Read a markdown line range [a,b].")
    parser.add_argument("path", help="Markdown file path.")
    parser.add_argument("range", help="Line range in [a,b] format.")
    args = parser.parse_args()

    path = Path(args.path)
    if not path.exists():
        print(f"Markdown file not found: {path}")
        return 1

    match = re.match(r"^\s*\[\s*(\d+)\s*,\s*(\d+)\s*\]\s*$", args.range)
    if not match:
        print("Range must use the format [a,b], such as [81,126].")
        return 2

    start = int(match.group(1))
    end = int(match.group(2))
    if start < 1 or end < start:
        print(f"Invalid range: {args.range}")
        return 3

    lines = path.read_text(encoding="utf-8-sig").splitlines()
    if start > len(lines):
        print(f"Range start {start} is beyond file length {len(lines)}.")
        return 4

    end = min(end, len(lines))
    for line in lines[start - 1:end]:
        print(line)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
