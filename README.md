# Knowledge Base Structure

Project map. Each section = one top folder. Describes who reads what and why. This markdown is both for developer and Agent

---
## Editor version
6000.3.11f1

## Document

Location: `Document/`

Two categories, two languages.

| Folder | Content | Audience |
|--------|---------|----------|
| `gameplay/` | Game rules, systems, mechanics, modes. What the game IS. | Player and Creative agent |
| `technical/` | Architecture, AI system, design decisions, migration plans. | Developer and Reasoning agent |
| `technical/how-to/` | Step-by-step guides for developers (testing, documentation, workflows) | Developer |

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
│   └── technical/
│       └── how-to/
│           └── How-to-documentation.md
├── en/
│   ├── gameplay/
│   │   ├── GameRules.md
│   │   ├── EntitySystem.md
│   │   ├── CardSystem.md
│   │   ├── Effect.md
│   │   ├── UpgradeSystem.md
│   │   └── GameModes.md
│   └── technical/
        ├── How-to-test.md
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
- `technical/how-to/` = step-by-step guides for developers (testing, documentation, workflows).
- Vietnamese first, English mirror. New docs in `vi/` first.
- All docs have YAML frontmatter for AI-parsable metadata (type, module, audience, related).
- How-to docs use `type: technical`, `audience: developer` in YAML frontmatter, plus `module: how_to_<subject>` (snake_case).

---

## Documentation Style Guide

Every `.md` file in `Document/` must follow these rules. AI and developers can use this as a single reference instead of inspecting existing files.

### 1. YAML Frontmatter

Every file **must** start with a YAML frontmatter block:

```yaml
---
type: gameplay           # gameplay | technical
module: entity           # snake_case, matches the system name
             # integer or semver
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

### 2. Markdown Style, Writing Conventions, File Naming

See [How-to-documentation](vi/technical/how-to/How-to-documentation.md) for Markdown style, writing conventions, and file naming rules.

---

## Resources

Location: `Assets/Resources/`

(reserved)

---

## Scenes

Location: `Assets/Scenes/`

Battlefield: [Write here]
Lobby: [Write here]
...
---

## Scripts

Location: `Assets/Scripts/`

Read `Architecture-as-is.md`

## Tests
