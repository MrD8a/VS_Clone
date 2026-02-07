# Player Health and Game Over – Editor Setup

## 1. Player: add health and trigger collider

1. Open **Assets/Scenes/GameScene**.
2. Select the **Player** GameObject in the Hierarchy (the one with tag **Player**).
3. **Add Component** → **Player Health** (script `PlayerHealth`).
4. Set **Max Health** (e.g. `10`), **Health Regen Per Second** (e.g. `0.5`), **Contact Damage Interval** (e.g. `0.25`).
5. Ensure the Player has a **Collider2D** (e.g. **Circle Collider 2D** or **Box Collider 2D**).
   - If there is no collider, add one: **Add Component** → **Circle Collider 2D** (or Box Collider 2D).
   - In the collider, enable **Is Trigger** so enemies overlap without physically colliding.
   - Adjust **Radius** (or size) so it roughly matches the player sprite for contact detection.

The Enemy prefab already has its collider set to **Trigger** and the **Enemy Contact Damage** component, so no collision occurs and damage is applied every 0.25 s while overlapping.

---

## 2. Game Over UI: same setup as Upgrade UI

Use the **same pattern as the Upgrade panel**: the script lives on the **Canvas**, and the panel is a **child** that starts inactive.

1. Under **Canvas**, create a **Panel**: **Right‑click Canvas → UI → Panel**. Rename it to **GameOverPanel**.
2. Style it like the Upgrade panel if you like (e.g. semi‑transparent background). Add **Text (TMP)** "Game Over" and a **Button** "New Game" as children of GameOverPanel.
3. **Select the Canvas** (the same GameObject that has **Upgrade UI** on it).
4. **Add Component** → **Game Over UI** (script `GameOverUI`).
5. In the inspector, assign:
   - **Panel** → drag the **GameOverPanel** (the child panel).
   - **New Game Button** → drag the **NewGameButton**.
6. Leave **GameOverPanel** **inactive** (uncheck the box next to its name in the Hierarchy), just like UpgradePanel. The script will call `panel.SetActive(true)` when the player dies.

---

## 3. Optional: health bar

You can add a simple health bar (e.g. a **Slider** or **Image** fill) and in **Update** set its value from `PlayerHealth.Instance.NormalizedHealth` (or find `PlayerHealth` on the player and read `NormalizedHealth`). This is optional and can be added later.

---

## Summary

- **Player**: has **PlayerHealth** and a **trigger** **Collider2D**.
- **Enemy** prefab: collider is **Trigger**, and **Enemy Contact Damage** is added (damage per tick configurable).
- **Game Over**: **Game Over UI** script on the **Canvas** (like Upgrade UI), **Panel** = GameOverPanel (child, starts inactive). **New Game** reloads the current scene.
