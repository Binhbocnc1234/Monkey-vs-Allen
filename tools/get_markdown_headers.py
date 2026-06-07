from __future__ import annotations

import argparse
from pathlib import Path
import re
import sys


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="List H1 and H2 headings with line ranges.")
    parser.add_argument("path", help="Markdown file path.")
    args = parser.parse_args()

    path = Path(args.path)
    if not path.exists():
        print(f"Markdown file not found: {path}")
        return 1

    lines = path.read_text(encoding="utf-8-sig").splitlines()
    sections: list[dict[str, object]] = []
    in_fence = False

    for index, line in enumerate(lines, start=1):
        if re.match(r"^\s*(```|~~~)", line):
            in_fence = not in_fence
            continue
        if in_fence:
            continue

        match = re.match(r"^(#{1,2})\s+(.+?)\s*$", line)
        if match:
            level = len(match.group(1))
            header = re.sub(r"\s+#+\s*$", "", match.group(2))
            sections.append({"level": f"H{level}", "header": header, "start": index, "end": None})

    for index, section in enumerate(sections):
        if index < len(sections) - 1:
            section["end"] = int(sections[index + 1]["start"]) - 1
        else:
            section["end"] = len(lines)

    print("Level\tHeader\tRange")
    for section in sections:
        print(f"{section['level']}\t{section['header']}\t[{section['start']},{section['end']}]")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
