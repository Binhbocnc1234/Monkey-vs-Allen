---
type: technical

audience: [developer, agent]
status: active
language: en
description: Defines the documentation format, tone, completeness rules, and YAML frontmatter requirements for project how-to documents.
---

# Introduction

How-to documents provide step-by-step instructions for completing a technical task in the project.

**Purpose:**
- Guide developers through a specific task such as running tests, creating a branch, or writing documentation
- Reduce trial-and-error and remove guesswork
- Keep the team aligned on a shared process

**Audience:**
- Agent - VIP audience, the document must be readable by an agent
- Developer - someone with a working knowledge of Unity and C#

---

# Consistency

All how-to docs must follow the rules below to avoid conflicts and duplication.

### 1.1 Do not duplicate content

Two documents must not describe the same logic or workflow. When cross-referencing is needed, use `See [file name](path)` or `Xem [file name](path)`.

**Example:**

```
❌ Wrong:
File A: "Entity starts with 100 HP"
File B: "Entity begins with 100 health"

✅ Right:
Only one file states "Entity initializes with 100 HP"
The other file references it: See [EntitySystem](../gameplay/EntitySystem.md)
```

### 1.2 Neutral and consistent tone

Keep the tone neutral, technical, and non-promotional. Use imperative mood for instructional steps.

**Example:**

```
❌ Wrong:
You will want to open Unity Editor first, then you go into Test Runner...

✅ Right:
Open Unity Editor. Go to Window > General > Test Runner.
```

### 1.3 Naming and heading rules

