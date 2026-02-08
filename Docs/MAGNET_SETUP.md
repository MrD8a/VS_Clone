# Magnet (XP / pickup pull) – Setup

## In the editor

1. **Player**
   - Select the **Player** in the Hierarchy.
   - **Add Component** → **Player Magnet** (script `PlayerMagnet`).
   - Set **Magnet Range** (e.g. **3**) and **Pull Speed** (e.g. **8**) as needed.

2. **XP orbs**
   - The **XP Orb** prefab already has the **Magnet Pullable** component. Any new pickup you want to be pulled should also have **Magnet Pullable** (and optionally **Pull Speed Multiplier**).

## Upgrades

- To add a “Magnet Range” upgrade: create a new **Upgrade** asset (Right‑click in Project → Create → Upgrades → Upgrade). Set **Type** = **Magnet Range** and **Value** (e.g. **1** for +1 range). Add it to the **Upgrade UI** “All Upgrades” list. The apply logic is already in place.
