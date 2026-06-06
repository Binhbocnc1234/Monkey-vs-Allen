---
type: gameplay
module: game-modes

audience: [player]
status: active
language: en
description: Defines available game modes, their win conditions, rewards, and relationships with cards and progression.
related:
  - game-rules
  - card-system
  - upgrade-system
---

# Campaign

Campaign is the primary game mode, where players progress through the story and unlock new content. Each location (Place) in the Campaign contains 16 levels. You must complete the first 8 levels of the current Place to unlock the next one.

## Campaign — First Playthrough

The first playthrough focuses on acquiring core cards and introducing new Alien types. If a card can hard-counter a specific Alien, you receive that card after defeating that Alien.

During the card selection phase, you can see which Aliens will appear in the upcoming match — the Alien side has pre-selected its "deck". This gives you the advantage of picking counter cards.

**Reward:** Iron trophy.

## Campaign — Second Playthrough

Inherits all content from the first playthrough with increased difficulty. Fewer new Monkey and Alien types appear compared to the first playthrough.

**Reward:** Gold trophy.

## Campaign — Third Playthrough

Inherits the difficulty of the second playthrough, with one major change: **the Alien team can now select cards**, with cost and cooldown mechanics identical to the Monkey side. Few new cards appear — matches are purely about strategy and counterplay.

Card selection is turn-based: Alien picks first and also picks last, preventing the player from holding an entirely countering deck.

**Reward:** Diamond trophy.

## Places

#### Lovely House

- **Appears**: Level 1–10
- **Story**: The monkeys live in this house.
- **Obtainable Monkeys:** Basic Monkey (Fighter, lv0), Banana Tree (Economy, lv1), Slingshot master (DPS, lv2), Boxer Monkey (DPS, lv4), Master Chef (Healing, lv6), Watermelon seller (AOE damage, lv8)
- **Aliens encountered:** Basic Alien (Tanker, lv1), Cranium-helmet Alien (Tanker, lv2), Space cadet (DPS, lv3), Boxer Alien (DPS, lv4), Flag Alien (Speed buff, lv6), Newspaper Alien (DPS, lv8)

#### Primal Breach

- **Appears**: Level 11–20
- **Story**: Monkey forces raid the enemy's food storage. Near the storage stands an ancient tree — a perfect spot for a temporary Monkey base.

Levels 1–10 are introductory, with no special mechanics. From level 51 onward and non-Campaign levels, each side has a unique mechanic:

- **Monkey side:** A monkey descends from the Command Tree. This monkey loves bananas — after eating, it throws the peel at Aliens, stunning them for 3 seconds.
- **Alien side:** A sniper perches atop the food storage, covering all 5 lanes. Having stayed awake on watch duty for too long, it falls into a "half-asleep, half-awake" state — every 45 seconds it alternates between sleeping and waking.

This is the only map without a day/night cycle.

- **Obtainable Monkeys:** Basic Monkey, Banana Tree, Naughty kid, Slimz
- **Aliens encountered:** Basic Alien, Space pistol, Boxer, Cranium-helmet Alien

#### Rampur Village

- **Appears**: Level 11–20
- **Story**: The Monkeys celebrate after raiding the Alien food storage. The Alien captain panics upon discovering the storage has been destroyed. In desperation, they spot the Monkeys' footprints and follow them to the village.
- **Level 10**: After victory, the Alien says "it's too late" and assembles a fire cannon, burning down the entire village. The Monkeys flee to their last refuge: Bodiam Castle.
- **Special mechanic:** Buffalo spawn alternately for both teams. Buffalo have high health and their basic attacks knock enemies back.
- **Obtainable Monkeys:** Farmer, Crane rider, Ronin

#### Bodiam Castle

- **Appears**: Level 21–30
- **Story**: Continues from the Rampur Village incident.
- **Special mechanic:** Because Aliens in these maps are quite strong, several bodyguards (Holy Order) are present from the start — they do not attack, only patrol near their post. The Monkey side can buy up to 2 Garrison Archers who fire from the castle walls.
- **Expected strategy:** Defend, accumulate bananas, then launch a full assault.
- **Collectible cards:** Crossbow tower, Mortal, Steel knight, Holy Order

#### Villa

