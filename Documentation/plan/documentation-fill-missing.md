# Plan: Complete missing documentation

Create a short, high-level plan to finish the missing documentation in `Document/vi/technical/how-to/How-to-documentation.md` and update `KnowledgeBaseStructure.md`, focusing on feasibility, correctness, consistency, rationale, and workload.

**Steps**
1. **Review requirements and style rules**: Use `KnowledgeBaseStructure.md` as the source of truth for documentation structure, YAML frontmatter requirements, tone, headings, and cross-reference rules. Confirm Vietnamese-first policy and placeholder conventions.
2. **Gap analysis (missing content)**: Compare `How-to-documentation.md` against the style guide to identify missing YAML frontmatter, missing required sections (e.g., completeness section body), and any structural inconsistencies (headings, tone, examples). Note any missing references in `KnowledgeBaseStructure.md` (e.g., missing technical how-to folder listing or rules) that should be documented to keep the project map accurate.
3. **Define additions with specific outlines**:
    - **`Document/vi/technical/how-to/How-to-documentation.md` outline (add/complete sections):**
      1. **Frontmatter (YAML)**: `type: technical`, `module: how_to_documentation` (snake_case), `version`, `audience: developer`, `related:` (list of linked docs if any).
      2. **Giới thiệu/Mục tiêu**: what “how-to documentation” is in this project, its purpose (practical, step-by-step guidance), and target readers.
      3. **Quy tắc nhất quán**: required terminology, heading levels, naming conventions, cross-reference style, and consistent Vietnamese tone. **Kèm ví dụ cụ thể.**
      4. **Tiêu chí “Tính đầy đủ”**: what must be present to consider a how-to complete (preconditions, step list, expected results, edge cases, validation/verification notes). **Kèm ví dụ cụ thể.**
      5. **Cấu trúc & giọng điệu**: recommended section order (intro → prerequisites → steps → results/verification → troubleshooting), concise imperative tone, avoid prose-heavy narrative. **Kèm ví dụ cụ thể.**
      6. **Ví dụ**: short, representative example snippet showing compliant structure (not full doc), highlighting how to apply rules.
    - **Each abstract theory/section must include a concrete example** — mọi quy tắc trừu tượng đều phải có ví dụ thực tế đi kèm.
   - **`KnowledgeBaseStructure.md` outline (changes to map/rules):**
     1. **Structure map update**: insert `Document/vi/technical/how-to/` under technical docs hierarchy, near other technical subfolders.
     2. **How-to mention**: brief description that how-to docs live in the above folder and are step-by-step guidance for developers.
     3. **Rule clarification (if missing/ambiguous)**: explicitly mention required frontmatter for technical how-to docs and adherence to consistent structure/tone rules (avoid duplicating existing rules, only clarify gaps).
4. **Update KnowledgeBaseStructure**: Ensure `Document/vi/technical/how-to/` is reflected in the project map if missing; document how-to as part of technical docs and confirm YAML frontmatter rule is already stated. Only add structure or notes necessary for accuracy and consistency.
5. **Verification**: Re-check both files for: valid YAML frontmatter, consistent headings, neutral tone, and no overlapping/duplicative descriptions. Confirm alignment with rules in `KnowledgeBaseStructure.md` and that the scope is limited to documentation completeness.

**Relevant files**
- `C:\Users\binhb\Documents\_Unity Projects\Monkey vs Allen\Document\vi\technical\how-to\How-to-documentation.md` — add YAML frontmatter, fill missing sections, and align structure/tone with the style guide.
- `C:\Users\binhb\Documents\_Unity Projects\Monkey vs Allen\KnowledgeBaseStructure.md` — update project map or notes to include/clarify the technical how-to documentation location and rules consistency.

**Workload estimate**
- Files affected: 2
- Likely sections: 1 frontmatter block + 5–6 content sections in `How-to-documentation.md`; 1 structure-map insertion + 1–2 short rule/description lines in `KnowledgeBaseStructure.md`.

**Verification**
1. Read both files to confirm YAML frontmatter exists and matches required fields (type/module/version/audience/related).
2. Validate headings are hierarchical and Vietnamese tone is neutral and concise.
3. Confirm no duplicated logic across sections and that the how-to doc explicitly defines completeness and consistency criteria.
4. Confirm every abstract theory/rule has a concrete example accompanying it.

**Decisions**
- Scope limited to filling missing documentation content and structure; no gameplay or code changes.
- Follow Vietnamese-first rule; do not create or update English mirror unless explicitly requested.
