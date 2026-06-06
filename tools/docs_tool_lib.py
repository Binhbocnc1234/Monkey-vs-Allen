from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
import re


VALID_TYPES = {"gameplay", "technical"}
VALID_AUDIENCES = {"player", "developer", "agent"}
VALID_STATUSES = {"draft", "active", "deprecated"}
VALID_LANGUAGES = {"en", "vi"}


@dataclass
class MarkdownDoc:
    path: Path
    frontmatter: dict[str, object]


def normalize_doc_id(value: str) -> str:
    return re.sub(r"[^a-z0-9]+", "", value.lower())


def parse_scalar_or_list(value: str) -> object:
    value = value.strip()
    if value.startswith("[") and value.endswith("]"):
        inner = value[1:-1].strip()
        if not inner:
            return []
        return [item.strip().strip('"').strip("'") for item in inner.split(",")]
    return value.strip('"').strip("'")


def parse_frontmatter(path: Path) -> tuple[dict[str, object] | None, str | None]:
    lines = path.read_text(encoding="utf-8-sig").splitlines()
    if len(lines) < 2 or lines[0].strip() != "---":
        return None, "missing frontmatter block"

    end_index = None
    for index in range(1, len(lines)):
        if lines[index].strip() == "---":
            end_index = index
            break

    if end_index is None:
        return None, "unterminated frontmatter block"

    data: dict[str, object] = {}
    current_key: str | None = None

    for line in lines[1:end_index]:
        list_match = re.match(r"^\s*-\s+(.+?)\s*$", line)
        if list_match and current_key:
            existing = data.get(current_key)
            if not isinstance(existing, list):
                existing = []
            existing.append(list_match.group(1).strip().strip('"').strip("'"))
            data[current_key] = existing
            continue

        key_match = re.match(r"^([A-Za-z_][A-Za-z0-9_-]*)\s*:\s*(.*)$", line)
        if key_match:
            current_key = key_match.group(1)
            raw_value = key_match.group(2)
            data[current_key] = [] if not raw_value.strip() else parse_scalar_or_list(raw_value)

    return data, None


def value_list(value: object) -> list[str]:
    if value is None:
        return []
    if isinstance(value, list):
        return [str(item).strip() for item in value if str(item).strip()]
    text = str(value).strip()
    if "," in text:
        return [item.strip() for item in text.split(",") if item.strip()]
    return [text] if text else []


def iter_markdown_docs(root: Path) -> list[Path]:
    return sorted(root.rglob("*.md"))


def load_docs(root: Path) -> list[MarkdownDoc]:
    docs: list[MarkdownDoc] = []
    for path in iter_markdown_docs(root):
        frontmatter, error = parse_frontmatter(path)
        if error or frontmatter is None:
            continue
        docs.append(MarkdownDoc(path=path, frontmatter=frontmatter))
    return docs


def infer_language_from_path(path: Path) -> str | None:
    parts = set(path.parts)
    if "en" in parts:
        return "en"
    if "vi" in parts:
        return "vi"
    return None


def doc_identifiers(doc: MarkdownDoc) -> set[str]:
    ids = {
        normalize_doc_id(doc.path.stem),
        normalize_doc_id(doc.path.stem.replace("-vn", "")),
        normalize_doc_id(doc.path.stem.replace("_vn", "")),
    }
    module = doc.frontmatter.get("module")
    if module:
        ids.add(normalize_doc_id(str(module)))
    return {item for item in ids if item}


def related_target_exists(related_name: str, source: MarkdownDoc, docs: list[MarkdownDoc]) -> bool:
    source_language = str(source.frontmatter.get("language", "")).strip()
    target_id = normalize_doc_id(related_name)

    for candidate in docs:
        if candidate.path == source.path:
            continue
        if str(candidate.frontmatter.get("language", "")).strip() != source_language:
            continue
        if target_id in doc_identifiers(candidate):
            return True

    return False
