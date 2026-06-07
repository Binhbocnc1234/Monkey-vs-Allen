from __future__ import annotations

import argparse
from pathlib import Path
import sys

from docs_tool_lib import (
    VALID_AUDIENCES,
    VALID_LANGUAGES,
    VALID_STATUSES,
    VALID_TYPES,
    infer_language_from_path,
    iter_markdown_docs,
    load_docs,
    parse_frontmatter,
    related_target_exists,
    value_list,
)


REQUIRED_FIELDS = ("type", "audience", "status", "language", "description")


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    parser = argparse.ArgumentParser(description="Validate markdown YAML frontmatter.")
    parser.add_argument("--root", default="./Documentation", help="Documentation root or subdirectory.")
    args = parser.parse_args()

    root = Path(args.root)
    if not root.exists():
        print(f"Documentation root not found: {root}")
        return 1

    all_docs = load_docs(root)
    issues: list[tuple[str, str, str]] = []

    for path in iter_markdown_docs(root):
        frontmatter, error = parse_frontmatter(path)
        relative_path = str(path.relative_to(root))
        if error or frontmatter is None:
            issues.append((relative_path, "frontmatter", error or "invalid frontmatter"))
            continue

        for field in REQUIRED_FIELDS:
            value = frontmatter.get(field)
            if value is None or (isinstance(value, str) and not value.strip()):
                issues.append((relative_path, field, "missing required field"))

        doc_type = str(frontmatter.get("type", ""))
        if doc_type and doc_type not in VALID_TYPES:
            issues.append((relative_path, "type", f"invalid value '{doc_type}'"))

        for audience in value_list(frontmatter.get("audience")):
            if audience not in VALID_AUDIENCES:
                issues.append((relative_path, "audience", f"invalid value '{audience}'"))

        status = str(frontmatter.get("status", ""))
        if status and status not in VALID_STATUSES:
            issues.append((relative_path, "status", f"invalid value '{status}'"))

        language = str(frontmatter.get("language", ""))
        if language and language not in VALID_LANGUAGES:
            issues.append((relative_path, "language", f"invalid value '{language}'"))

        inferred_language = infer_language_from_path(path)
        if language and inferred_language and language != inferred_language:
            issues.append((relative_path, "language", f"value '{language}' does not match path language '{inferred_language}'"))

        source_doc = next((doc for doc in all_docs if doc.path == path), None)
        if source_doc and language in VALID_LANGUAGES:
            for related in value_list(frontmatter.get("related")):
                if related and not related_target_exists(related, source_doc, all_docs):
                    issues.append((relative_path, "related", f"target '{related}' was not found in language '{language}'"))

    if issues:
        print("Path\tField\tIssue")
        for path, field, issue in sorted(issues):
            print(f"{path}\t{field}\t{issue}")
        return 2

    print(f"All markdown frontmatter is valid under {root}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
