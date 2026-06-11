---
type: technical
audience: [developer]
status: active
language: en
description: Explains the AI system architecture, behavior flow, dependencies, and its relationship to execution paths and design principles.
related:
  - architecture
  - design-principles
  - execution-paths
  - 
---

# AI System Overview

The AI system lives under `Assets/Scripts/Battlefield/AI/` in the `MvA.AI` assembly. It evaluates battle states and recommends actions by simulating outcomes using real C# `Entity` objects running in simulation mode (`isSimulated = true`) without Unity GameObject overhead.

# Goals

- Provide fast, deterministic evaluation for AI decisions.
- Avoid Unity runtime overhead during prediction (no `MonoBehaviour` gameobject creation, no `Transform`).
- Keep simulation generic and independent of concrete entity types.

# Core Components

## Simulator.cs

`Simulator` is a static, pure C# simulation engine. It clones the real entities from the battlefield with `isSimulated = true` (which bypasses Unity's GameObject creation) and runs a pure C# simulation.

**Key ideas:**
- Uses real `Entity` objects to ensure simulation logic (behaviors, stats) matches actual gameplay.
- Cloned entities run pure C# logic without game loop overhead.
- Simulation steps in fixed time slices (`SimulationStep = 1f`).

**Important methods:**
- `EvaluateBundle(...)` — public entry point. Converts inputs and evaluates score.
- `EvaluateBundle_2(...)` — runs two-phase sim (pre-spawn + post-spawn).

## AIManager

AI behavior is split into multiple files:

- `AIManager(Editor).cs` — Editor tooling / debug helpers.
- `AIManager(Upgrade).cs` — Upgrade-related decision logic.
- `AIManager(subclassess).cs` — Data classes and actions for AI.

## EnemyManager.cs

High-level orchestration for AI-controlled enemies. Coordinates with the simulator to select actions and spawning decisions. Exposes parameters such as `costPenaltyFactor` and `upgradeThreshold`.

# Data Flow

```
Current Battle State (IEntity[] + IBattleCard[])
    ↓ clone to simulated Entity objects
Simulator.EvaluateBundle(...)
    ↓ returns numeric score (SimulationResult)
EnemyManager calculates Net Improvement & compares options
    ↓
Chosen AI Action (Bundle spawn, Upgrade, or Wait)
```

# Unity Integration

- Domain objects (entities, cards) are Unity `MonoBehaviour` types, but the simulator is pure C#.
- AI reads entity data through `IEntity` contract (`GetAssessPoint`, `GetRealMoveSpeed`, `ST.Range`, `gridPos.x`).
- No Unity rendering or UI access from AI or simulator.

# Constraints

- **No UI/TMP dependencies** inside `MvA.AI` (Phase 1 rule).
- Conversion must remain **generic**. If a change requires per-entity mapping logic, do not add it — refactor the contract instead.

# Testing

- Unit tests in `Assets/Tests/Editor/SimulatorTests.cs`.
- Tests log snapshots and assert pass (debug-focused tests).

# Extension Guidelines

- Add new simulation fields only if strictly needed by AI decisions.
- Prefer adding new values to `IEntity` contract or shared stats (e.g., `ST` enum) rather than adding concrete-type checks.
- Keep `FakeEntity` minimal — avoid simulating visuals or exact animations.

# Action Types

The AI operates on three action types:

- **Upgrade action** — single parameter: `lookahead`.
- **BundleDecision** — three parameters: `lookahead`, `lane`, `usedCards: List<IBattleCard>`.
- **WaitForFuture** — executes when a future action is predicted to yield higher score than current options.

# Simulation Details

**Context:** Each lane contains multiple `Entity` clones. Each simulated `Entity` uses its actual gameplay behaviors (like `MeleeAttack` and `StraightMove`) and stats.

