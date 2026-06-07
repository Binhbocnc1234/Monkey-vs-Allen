---
type: gameplay
audience: [player]
status: active
language: en
description: Defines gameplay effects, how effects interact with entities and cards, and their role in combat behavior.
related:
  - entity-system
  - card-system
---

# Definition

Effects are temporary states applied to Entities during battle. They can alter an Entity's stats, behaviour, or capabilities — for better (buff) or worse (debuff).

Unlike **Traits** — innate characteristics that an Entity carries for its entire lifetime — Effects only last for a limited duration and disappear when their time runs out.

# Sources

Effects can originate from several sources:

- **Other Entities' skills** — The most common source. An Entity can apply effects to allies (buffs) or enemies (debuffs) through attacks or skills.
- **Passive skills** — Some Entities automatically trigger effects on themselves or nearby allies when conditions are met.
- **Environment** — Certain levels have battlefield hazards that apply effects to Entities.

When the Entity carrying an effect dies, all effects on it disappear as well.

# Classification

## By nature

- **Buff** — Beneficial effects: stat increases, healing, immunity, etc.
- **Debuff** — Harmful effects: stat reductions, damage over time, paralysis, etc.

Distinguishing buffs from debuffs matters because some Entities have skills that **cleanse debuffs** or **remove effects** from allies.

## By duration

- **Timed effects** — Automatically expire after a set duration. The countdown begins when the effect is applied.
- **Permanent (in-battle) effects** — Last until the Entity dies or the effect is removed by another skill.

## Stacking behaviour

Some effects **stack** — applying them multiple times increases their potency. Others do not stack — reapplying only **resets the duration**.

# Common Effect Types

| Effect | Description |
|--------|-------------|
| **Stun** | Completely paralyzes the Entity — cannot move, cannot attack. Ends after a duration. Example: Slimz has a 15% chance per Poke attack to stun the enemy for 3 seconds. |
| **Slow** | Reduces movement speed. Visual: blue tint on the Entity. |
| **Freeze** | Freezes all actions. The Entity stands still completely, turning blue. |
| **OnFire** | Deals 3 damage every second for a duration. Visual: small flames on the body. |
| **Poisoning** | Deals 2 magic damage every 3 seconds. Visual: flashing green. |
| **Hypnotized** | Converts the Entity to the opposite team — it attacks its former allies. Visual: purple body with floating bubbles. |
| **IronBody** | Significantly reduces incoming damage. |
| **LifeSteal** | Heals a portion of damage dealt. |
| **Fury** | Increases damage dealt by a fixed amount. |
| **Excited** | Increases attack speed by 70%. |
| **Momentum** | Increases movement speed by 70%. |
| **BullEye** | Counters armored Entities — deals bonus damage or ignores armor. |
| **Splash** | Attacks deal damage to multiple targets in an area. |
| **Strikethrough** | Attacks pierce through multiple enemies along their path. |

# Relationships with Other Systems

- **Entity System** — Effects modify an Entity's stats and behaviour.
- **Combat System** — Many effects intervene in the damage flow: amplify outgoing damage, reduce incoming damage, or reflect damage back.
- **Card System** — Entities summoned from cards carry their own set of potential effects, and different cards offer different effect capabilities.