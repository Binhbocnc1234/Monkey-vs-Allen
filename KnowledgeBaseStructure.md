# Knowledge Base Structure

Project map. Each section = one top folder. Describes who reads what and why. This markdown is both for developer and Agent

---

## Document

Location: `Document/`

Two categories, two languages.

| Folder | Content | Audience |
|--------|---------|----------|
| `gameplay/` | Game rules, systems, mechanics, modes. What the game IS. | Player and Creative agent |
| `technical/` | Architecture, AI system, design decisions, migration plans. | Developer and Reasoning agent |

```
Document/
├── vi/
│   ├── gameplay/
│   │   ├── GameRules.md
│   │   ├── EntitySystem.md
│   │   ├── CardSystem.md
│   │   ├── Effect.md
│   │   ├── UpgradeSystem.md
│   │   ├── GameModes.md
│   │   └── Grid.md (reserved)
│   └── technical/         (reserved for future Vietnamese technical docs)
├── en/
│   ├── gameplay/
│   │   ├── GameRules.md
│   │   ├── EntitySystem.md
│   │   ├── CardSystem.md
│   │   ├── Effect.md
│   │   ├── UpgradeSystem.md
│   │   └── GameModes.md
│   └── technical/
│       ├── DesignPrinciples.md
│       ├── Architecture-to-be.md
│       ├── Architecture-as-is.md
│       ├── ExecutionPaths.txt
│       └── SoftwareDevelopmentProcess.md
├── _legacy-docs/
│       (old .docx design docs, kept for reference)
└── OldDocument/
    └── General.md         (legacy, kept for reference)
```

Key rules:
- `gameplay/` = user-facing. No dev jargon. Any player can read.
- `technical/` = developer-facing. Architecture, algorithms, migration.
- Vietnamese first, English mirror. New docs in `vi/` first.
- All docs have YAML frontmatter for AI-parsable metadata (type, module, audience, related).

---

## Documentation Style Guide

Every `.md` file in `Document/` must follow these rules. AI and developers can use this as a single reference instead of inspecting existing files.

### 1. YAML Frontmatter

Every file **must** start with a YAML frontmatter block:

```yaml
---
type: gameplay           # gameplay | technical
module: entity           # snake_case, matches the system name
version: 1.0             # integer or semver
audience: user           # user | developer
related:                 # list of related modules (reference only, not exhaustive)
  - game-rules
  - card-system
---
```

Fields:
| Field | Required | Values |
|-------|----------|--------|
| `type` | yes | `gameplay` (player-facing), `technical` (developer-facing) |
| `module` | yes | snake_case identifier; mirrors the filename concept (e.g. `effect`, `progression`) |
| `version` | yes | integer or semver. Bump on meaningful content changes. |
| `audience` | yes | `user` or `developer`. Matches the folder. |
| `related` | no | array of module names. Only list direct, important crosslinks. |

### 2. Markdown Style

- **Headings**: `#` H1 for top-level sections, `##` H2 for subsections, `###` H3 for sub-subsections if needed. Use headings hierarchically — never skip a level.
- **Bold** for UI labels, key terms, button names.
- `Inline code` for file paths, module names, identifiers (e.g. `EntitySO`, `MvA.Core`).
- Code blocks for examples, diagrams (text), configuration. Always specify language: ```csharp, ```text, ```yaml.
- Lists: dashes `-` not asterisks. One space after `-`.
- Tables for structured comparisons, attribute lists.
- Horizontal rules `---` to separate major sections (but not between every paragraph).
- No HTML unless absolutely necessary (tables with complex cell merges).
- No emojis unless the content explicitly calls for them (e.g. a fun tutorial) — keep gameplay/technical docs neutral.

### 3. Writing Conventions

- **Tone**: neutral, informative. No marketing, no hype, no "you'll love this".
- **Audience-awareness**: `gameplay/` docs never mention `ScriptableObject`, `EntitySO`, `assembly`, or any Unity-internal concept. Describe the game rule, not the implementation. `technical/` docs can use technical terms but should explain acronyms on first use.
- **Brevity**: prefer a short sentence over a long one. Prefer a table over a paragraph.
- **Cross-references**: link to related docs by filename (without extension) in the `related` frontmatter field. In the body, write e.g. `Xem [EntitySystem](EntitySystem.md)` or `See [EntitySystem](../gameplay/EntitySystem.md)`.
- **Vietnamese docs**: author in Vietnamese first. English docs are a mirror — same structure, same headings, same YAML fields. Content is translated, not rewritten.
- **Placeholder docs**: if a doc is reserved but not yet written, keep the file empty (0 lines) — the folder structure alone signals intent.

### 4. File Naming

- PascalCase. One concept = one word or compound word. Examples: `GameRules.md`, `EntitySystem.md`, `UpgradeSystem.md`.
- Avoid abbreviations. `EntitySystem.md` not `EntitySys.md`. Exception: well-known acronyms like `AI`.
- Hyphens only if the name would be unreadable without them (avoid if possible).

---

## Resources

Location: `Assets/Resources/`

(reserved)

---

## Scenes

Location: `Assets/Scenes/`

Battlefield: [Write here]
Lobby: [Write here]

---

## Scripts

Location: `Assets/Scripts/`

(reserved)