- Heading and file name rules are defined in [Format and conventions](#format-and-conventions).
- Frontmatter no longer uses a `module` field; use file names for document identity and `related` references.

**Example:**

```markdown
# How to Run EditMode Tests   ← H1
## Requirements               ← H2
### Required Tools            ← H3
```

---

# Format and Conventions

## Language

Default to English and write only in English. Write Vietnamese only when the user asks for it.

## YAML Frontmatter

Every official markdown file in `Documentation/` must start with YAML frontmatter. The YAML is primarily for agents, so it must be accurate for an agent to decide whether the document is relevant before reading the full content.

**Required format:**

```yaml
---
type: gameplay | technical
audience: player | developer | agent
status: draft | active | deprecated
language: en | vi
description: <one sentence covering the entire document>
---
```

**Required fields:**

| Field | Rule | Why |
|-------|------|-----|
| `type` | Use `gameplay` for player-facing game rules or `technical` for implementation and workflow documentation. | Lets agents filter by document category before reading content. |
| `audience` | Use only `player`, `developer`, or `agent`. For multiple audiences, use a YAML list, such as `[developer, agent]`. | Lets agents prefer documents written for the current task role. |
| `status` | Use `active` by default. Use `draft` for incomplete documents and `deprecated` for documents that should not be the primary source. | Helps agents avoid treating draft or deprecated documents as authoritative sources. |
| `language` | Use `en` for English documents or `vi` for Vietnamese documents. | Lets agents query the correct language and validate `related` links within the same language. |
| `description` | Write one sentence that covers the entire document, not only the first section. | Lets agents decide whether the document is relevant before reading the full content. |

**Audience values:**

- `player`: gameplay documentation for players or player-facing design.
- `developer`: technical documentation for developers working on the Unity project.
- `agent`: documentation optimized for automated agents to read, decide, and execute.

**Status values:**

- `active`: default value for complete documents that can be used as a source.
- `draft`: incomplete document that should not be treated as authoritative.
- `deprecated`: outdated document kept for history or migration context.

**Language values:**

- `en`: English document under `Documentation/en/`.
- `vi`: Vietnamese document under `Documentation/vi/`.

**Optional fields:**

| Field | Format | Reason |
|-------|--------|--------|
| `related` | `[<related-documents>]` | Lets agents discover connected documents when the current document does not contain enough context. Each item must match a markdown file name in the same language. |
| `updated` | `YYYY-MM-DD` | Helps agents compare freshness when multiple documents conflict. |

Do not add `owner`, `tags`, or `priority` unless the project has a clear maintenance process for those fields. Otherwise, they create metadata noise.

## Markdown Style

- Use the correct heading levels: `#` for H1, `##` for H2, `###` for H3; do not skip levels.
- Use `**bold**` for button names, UI labels, or emphasis.
- Use `inline code` for paths, file names, modules, and identifiers.
- Use fenced code blocks for commands, configuration, or long examples.
- Use lists, tables, and horizontal rules when they improve readability.
- Do not use HTML or emoji.

**Example heading hierarchy:**

```markdown
# Writing Documentation
## Markdown Style
### Heading Hierarchy
```

## Writing Conventions

- Keep the tone neutral, informative, and non-promotional.
  - Example: write `Open Unity Editor.` instead of `You may want to open Unity Editor first.`
- Technical terms are fine when the document is for developers.
  - Example: `Run EditMode tests` instead of a long plain-language explanation.
- Keep sentences short and prefer imperative verbs.
  - Example: `Click Run All.` instead of `Then you should click Run All to start everything.`
- Cross-references must be explicit and use links in the correct context.
  - Example: `See [How-to-test](How-to-test.md)`.
- Vietnamese documents are the primary source; English versions are mirrors when required.
  - Example: create `vi/technical/how-to/How-to-documentation.md` first, then create the English counterpart if needed.
- If a file is reserved, keep the content empty or only note the reserved status.
  - Example: `Grid.md (reserved)` should not contain an incomplete description.

## File Naming

- Use PascalCase for markdown file names.
  - Example: `How-to-documentation.md`, `How-to-test.md`.
---

# Completeness

A how-to doc is considered **complete** when it contains all five parts below:

### 2.1 Preconditions

Describe the environment, tools, and state required before starting. The reader must satisfy these conditions before following the doc.

**Example:**

```markdown
**Preconditions:**
- The project is open in Unity Editor
- Test Runner is open (Window > General > Test Runner)
- At least one test file exists in Tests/
```

### 2.2 Step list

Use a numbered list, with one concrete action per step. Use imperative verbs.

**Example:**

```markdown
1. Select the EditMode tab in Test Runner
2. Click Run All
3. Review the displayed results
```

### 2.3 Expected results

Describe what should happen after each step or after the full procedure.

**Example:**

```markdown
**Expected results:**
- All tests pass (shown in green)
- No tests fail or are skipped
- The progress bar reaches 100%
```

### 2.4 Edge cases

Describe how to handle errors, exceptions, or unexpected cases.

**Example:**

```markdown
**Edge cases:**
- **Test fails:** Check the Console log for the error. Fix the code and run again.
- **Test Runner is missing:** Open Window > General > Test Runner.
- **Project does not compile:** Fix compile errors first, then run tests.
```

### 2.5 Verification

Explain how to know the task is done correctly, either automatically or manually.

**Example:**

```markdown
**Verification:**
- The message "All tests passed" appears
- The number of passed tests equals the total number of tests in the suite
```

---

# Structure and Tone

### Recommended order

```
Introduction → Preconditions → Steps → Expected results → Error handling
```

### Writing rules

- Keep it **short** and imperative.
- Avoid long narrative passages.
- Use **tables**, **numbered lists**, and **code blocks** for commands.
- Use **bold** for UI labels and button names.
- Use `inline code` for file paths, module names, and identifiers.

**Complete example:**

```markdown
## How to Run EditMode Tests

**Preconditions:**
- Unity Editor is open
- Test Runner is open (Window > General > Test Runner)

**Steps:**
1. Select the EditMode tab
2. Click Run All
3. Review the results

**Expected results:**
- Tests complete in about 15 seconds
- All tests pass (green)

**Error handling:**
- If a test fails, check the Console log, fix the issue, and run again
- If Test Runner does not open, restart Unity

**Verification:**
- The message "All tests passed" appears
```

### Good vs Bad

| Bad | Good |
|-----------|-----|
| "First you need to open Unity Editor" | "Open Unity Editor" |
| "Then you should click Run All" | "Click Run All" |
| "If everything is fine, you'll see the tests pass" | "Result: all tests pass" |
