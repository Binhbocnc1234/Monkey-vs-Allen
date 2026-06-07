---
name: read-document
description: Use when an agent needs to find, inspect, read, or validate project documentation under Documentation/ with the local docs tools.
---

# Read Project Documentation

Use local documentation tools in `tools/` before reading full documentation files. Prefer the Python tools as the cross-platform default. The older `.ps1` tools exist for Windows PowerShell workflows.

## Purpose

- Find the right document before reading full files.
- Read only the section needed for the current task.
- Validate whether documentation metadata is usable for agent queries.
- Follow related documentation without broad text search.

## Workflow

1. Query metadata to find candidate documents.
2. Inspect headings in the most relevant document.
3. Read only the needed section range.
4. Follow related documents if the current file is not enough.
5. Validate frontmatter after creating or editing documentation.

Expected result: agent decisions use structured metadata before full-content reads, and the agent avoids reading unrelated files.

## Tool Summary

| Tool | Use it for |
|------|------------|
| `query_docs_frontmatter.py` | Filter documents by YAML fields such as `type`, `audience`, `status`, `language`, and `description`. |
| `get_markdown_headers.py` | List H1 and H2 headings with line ranges. |
| `read_markdown_section.py` | Read a file slice by range `[a,b]`. |
| `find_related_docs.py` | Find outgoing and incoming `related` links for a document id. |
| `validate_docs_frontmatter.py` | Check required YAML fields, allowed enum values, and same-language `related` targets. |

## Query Documents by Frontmatter

Use `query_docs_frontmatter.py` when the task starts with a broad question such as "where is the testing documentation?" or "which active docs are for agents?"

```powershell
python .\tools\query_docs_frontmatter.py --audience agent --status active --language en
```

Useful filters:

| Filter | Meaning |
|--------|---------|
| `--type technical` | Show only technical documents. |
| `--audience agent` | Show documents intended for agents. |
| `--status active` | Show documents that can be treated as current sources. |
| `--language en` | Show documents in one language. |
| `--description-contains testing` | Search document-level descriptions. |

## Inspect Headings

Use `get_markdown_headers.py` after choosing a candidate document. The tool lists H1 and H2 headings with line ranges and ignores headings inside fenced code blocks.

```powershell
python .\tools\get_markdown_headers.py .\Documentation\en\technical\how-to\How-to-documentation.md
```

Output shape:

```text
Level Header           Range
----- ------           -----
H1    Introduction     [11,25]
H2    YAML Frontmatter [81,126]
```

## Read a Section by Range

Use `read_markdown_section.py` after getting a range from `get_markdown_headers.py`.

```powershell
python .\tools\read_markdown_section.py .\Documentation\en\technical\how-to\How-to-documentation.md "[81,126]"
```

## Follow Related Documents

Use `find_related_docs.py` when a document references another document or when the current document is not enough.

```powershell
python .\tools\find_related_docs.py --id testing --root .\Documentation\en
```

Result types:

| Type | Meaning |
|------|---------|
| `outgoing` | Documents listed by the selected document's `related` field. |
| `incoming` | Documents that list the selected id in their `related` field. |

## Validate Frontmatter

Use `validate_docs_frontmatter.py` after creating or editing documentation. This tool validates every markdown file under the `--root` directory, not only one file.

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation\en
```

Single-folder check:

```powershell
python .\tools\validate_docs_frontmatter.py --root .\Documentation\en\technical\how-to
```

Checks:

- Required fields: `type`, `audience`, `status`, `language`, `description`.
- `type` is `gameplay` or `technical`.
- `audience` uses `player`, `developer`, or `agent`.
- `status` uses `draft`, `active`, or `deprecated`.
- `language` uses `en` or `vi`.
- Optional `related` entries point to a markdown file in the same language.
- Frontmatter starts and ends with `---`.

## Error Handling

| Case | Action |
|------|--------|
| A tool says the file does not exist | Check the path and run from the project root. |
| No documents are returned | Relax filters or inspect `status` and `audience` values. |
| Validation fails | Fix the listed frontmatter field before treating the document as complete. |
| A section range is wrong | Re-run `get_markdown_headers.py` and use the current range. |

## Verification

The documentation reading workflow is correct when:

- `query_docs_frontmatter.py` can find candidate documents by metadata.
- `get_markdown_headers.py` returns H1/H2 ranges for the selected file.
- `read_markdown_section.py` returns only the requested line range.
- `validate_docs_frontmatter.py` reports no issues for newly edited documents.
