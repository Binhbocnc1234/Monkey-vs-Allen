# Agent Instructions: Monkey vs Allen

## Project Overview
- Unity 2D project using modular `.asmdef` / `.csproj` assemblies.
- **Ignore entirely**: `Monkey vs Allen project/` (old project), `Raw assets/`, `Releases/`.
- **Project Structure**: Read `KnowledgeBaseStructure.md` first to understand where things live.
- **Code Style**: Focus on readability. Avoid overly complex C# syntax where simpler is clearer.

## Development & Testing Workflow
The architecture separates pure C# logic (e.g., `IEntity`, `IBehaviour`) from Unity-specific wrappers (e.g., `EntityWrapper`). Testing is tiered to avoid Unity's slow editor overhead:

1. **Tier 1 (Compile Check) - Execute this frequently!**
   - You can quickly verify C# syntax and types without starting Unity.
   - Command: `dotnet build <AssemblyName>.csproj` (e.g., `dotnet build MvA.Core.csproj`, `dotnet build MvA.Entities.csproj`)
   - **Agent Rule**: Run this compile check immediately after modifying C# code.
2. **Tier 2 (EditMode) & Tier 3 (PlayMode)**
   - Tests are in `Assets/Tests/`.
   - Require Unity Editor. Only run PlayMode tests for features that inherently require frame lifecycle (`Update`, Physics).
- If tests fail with "undefined reference", verify that the corresponding `.asmdef` (like `Tests.asmdef`) includes the required assembly reference.

## Documentation Rules
Documentation lives in `Document/` (subdivided into `vi/` and `en/`) and strictly follows a YAML schema.
Every `.md` file created or updated in `Document/` MUST include this YAML frontmatter:

```yaml
---
type: gameplay | technical
module: <snake_case_name>
version: <1.0>
audience: user | developer
related: [<related-modules>]
---
```
*(For complete rules, see `Document/en/technical/how-to/How-to-documentation.md`)*
