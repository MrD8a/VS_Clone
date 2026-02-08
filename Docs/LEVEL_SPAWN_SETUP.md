# Level spawn system (timer-based scaling)

Enemy spawn rate and which enemy types spawn are driven by **level time** and a **per-level config asset**.

## Overview

- **Enemy stats** do not change during the level; only **who** spawns and **how often** change over time.
- Each **level/map** has one **Level Spawn Config** (ScriptableObject) that defines **phases**.
- Each phase has a **start time** (in seconds) and a list of **spawn entries** (prefab + interval).
- The **GameTimer** is the single source of truth for elapsed level time (used by HUD and spawner).

## Setup

### 1. GameTimer

- Add a **GameTimer** component to a scene object that persists during gameplay (e.g. same GameObject as your spawner or a small “GameManager”).
- The HUD and **EnemySpawner** will use it for the level clock. If not assigned, they fall back to `Time.time` or local logic.

### 2. Level Spawn Config

- In the Project window: **Right‑click → Create → Spawn → Level Spawn Config**.
- Name it (e.g. `Level1SpawnConfig`) and use it for one level/map.

**Phases** (order by `startTime`, ascending):

| startTime | Meaning        | Example entries                          |
|-----------|----------------|------------------------------------------|
| 0         | From start     | Basic enemy, interval 1.5s               |
| 60        | From 1:00      | Basic enemy, interval 0.75s (faster)     |
| 120       | From 2:00      | Basic (0.75s) + Strong enemy (2s)        |

- **startTime**: Phase becomes active when level time (seconds) ≥ this value. Only one phase is active at a time (the latest one that has started).
- **entries**: List of prefab + interval. Each entry is ticked separately (e.g. basic every 0.75s, strong every 2s).

### 3. EnemySpawner

- Assign **Game Timer** and **Level Spawn Config**.
- **Spawn Distance** is unchanged (spawn radius around player).
- **Fallback**: If no config or no phase applies, you can set **Fallback Enemy Prefab** and **Fallback Spawn Interval** so something still spawns.

### 4. GameHUD

- Optionally assign **Game Timer** so the on-screen timer and spawn phases use the same clock. If left empty, the HUD will try to find a `GameTimer` in the scene.

## Adding a new level/map

1. Duplicate an existing Level Spawn Config or create a new one (**Create → Spawn → Level Spawn Config**).
2. Set phases and entries for that level (start times + prefabs + intervals).
3. In that level’s scene, set the **EnemySpawner**’s **Level Spawn Config** to this asset.

## Adding a new enemy type

1. Create the enemy prefab (e.g. with different **EnemyHealth** / **EnemyContactDamage** / **EnemyMovement**).
2. In the Level Spawn Config, add a **Spawn Entry** in the phase where it should appear: set **Prefab** to the new prefab and **Interval** to the desired seconds between spawns.

No code changes are required for new enemies or new phases; everything is data-driven.