- **Appears**: Level 31–40
- **Story**: Summer has arrived. The Aliens stop working after a busy year — they gather at the villa to party and forget their guard duties. The Monkeys seize the opportunity to attack.
- **Special mechanic:** The battlefield has 6 lanes instead of 5, with a swimming pool in the middle occupying 2 lanes.
  - **Alien side:** No Aliens can swim. Every 30 seconds, a UFO drops a Ferry Alien — it floats on water and can carry 3 other Aliens. Once full, it departs. The Ferry is not an Entity and cannot be attacked. When encountering Monkeys, it stops and waits for its passengers to finish fighting. The Ferry disappears after a while if all its passengers are killed. Large Aliens cannot board the Ferry (shows "This Alien is too bulky to stand on the ferry").
  - At the start, some Basic Aliens are still playing or sunbathing — they only notice the attack when hit. This is a lore detail with minimal tactical impact.
  - **Monkey side:** Some Entities have the Amphibious trait, allowing them to operate in water.

#### Formusa Factory

- **Appears**: Level 41–60
- **Story**: The Monkeys find a map in the villa leading to Formusa Factory — the massive production facility supplying modern equipment to the Aliens. When Daimonas realizes the factory has fallen, he orders it blown up, destroying the entire blue crystal supply. The Monkeys must find an alternative blue crystal source.

#### Crysty Cave

- **Appears**: Level 61–70
- **Special mechanic:** Crystal Golem

#### Sky City

- **Story**: After capturing Formusa Factory, the Monkeys discover the formula for anti-gravity technology, allowing them to build floating structures beyond ground attacks.
- **Level 2:** 3 lanes
- **Level 3:** 5 lanes

## Campaign — Play as Alien

Unlocked after completing the first Monkey Campaign playthrough. You control the Alien team with the same core rules.

In the second half, the rules change: without "reinforcements", Aliens no longer receive off-screen Alien Grape supply. Aliens cannot farm, but they can destroy — if they kill a Banana Pub or Banana Tree, these plants become infected and transform into **Alienana** (functions like Banana Tree but generates resources for Aliens). Alienana killed by Monkeys reverts to Banana Pub.

# Challenge

Each map has approximately 7 challenges. You must complete 7 out of 10 levels in a map to unlock its challenges.

# Weekly Event

Similar to Plants vs. Zombies Heroes Weekly Events. Each day of the week offers one match with varied rules, letting players experience the event's featured cards. Variations include:

- **Conveyor Belt** (PvZ-style): No economy in-battle. Monkeys are randomly delivered via conveyor belt at intervals. When the belt is full, no more Monkeys can be delivered. Aliens spawn algorithmically without resource constraints.
- **Puzzle for Alien**: Monkeys and Towers are pre-placed on the battlefield. You play as Aliens with pre-selected, un-upgraded units for fairness.
- **Slot Machine**: Spend 1 banana to pull the lever. Rewards can be cards, bananas, or nothing. The goal can be standard (defeat Targets) or new (accumulate 50 bananas).
- **Column Like You See 'Em**: Entities spawn across all lanes simultaneously.
- **Smash All Aliens**: Eggs fall from the sky and hatch into Aliens. You have no cards — instead, hold down on Aliens to smash them with a hammer.
- **Vase Breaker** (PvZ-style)
- **Last Stand**: You cannot pick cards but are given a large amount of bananas to build a tower defense.
- **Try Out**: Standard mode, but you can try cards available in the Shop.

**Reward:** Tickets. Completing Weekly Event matches grants more Tickets than regular matches. Tickets can be exchanged for Event Cards at the Event Shop.

**Refresh:** Every 2 weeks, both the Shop (old Event Cards replaced) and the available matches are swapped out.

# Galactic War

Planned for release once the player base is stable. Resets weekly with 4 matches — you must complete each match to progress to the next.

# Free Play Mode

A sandbox mode for testing cards and strategies. Two options:

- **Casual Mode:** Pick cards for both yourself and the enemy, then fight normally — destroy Targets to win.
- **Hack Mode:** Control both teams. Toggle features include: disable cooldowns, add 100 / clear all resources, adjust card upgrade levels. You still cannot use unowned cards unless you have a trial card.

# Ranked Mode

To be implemented when the player base is large enough to support matchmaking. Two players — one Monkey, one Alien — compete against each other.

The gap between free-to-play and paying players is narrow. Paying players have access to more high-rarity cards (Epic, Legendary, etc.), granting a wider variety of strategies.

# Lunar New Year [Unfinished]

A special event for the Lunar New Year. Red envelopes (lì xì) drop as equipment.

- **Obtainable Monkeys:** Firecracker, Mammon
- **Obtainable Aliens:** Lion Dancer, Shaman

# Dungeon [Unfinished]