**Execution loop:**
- Each iteration represents `SimulationStep = 1f` second of simulated time.
- Movement: Simulated entities move toward the enemy side according to their `GetRealMoveSpeed()`.
- Combat: Entities in range deal damage to the nearest enemy based on: `damage = danger * step`, which is subtracted from `Stats[ST.Health]`.
- Death: Entities whose health drops to or below 0 die and are removed from the simulation.
- Scoring: After simulation, the score is computed as `OurTeamPower - EnemyTeamPower`. A team's power is computed as `dangerSum * survivabilitySum * UnitCountDebuff`, where `danger` and `survivability` are retrieved using `GetAssessPoint` (Danger and Defend/Defensive). An alive-team bonus of `1000f` is awarded if one team is completely wiped out.

# Decision-Making Logic

The AI makes decisions by evaluating candidate card bundles and upgrades, comparing them to a "do nothing" baseline:

1. **Candidate Shortlisting**: Performed on a per-lane basis inside `FindBestSimulatedBundle()` in `AIManager.cs`.
   - The AI then generates all possible subsets (combinations) of these valid cards (`laneCandidates`) using a bitmask search.
   - Combinations whose combined lookahead (maximum of resource wait time and card cooldown) exceeds `MaxDecisionLookaheadSeconds = 30f` are discarded.
2. **Lookahead Estimation**: For each candidate bundle, the lookahead time is the maximum of the resource generation wait (`CalculateLookaheadToAfford(totalCost)`) and the cooldown timer (`GetCardCooldownWait(card)`).
3. **Simulation (Multi-Lane Scope)**: The simulator (`Simulator.EvaluateBundle`) operates globally across all lanes.
   - It clones all entities present across the entire battlefield.
   - It runs the simulation for `lookahead` seconds, spawns the candidate card bundle on its target lane, and simulates the combat outcome for another `MaxPostSpawnSeconds = 20f`.
   - **Cross-Lane / Multi-Lane Creatures (Future Problem)**: Currently, there are no creatures that can change lanes (no multi-lane creatures), so combat and movement are isolated per lane during simulation. However, simulating all lanes globally allows the AI to make strategic trade-offs—such as launching an attack on a different lane while sacrificing another lane. If cross-lane creatures or area-of-effect abilities across adjacent lanes are introduced in the future, the simulator will naturally support them since the entire board is simulated together.
4. **Baseline Evaluation**: An empty bundle (doing nothing) is simulated globally across all lanes for the same `lookahead` duration to compute the baseline score.
5. **Cost Penalty & Simulator Tuning**: Raw simulation scores are adjusted to promote resource efficiency and strategic defense:
   - **Cost Penalty**: `effectiveScore = rawScore - cost * costPenaltyFactor` (default `costPenaltyFactor = 50f` in `AIManager`).
   - **Proximity Penalty**: Enemies close to our base deduct from the score: `penalty = Max(0, ProximityMaxDistance - distanceToBase) * ProximityPenaltyWeight` (defaults: `ProximityMaxDistance = 10f`, `ProximityPenaltyWeight = 50f` in `Simulator`).
   - **Protection Loss Penalty**: Damage to or death of units requiring protection (like bases/towers) deducts from the score: `penalty = protectionLoss * ProtectionLossWeight` (default `ProtectionLossWeight = 10f` in `Simulator`).
   - **Anti-Reward Hacking**: Victory (+1000f) or defeat (-1000f) wipeout bonuses are only applied if there was active combat (initial enemies/allies present), preventing the AI from spawning on empty lanes just for victory points.
6. **Net Improvement**: The decision metric is the net improvement over the baseline:
   `netImprovement = effectiveScore - baselineScore`.
   Bundles are only considered if `netImprovement > 0`.
7. **Upgrade Logic**: Upgrading is rule-based:
   - If an upgrade is available and affordable, its action score is set to `upgradeThreshold` (default `800f`).
   - If the best bundle's `netImprovement` is less than `upgradeThreshold`, upgrading wins (no urgent defense needed).
   - If a bundle has `netImprovement >= upgradeThreshold`, the AI prioritizes spawning/defending.

> **Note:** This is an intentionally simplified evaluation — real entity behavior is more complex, but the numeric approximation is sufficient for AI ranking.

# Search Algorithm Analysis

When deciding which cards to spawn, which lane to target, and how to manage resources/time, the following algorithms were considered:

## Monte-Carlo Tree Search (MCTS / UCT)

