# Game Rules

## Overview
Monkey vs Alien is a lane-based strategy game where two teams (Monkey and Alien) deploy cards to summon Entities onto the battlefield, push their forces toward the opponent's base, and create combat advantages in real-time.

## Entity Attributes
Entities have the following core attributes that determine their combat effectiveness:
- Lane (integer): Entities in the same lane interact with each other. Allies support each other; enemies attack each other.
- X-coordinate position(float): Position along the lane axis.
- Health: When this reaches 0, the Entity dies.
- Move Speed: How fast the Entity moves.
- Range: Attack range.
- Strength: Damage dealt per attack.
- AttackSpeed: How fast the Entity attacks.

Some Entities have special abilities, such as healing allies, self-buffing, or dealing massive damage.

## Resources and Card Usage
Each card has a resource cost and a cooldown timer. A player (or AI) can only play a card when they have enough resources and the card is off cooldown. Therefore, the decision to play a card depends on both current resource availability and the timing of when the next card can be played.

## How to Obtain Resources
- Monkey team owns the Banana Tree card, which is a Tower Entity that generates bananas at regular intervals (similar to Sunflower in Plants vs. Zombies).
- Alien team receives "reinforcements" from outside, providing a steady resource income without needing to farm. While the initial generation rate is lower, the Alien team can upgrade to increase production rate by one tier (similar to Tower Conquest).

## Lanes and Spawn Positions
The battlefield is divided into multiple independent lanes, up to 5 lanes vertically. When a card is played, the Entity spawns at the team's end of the selected lane (Left team spawns from the left, Right team spawns from the right) and automatically moves toward the opponent's base.

Each lane is between 9 and 15 grid cells long, depending on the level.

## Win Condition
The team that destroys 3 enemy Targets first wins the match. 
