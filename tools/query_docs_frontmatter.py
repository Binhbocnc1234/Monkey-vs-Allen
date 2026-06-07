from __future__ import annotations

import argparse
from pathlib import Path
import sys

from docs_tool_lib import load_docs, value_list


def field_matches(value: object, expected: str | None) -> bool:
    if not expected:
        return True
    return expected in value_list(value)


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Query markdown documents by YAML frontmatter.")
    parser.add_argument("--root", default="./Documentation", help="Documentation root.")
    parser.add_argument("--type")
    parser.add_argument("--audience")
    parser.add_argument("--status")
    parser.add_argument("--language")
    parser.add_argument("--description-contains")
    args = parser.parse_args()

    root = Path(args.root)
    if not root.exists():
        print(f"Documentation root not found: {root}")
        return 1

    rows = []
    for doc in load_docs(root):
        frontmatter = doc.frontmatter
        if not field_matches(frontmatter.get("type"), args.type):
            continue
        if not field_matches(frontmatter.get("audience"), args.audience):
            continue
        if not field_matches(frontmatter.get("status"), args.status):
            continue
        if not field_matches(frontmatter.get("language"), args.language):
            continue
        description = str(frontmatter.get("description", ""))
        if args.description_contains and args.description_contains.lower() not in description.lower():
            continue

        rows.append((
            str(doc.path.relative_to(root)),
            str(frontmatter.get("type", "")),
            ", ".join(value_list(frontmatter.get("audience"))),
            str(frontmatter.get("status", "")),
            str(frontmatter.get("language", "")),
            description,
        ))

    print("Path\tType\tAudience\tStatus\tLanguage\tDescription")
    for row in rows:
        print("\t".join(row))

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