Builds a tree of actions (spawn, wait, upgrade), uses rollouts via the simulator to estimate value, selects actions by UCT.

| Aspect | Details |
|--------|---------|
| **When** | High randomness / unknown opponent behavior; large branching but cheap rollouts |
| **Pro** | Handles uncertainty well; easy to plug current Simulator as rollout |
| **Con** | Simulation-heavy; needs playout/time limits; needs move-generation & policy to reduce branching |

## Expectimax / Minimax (with alpha-beta pruning)

Game tree (our move / enemy move), expectimax if opponent is stochastic.

| Aspect | Details |
|--------|---------|
| **When** | Good opponent model available (or assume rational opponent) |
| **Pro** | Rational decisions grounded in game theory |
| **Con** | Branching × depth explodes; needs reduced action space and fast state transitions |

## Beam Search (best-k expansion + simulation)

Each step expands only top-k nodes by heuristic or short simulation; continues for several depths.

| Aspect | Details |
|--------|---------|
| **When** | Need breadth-depth trade-off under time constraints |
| **Pro** | Lightweight, easy to integrate with simulator for node scoring |
| **Con** | May miss optimal paths if beam is too narrow |

## Greedy / Best-First with Local Search (hill-climbing, simulated annealing)

Pick the best action by heuristic/sim; try neighbors (swap a card, change lane); iterate.

| Aspect | Details |
|--------|---------|
| **When** | Very low latency required; quick improvements needed |
| **Pro** | Simple, low computation |
| **Con** | Prone to local optima |

## Knapsack / Dynamic Programming (budgeted selection + per-lane simulation)

Treat card combos as a knapsack problem (cost = mana/time) to shortlist optimal combos; then simulate the shortlist.

| Aspect | Details |
|--------|---------|
| **When** | Need fast valid-combo filtering by resource constraints |
| **Pro** | Drastically reduces combinatorial space; saves simulations |
| **Con** | Cannot model complex time interactions (cooldown, spawn timing) without state expansion |

## Monte-Carlo (flat sampling / rollouts)

Generate many random scenarios (card + timing combos), simulate, pick best average.

| Aspect | Details |
|--------|---------|
| **When** | Action space too large for tree search; fast rollouts |
| **Pro** | Simple, parallelizable |
| **Con** | Needs many rollouts for stable results |

## Genetic / Evolutionary Algorithms (GA)

Represent combos as individuals; crossover/mutate; fitness = simulation score.

| Aspect | Details |
|--------|---------|
| **When** | Long-horizon optimization (offline deck/tactic discovery) |
| **Pro** | Good for large search spaces, offline tuning |
| **Con** | Slow for realtime; needs large populations |

## Branch-and-Bound / A\* Style

Expand by priority; use an upper bound (e.g., max reachable total power) to prune.

| Aspect | Details |
|--------|---------|
| **When** | Fast bound calculation available |
| **Pro** | Aggressive pruning if bounds are tight |
| **Con** | Needs good bounds; hard to design |

# Recommended Hybrid Approach

**Practical hybrid pipeline:**
1. **Knapsack / heuristic** to shortlist candidate combos per lane.
2. **MCTS or Beam Search** on the reduced space, using the current Simulator for rollouts.

This balances decision quality and computational cost.

- **If latency is very low:** Greedy / Beam with small beam (k=3..10) + cached results.
- **If opponent is uncertain:** MCTS with opponent model / rollout policy.

# Implementation Plan

| Priority | Task |
|----------|------|
| 1 | Reduce branching: macro-actions (spawn bundle at once, discrete "wait Xs"), top-N cards per lane |
| 2 | Cheap state copy: use `FakeEntity` (already available) + pooling; avoid `Instantiate` during rollouts |
| 3 | Parallelize rollouts (Jobs / ThreadPool) for high volume |
| 4 | Caching transposition: hash state → cached score |
| 5 | Iterative deepening / time-budgeted search: run search within X ms per decision |

**Next steps (choose one):**
- A) Implement MCTS skeleton using Simulator as rollout engine.
- B) Implement Beam Search + knapsack shortlist pipeline.
- C) Write benchmark comparing Beam vs MCTS on several scenarios.
