---
type: gameplay

audience: [player]
status: active
language: en
description: Defines gameplay entities, their stats, ownership, combat role, and relationships with cards and effects.
related:
  - game-rules
  - card-system
  - effect
---

# What is an Entity?

Entities are the units that appear on the battlefield. Anything present in a match — from a Basic Monkey, a Banana Tree, to your base Target — is an Entity.

Entities are summoned through cards. When you play a card, the corresponding Entity appears on the battlefield and starts acting according to its nature.

# Entity Lifecycle

Every Entity goes through a common lifecycle:

1. **Spawn** — The Entity appears on the battlefield, usually at its team's edge of the lane.
2. **Act** — The Entity moves, finds enemies, attacks, or uses skills depending on its type.
3. **Death** — When Health reaches 0, the Entity dies and disappears.

Throughout its life, the Entity constantly interacts with its surroundings — allies support each other, enemies deal damage, and temporary Effects can alter its stats or behaviour.

# Core Stats

Each Entity has a set of stats that govern its combat effectiveness:

| Stat | Description |
|------|-------------|
| Health | Current hit points |
| MaxHealth | Maximum hit points |
| Strength | Damage dealt per attack |
| Range | Attack range (in cells) |
| AttackSpeed | Attack speed (hits per second) |
| MoveSpeed | Movement speed (cells per second) |
| Armor | Physical armor, reduces incoming physical damage |
| MagicResistance | Magic resistance, reduces incoming magic damage |
| LifeSteal | Life steal (percentage of damage dealt) |

Some Entities also have extended stats like CriticalChance, Penetration, MagicPower, and other specialized attributes.

# Class and Tribe

Each Entity belongs to a **class** and can carry one or more **tribes**. The class doesn't affect entity power in battle

Available classes:
- **Hearty** — Defense and survivability specialists
- **Crazy** — High damage but fragile
- **Brainy** — Strategy and effects
- **Swarmy** — Numbers over individual strength
- **Sneaky** — Mobile, surprising, often carries special effects

Tribes are smaller groupings that enable interactions with various effects and skills. Examples: Basic, Target, Pet, Mechanic, Gargantuar, Cosmic, Medieval, Plant, Fruit, etc. An Entity can belong to multiple tribes at once — for instance, an Entity could be both Mechanic and Medieval.

# Behaviour

Entities are never idle — they always perform a **behaviour** at any given moment. Behaviour determines what the Entity is doing: moving, attacking, building, or standing by.

Movement behaviour types:
- **StraightMove** — Walks straight toward the enemy
- **HoppingMove** — Hopping forward
- **BehindAlliesMove** — Stays behind allies, suited for ranged attackers and healers

Attack behaviour types:
- **Melee** — Close combat, the Entity approaches and strikes the nearest enemy
- **Ranged** — Fires projectiles from a distance
- **LobAttack** — Fires arcing projectiles that can bypass obstacles
- **StrikeThrough** — Attacks that pierce through multiple enemies
- **AreaDamage** — Area-of-effect damage within a zone

> **Example:** Basic Monkey uses StraightMove and Melee — it walks forward and attacks enemies face to face. Slimz uses Ranged — it shoots from a distance, and occasionally throws a special Poke attack.

# Entity-to-Entity Interaction

Entities on the same lane belonging to the same team support each other — ranged fighters stay behind, melee fighters push forward. Entities from opposing teams automatically seek and attack enemies within range.

When an Entity attacks an enemy, the damage flow proceeds as follows:

1. The attacker produces a **strike** (melee hit or projectile)
2. The strike hits the target → creates a **DamageContext** containing information about the damage type, amount, and source
3. Effects on both the attacker and the target can intervene — amplifying, reducing, or transforming the damage
4. The final damage is applied to the target's Health

# Skills

Many Entities possess **skills**, which can be **active** (the Entity consciously uses them) or **passive** (they trigger automatically). Each skill has its own set of stats defined in its ScriptableObject.

> **Example:** Slimz has a passive skill granting a 15% chance per attack to become a Poke. When Poke hits, it deals damage and applies **Stun** for 3 seconds (completely paralyzing the target). At level 3, Poke also applies **SlimzSting** — additional damage over time.

# Relationships with Other Systems

- **Card System** — Cards are the "summoning tickets" for Entities. Each CardSO references an EntitySO, determining what type of Entity appears.
- **Effect System** — Effects can temporarily alter an Entity's stats and behaviour.
- **Combat System** — Entities deal and receive damage through strikes, projectiles, and skills.
- **Grid System** — Each Entity occupies a cell on the grid and moves through cells.