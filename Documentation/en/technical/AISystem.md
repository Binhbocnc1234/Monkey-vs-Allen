---
type: technical
audience: [developer]
status: active
language: en
description: Explains the AI system architecture, behavior flow, dependencies, and its relationship to execution paths and design principles.
related:
  - architecture-as-is
  - design-principles
  - execution-paths
---

# AI System Overview

The AI system lives under `Assets/Scripts/Battlefield/AI/` in the `MvA.AI` assembly. It evaluates battle states and recommends actions by simulating outcomes using a lightweight model (`FakeEntity`) rather than full Unity objects.

# Goals

- Provide fast, deterministic evaluation for AI decisions.
- Avoid Unity runtime overhead during prediction (no `MonoBehaviour`, no `Transform`).
- Keep simulation generic and independent of concrete entity types.

# Core Components

## Simulator.cs

`Simulator` is a static, pure C# simulation engine. It converts real entities into `FakeEntity` snapshots and runs a simplified battle model.

**Key ideas:**
- `FakeEntity` is a POCO: `team`, `x`, `moveSpeed`, `range`, `danger`, `survivability`.
- Conversion from `IEntity` is generic; no concrete-entity branching.
- Simulation steps in fixed time slices (`SimulationStep = 1f`).

**Important methods:**
- `EvaluateBundle(...)` — public entry point. Converts inputs and evaluates score.
- `EvaluateBundle_2(...)` — runs two-phase sim (pre-spawn + post-spawn).
- `GetWorldSnapshot()` — returns simulated state for debugging/tests.

## AIManager

AI behavior is split into multiple files for responsibilities:

- `AIManager(Editor).cs` — Editor tooling / debug helpers.
- `AIManager(Forecast).cs` — prediction logic and forecast evaluation.
- `AIManager(Upgrade).cs` — upgrade-related decision logic.
- `AIManager(subclassess).cs` — AI behavior variants (strategy overrides).

## EnemyManager.cs

High-level orchestration for AI-controlled enemies. Coordinates with the simulator to select actions and spawning decisions.

# Data Flow

```
Current Battle State (IEntity[] + IBattleCard[])
    ↓ convert to FakeEntity snapshots
Simulator.EvaluateBundle(...)
    ↓ returns numeric score
AI Manager chooses best action based on score
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

**Context:** Each lane contains multiple `FakeEntity` instances. Each `FakeEntity` has: `gridPos` (x, y), `moveSpeed`, `range`, `danger`, `survivability`.

**Execution loop:**
- Each iteration represents 1 second of simulated time.
- Per lane, entities in range attack the nearest enemy: `target.survivability -= attacker.attackPower / 7`.

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