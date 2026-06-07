---
type: technical

audience: [agent]
status: active
language: en
description: Explains how to use project documentation tools to query frontmatter, inspect headings, read specific ranges, validate metadata, and follow related documents.
related:
  - how_to_documentation
---

# Introduction

This document explains how to read project documentation with the local tools in `tools/`. These tools are intended for agents first, but developers can use the same commands when navigating the documentation manually.

Use the `.py` tools as the cross-platform default. The older `.ps1` tools are kept for Windows PowerShell workflows.

**Purpose:**
- Find the right document before reading full files.
- Read only the section needed for the current task.
- Validate whether documentation metadata is usable for agent queries.
- Follow related documentation without relying on broad text search.

---

# Tool Summary

| Tool | Use it for | Gap it fills |
|------|------------|--------------|
| `get_markdown_headers.py` | List H1 and H2 headings with line ranges. | `rg` finds matching text but does not show section boundaries. |
| `query_docs_frontmatter.py` | Filter documents by YAML fields such as `type`, `audience`, `status`, `language`, and `description`. | Plain text search does not reliably combine YAML field filters. |
| `read_markdown_section.py` | Read a file slice by range `[a,b]`. | `Get-Content` can slice lines, but this tool gives a stable interface paired with header ranges. |
| `validate_docs_frontmatter.py` | Check required YAML fields, allowed enum values, and same-language `related` targets. | `rg` can show likely issues but cannot prove every file follows the schema. |
| `find_related_docs.py` | Find outgoing and incoming `related` links for a document id. | Plain search cannot distinguish documents that point to an id from documents only mentioning it. |

---

# Recommended Workflow

**Steps:**
1. Query metadata to find candidate documents.
2. Inspect headings in the most relevant document.
3. Read only the needed section range.
4. Follow related documents if the current file is not enough.
5. Validate frontmatter after creating or editing documentation.

**Expected result:**
- The reader finds the relevant document and section without reading unrelated files.
- Agent decisions are based on structured metadata before full-content reads.

---

# Query Documents by Frontmatter

Use `query_docs_frontmatter.py` when the task starts with a broad question such as "where is the testing documentation?" or "which active docs are for agents?"

**Command:**

```powershell
python .\tools\query_docs_frontmatter.py --audience agent --status active --language en
```

**Useful filters:**

| Filter | Meaning |
|--------|---------|
| `--type technical` | Show only technical documents. |
| `--audience agent` | Show documents intended for agents. |
| `--status active` | Show documents that can be treated as current sources. |
| `--language en` | Show documents in one language. |
| `--description-contains testing` | Search document-level descriptions. |

**Why this exists:**
- `rg` can find strings, but it does not understand YAML lists, enum values, or combined filters.
- This tool helps agents choose files before reading full content.

---

# Inspect Headings

Use `get_markdown_headers.py` after choosing a candidate document. The tool lists H1 and H2 headings with line ranges.

**Command:**

```powershell
python .\tools\get_markdown_headers.py .\Documentation\en\technical\how-to\How-to-documentation.md
```

**Output shape:**

```text
Level Header           Range
----- ------           -----
H1    Introduction     [11,25]
H2    YAML Frontmatter [81,126]
```

**Why this exists:**
- Headings show the document structure.
- Ranges let agents read only the relevant section.
- The tool ignores headings inside fenced code blocks.

---

# Read a Section by Range

Use `read_markdown_section.py` after getting a range from `get_markdown_headers.py`.

**Command:**

```powershell
python .\tools\read_markdown_section.py .\Documentation\en\technical\how-to\How-to-documentation.md "[81,126]"
```

**Why this exists:**
- `Get-Content` requires manual line slicing logic.
- This tool provides one simple interface: file plus `[a,b]`.
- It keeps agent reads focused and reduces irrelevant context.

---

# Validate Frontmatter

Use `validate_docs_frontmatter.py` after creating or editing documentation. This tool validates every markdown file under the `--root` directory, not only one file.

**Command:**

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation\en
```

**Single-folder check:**

To validate the files in one documentation folder, pass that folder as `--root`.

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation\en\technical\how-to
```

**Checks:**
- Required fields: `type`, `audience`, `status`, `language`, `description`.
- `type` is `gameplay` or `technical`.
- `audience` uses `player`, `developer`, or `agent`.
- `status` uses `draft`, `active`, or `deprecated`.
- `language` uses `en` or `vi`.
- Optional `related` entries point to a markdown file in the same language.
- Frontmatter starts and ends with `---`.

**Why this exists:**
- Manual review and text search miss schema drift.
- Agents need predictable metadata to query documents reliably.

---

# Follow Related Documents

Use `find_related_docs.py` when a document references another document or when the current document is not enough.

**Command:**

```powershell
python .\tools\find_related_docs.py --id testing --root .\Documentation\en
```

**Result types:**
- `outgoing`: documents listed by the selected document's `related` field.
- `incoming`: documents that list the selected id in their `related` field.

**Why this exists:**
- Documentation relationships form a graph.
- Plain text search cannot reliably show direction.
- Agents can use the graph to gather nearby context without scanning the whole tree.

---

# Error Handling

| Case | Action |
|------|--------|
| A tool says the file does not exist | Check the path and run from the project root. |
| No documents are returned | Relax filters or inspect `status` and `audience` values. |
| Validation fails | Fix the listed frontmatter field before treating the document as complete. |
| A section range is wrong | Re-run `get_markdown_headers.py` and use the current range. |

---

# Verification

The documentation reading workflow is correct when:
- `query_docs_frontmatter.py` can find candidate documents by metadata.
- `get_markdown_headers.py` returns H1/H2 ranges for the selected file.
- `read_markdown_section.py` returns only the requested line range.
- `validate_docs_frontmatter.py` reports no issues for newly edited documents.
