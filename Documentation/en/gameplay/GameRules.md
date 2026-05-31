---
type: gameplay
module: game-rules
version: 1.0
audience: user
related: [entity-system, card-system, grid-system, economy-system, game-modes]
---

# Overview

Monkey vs Alien is a lane-based strategy game between two teams: **Monkey** and **Alien**. Each team uses cards to summon Entities onto the battlefield. Entities move and fight automatically, pushing their forces toward the opponent's base. You play as the commander, choosing when and where to deploy each card to gain an advantage.

# Win and Loss Conditions

The team that **destroys 3 enemy Targets first** wins the match. Targets are special Entities located deep in each team's base, and losing 3 of them is the only way to lose — if you lose 3 Targets, the match is over.

Your Targets are your last line of defense, and every strategic decision revolves around protecting yours while finding a way to break through the enemy's.

# The Battlefield

The battlefield is split into multiple **independent lanes** running horizontally across the screen. The maximum number of lanes is 5, depending on the level's configuration.

Each lane is between **9 and 15 cells** long. Entities can only interact with each other if they are on the same lane — allies support each other, enemies attack each other.

When a card is used, the Entity spawns at its team's edge of the selected lane — Left team (Monkey) spawns from the left, Right team (Alien) spawns from the right — and automatically advances toward the enemy.

# Entity

Entities are the fundamental units on the battlefield. Each Entity has the following core attributes:

| Attribute | Description |
|-----------|-------------|
| Lane | Which lane the Entity belongs to |
| Health | Hit points. When this reaches 0, the Entity dies |
| Move Speed | Movement speed |
| Range | Attack range |
| Strength | Damage dealt per hit |
| Attack Speed | Attack speed (hits per second) |

Some Entities also possess special abilities — they can heal allies, buff themselves, deal area damage, or stun enemies.

> **Example:** Basic Monkey is a simple melee Entity with average stats and no special abilities out of the gate. Slimz, on the other hand — a Sneaky-class Entity — has a 15% chance to throw a Poke that stuns an enemy for 3 seconds.

# Resources

Each card requires resources to be played. The two teams handle resources differently:

- **Monkey team**: Owns the **Banana Tree** card (a Tower Entity). It generates bananas at regular intervals, similar to the Sunflower in Plants vs. Zombies.

- **Alien team**: Receives "reinforcements" from off-screen, providing a steady resource income without needing to farm. The initial rate is slower, but Aliens can upgrade to increase production by one tier, similar to Tower Conquest.

# Cards

Each card has two constraints: a **resource cost** and a **cooldown timer**. You can only play a card when you have enough resources and the card is off cooldown.

The decision to play a card depends on both your current resource pool and when the next card will become available. Spend all your resources on an expensive card, and you'll need to wait before playing another.

Cards come in several types, including **Combat Cards** (summon fighting Entities) and **Tower Cards** (summon a Builder who constructs a tower at the chosen location).
