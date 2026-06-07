---
type: technical

audience: [agent]
status: active
language: en
description: Explains how agents should modify project data while respecting architecture, design principles, and validation workflow.
related:
  - architecture-as-is
  - design-principles
  - Testing-workflow
---

# How to Modify Data

This guide explains how an AI agent should edit Unity data safely and predictably in this project. In practice, “data” usually means one of three things:

- A prefab asset in the Project window
- A prefab instance in a scene
- A regular GameObject inside an open scene

The goal is to make these edits easy for a future agent without breaking prefab links, scene references, or undo behavior.

---

## What To Edit

Choose the smallest editable target that actually owns the data.

| Target | Use when | Preferred API surface |
|---|---|---|
| Prefab asset | The change should affect every instance | `PrefabUtility.EditPrefabContentsScope` or `LoadPrefabContents` |
| Prefab instance | The change is specific to one placed instance | Modify the instance, then `PrefabUtility.ApplyPrefabInstance` |
| Scene GameObject | The object is not part of a prefab source | Edit the object directly in the scene |

Do not edit a child inside a prefab instance as if it were a plain scene object. If the object belongs to a prefab source, first identify whether you want to change the source asset or only an override on one instance.

---

## Preferred Workflow For Prefab Assets

When the intention is to modify the prefab itself, use the prefab asset workflow.

1. Resolve the prefab asset path.
2. Load the prefab contents into an isolated editing context.
3. Make the change on the loaded root GameObject.
4. Save the prefab asset.
5. Unload the temporary contents.

The safest shape for this is `EditPrefabContentsScope`, because it wraps load, save, and unload in one disposable scope.

### Typical use cases

- Add or remove components from a prefab
- Reparent or delete child objects inside the prefab hierarchy
- Update serialized fields on components in the prefab
- Batch edit many prefabs in one editor command

### Why this is the preferred path

This avoids editing a live scene object and then trying to reconstruct the prefab source afterward. It also keeps the operation deterministic for automation, which is important if an agent must run the same edit on many assets.

---

## Preferred Workflow For Prefab Instances

When the intention is to change only one placed instance, edit the instance in the scene and then apply the override if needed.

1. Find the prefab instance root in the scene.
2. Change the component, field, or child object on that instance.
3. If the change should become part of the source prefab, call `PrefabUtility.ApplyPrefabInstance`.
4. If the change should remain local, keep it as an override.

### Important rule

Use this path only when the user explicitly wants an instance-level override or when the scene placement itself is the real source of truth.

### Caveat

Unity does not treat every override equally. Some root transform values and nested prefab behaviors have restrictions, so a future agent should prefer source editing when the goal is structural change.

---

## Scene GameObject Edits

If the object is a normal scene object and not part of a prefab source, edit it directly.

Common operations:

- Add or remove components
- Rename objects
- Move objects in the hierarchy
- Change serialized fields on components
- Create or delete child GameObjects

Use the Undo system when the edit happens through the editor so the user can revert the change naturally.

---

## Safety Rules For Agents

An AI agent editing Unity data should follow these rules:

- Prefer the prefab asset over a scene instance when the change is meant to affect all copies.
- Prefer the instance only when the change is local to that one scene placement.
- Never assume a scene object is safe to edit without checking whether it belongs to a prefab instance.
- Preserve object names and hierarchy order when possible, because Unity uses them when saving over an existing prefab.
- Keep edits small and explicit. A broad hierarchy rewrite is riskier than a targeted field change.
- After editing prefab contents, always save and unload the temporary contents.
- For scene edits, prefer editor-time changes over runtime hacks.

---

## Recommended Implementation Shape

For this project, the most practical editor tool should expose three actions:

1. Edit selected prefab asset
2. Edit selected prefab instance
3. Edit selected scene GameObject

That gives the agent one consistent entry point while still routing the operation to the correct Unity API.

### Suggested behavior

- If the selection is a prefab asset, open it in a prefab editing scope and apply changes there.
- If the selection is a prefab instance, inspect whether the request is source-level or instance-level before editing.
- If the selection is a plain GameObject, edit it directly and record Undo.

### Suggested validation

After each change, verify:

- The prefab still opens correctly
- The hierarchy still matches the intended structure
- The serialized values persist after reload
- No unintended overrides were created on scene instances

---

## Good Mental Model

Use this decision order:

1. Is this a prefab asset, a prefab instance, or a plain scene object?
2. Should the change affect one object or every copy?
3. Does the change belong in the source asset or as a local override?
4. Which Unity API keeps the edit isolated and reversible?

If the answer points to the prefab source, edit the asset. If it points to a one-off scene placement, edit the instance. If it is a normal scene object, edit the scene object directly.

---

## Short Example

If the request is “add a BoxCollider to every prefab in a folder”, the agent should:

1. Find each prefab asset path.
2. Load the prefab contents.
3. Add the component to the root or selected child.
4. Save the prefab asset.
5. Unload the prefab contents.

If the request is “add a debug label to the selected enemy in this scene only”, the agent should:

1. Edit the scene instance.
2. Add the label component or field value.
3. Leave it as a local override unless the user asks to push it back to the prefab.

---

## Summary

Prefab asset edits should go through isolated prefab contents. Prefab instance edits should go through overrides and apply only when the user wants to update the source. Plain scene objects can be edited directly. This split keeps automated Unity edits safe, predictable, and easy to repeat.

---

## Notes From Unity Prefab Migration

During the MonoBehaviour -> plain C# migration, prefabs can keep broken component entries after the script class disappears. Unity will show these as missing scripts in the prefab inspector, and the prefab asset may still contain stale serialized component slots until it is cleaned up.

For this project, the correct repair flow is:

1. Open the prefab asset itself, not a scene instance.
2. Remove the obsolete MonoBehaviour components from the prefab hierarchy.
3. Move any gameplay setup data into the matching `EntitySO` asset, especially `behaviourTemplates`.
4. Recreate the runtime object graph from data when the prefab is instantiated.

Important details learned from Unity:

- If a behaviour used to live on the prefab as a component, its replacement should usually be a plain C# object stored in `EntitySO.behaviourTemplates`.
- `behaviourTemplates` is serialized by reference, so each template entry must be a concrete behaviour instance type that Unity can persist.
- If a prefab no longer needs a runtime component, deleting the missing script component is the correct fix; leaving it in place keeps the prefab corrupted.
- Use prefab asset editing, not live scene edits, when cleaning these assets so the fix applies to every instance.

For automation, the safest editor-side tool shape is:

- Find the relevant `EntitySO` asset.
- Load its assigned prefab asset with prefab contents editing.
- Strip missing script components from the prefab root and children.
- Copy the old setup intent into `EntitySO.behaviourTemplates` or related data fields.
- Save and unload the prefab contents.

This keeps prefab data and runtime behaviour aligned after the refactor.