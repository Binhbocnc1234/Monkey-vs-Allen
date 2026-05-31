---
type: gameplay
module: card
version: 1.0
audience: user
related: [game-rules, entity-system, economy-system, progression-system]
---

# What is a Card?

Each card represents an Entity you can summon onto the battlefield. If Entities are the fighters on the front lines, cards are the "deployment orders" — you need the card in hand and enough resources to use it.

Each card is linked to an **EntitySO** — a data file containing all the information about the Entity it summons, including its stats, skills, and appearance.

# Card Types

There are currently two main card types:

## Combat Card
This is the most common type. When used, it summons a combat Entity at your team's lane edge. The Entity immediately starts moving and joins the fight.

- **MonkeyCardSO** — For the Monkey team
- **EnemyCardSO** — For the Alien team

## Tower Card
Unlike Combat Cards which spawn Entities at the lane edge, a Tower Card summons a **Builder** — a special Entity dedicated to construction. The Builder moves to the selected position on the lane and begins building. After a certain construction time, the Tower Entity is complete and starts functioning.

Tower Cards can only be placed on **non-edge cells** (not the first or last cell of the lane), and the target cell **must not already have a tower**.

> **Example:** The **Banana Tree** — a Monkey Tower Card. When you place it, a Builder appears, moves to the target position, builds for a few seconds, and then the tree starts producing resources.

# How to Own Cards

There are several ways to own a card:

- **Starting card** — You are provided with the Basic Monkey card from the very first level.
- **Match rewards** — Clear levels to earn new cards.
- **Shard collection** — Each card has corresponding shards. Once you collect enough, you can exchange them for **permanent ownership** of that card.

After owning a card, you can keep collecting its shards to fuel upgrades.

## Rarity

Cards come in different rarity tiers: **Common, Occasional, Rare, Epic, Exotic, Legendary**. Rarity does **not** affect the card's power or the number of shards needed for upgrades — instead, it affects the **drop chance** of its shards. The rarer the card, the harder its shards are to find.

# How to Use Cards in Battle

During a match, each card exists as a **Battle Card** — the live instance of the card with two constraints:

1. **Cost** — The amount of resources you must pay to use the card. Stronger cards cost more.
2. **Cooldown** — After using a card, you must wait for a timer before you can use it again.

The card usage flow in battle:

1. **Select** — Choose a card from your hand.
2. **Check conditions** — The game checks three things:
   - Do you have enough resources? (If not → "Insufficient Resource")
   - Is the card off cooldown? (If not → "Recovering")
   - Is the target position valid? (Especially for Tower Cards)
3. **Choose lane** — Pick the lane to deploy the Entity.
4. **Summon** — The Entity appears, resources are deducted, and cooldown begins.

Additionally, Enemy Cards have a special mechanic: **max stack**. Instead of being usable once per cooldown, they can accumulate multiple charges over time, allowing the Alien team to spam the same type of Entity in quick succession.

# Card Upgrades

Each card can be upgraded up to **5 times**, with each level providing a different improvement:

1. **Stat upgrade** — Boosts a specific stat (MaxHealth, Armor, MagicResistance, MoveSpeed, AttackSpeed, etc.)
2. **New skill** — Unlocks a new active or passive skill
3. **Stat upgrade** (again)
4. **Skill upgrade** — Enhances an existing skill or passive
5. **Temporary upgrade** — The 5th upgrade is active for a limited duration

This sequence may vary between cards, but most follow the pattern above.

## Example: Basic Monkey

| Upgrade | Effect | Shard Cost |
|:---:|:---|:---:|
| 1 | **Stat**: +5 Strength | 10 |
| 2 | **New Skill**: Monkey Solidarity — For each Basic Monkey on the battlefield, gain 8% Armor | 15 |
| 3 | **Stat**: +40 MaxHealth | 20 |
| 4 | **Skill Upgrade**: [ArmorIncreasedAmount] +4% | 25 |
| 5 | **Temporary**: 10% chance to spawn an Elite unit | 20 |

## Example: Basic Alien

| Upgrade | Effect | Shard Cost |
|:---:|:---|:---:|
| 1 | **Stat**: +5 Strength | 10 |
| 2 | **New Skill**: Advanced — 15% chance to equip a Cranium Helmet | 15 |
| 3 | **Stat**: +40 MaxHealth | 20 |
| 4 | **Skill Upgrade**: [Chance] +8% | 25 |
| 5 | **Temporary**: 10% chance to spawn an Elite unit | 20 |

# Relationships with Other Systems

- **Entity System** — Each card references an EntitySO, determining which Entity appears when used.
- **Economy System** — Card cost consumes your team's resources.
- **Grid System** — Tower Cards validate grid positions before the Builder begins construction.
- **Progression System** — Shards are used for card ownership and upgrades, tied to the player's progression.
