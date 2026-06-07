from __future__ import annotations

import argparse
from pathlib import Path
import sys

from docs_tool_lib import doc_identifiers, load_docs, normalize_doc_id, value_list


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Find outgoing and incoming related-document links.")
    parser.add_argument("--id", required=True, help="Document id or file stem.")
    parser.add_argument("--root", default="./Documentation", help="Documentation root.")
    args = parser.parse_args()

    root = Path(args.root)
    if not root.exists():
        print(f"Documentation root not found: {root}")
        return 1

    docs = load_docs(root)
    target_id = normalize_doc_id(args.id)
    rows = []

    for doc in docs:
        related = value_list(doc.frontmatter.get("related"))
        if target_id in doc_identifiers(doc):
            for item in related:
                rows.append(("outgoing", str(doc.path.relative_to(root)), item))

        if any(normalize_doc_id(item) == target_id for item in related):
            rows.append(("incoming", str(doc.path.relative_to(root)), args.id))

    print("Direction\tSource\tTarget")
    for row in sorted(rows):
        print("\t".join(row))

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
