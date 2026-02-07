# In-Game HUD – Editor Setup

Create the following in the editor, then assign the references to the scripts.

---

## 1. Player health bar (under the player)

The bar sits under the player in world space and is only visible when the player has taken damage.

### Step 1 – Create the Canvas

1. In the **Hierarchy**, select the **Player**.
2. Right‑click the Player → **UI** → **Canvas**. A new child object is created.
3. Rename it to **HealthBarCanvas** (select it and press F2, or change **Name** at the top of the Inspector).
4. With **HealthBarCanvas** selected, in the **Inspector**:
   - Find the **Canvas** component. Set **Render Mode** to **World Space** (use the dropdown).
   - Find **Rect Transform**:
     - **Pos X** = 0, **Pos Y** = **-0.6** (so the bar sits under the player), **Pos Z** = 0.
     - **Width** = **100**, **Height** = **15**.
     - **Scale** = **0.01** for X, Y, and Z (so the bar is about 1 world unit wide).

### Step 2 – Add the background

1. In the Hierarchy, right‑click **HealthBarCanvas** → **UI** → **Image**. Rename the new object to **Background**.
2. Select **Background**. In the Inspector:
   - **Rect Transform**: click the **Anchor** square (top-left of Rect Transform). Choose the **stretch–stretch** preset (bottom-right option: square that fills the parent). Set **Left**, **Right**, **Top**, **Bottom** all to **0**.
   - **Image** component: set **Color** to a dark grey (e.g. R 0.2, G 0.2, B 0.2). Leave **Source Image** as the default (e.g. UISprite) or assign any sprite.

### Step 3 – Add the fill (the red bar that shrinks)

1. In the Hierarchy, right‑click **HealthBarCanvas** → **UI** → **Image**. Rename the new object to **Fill**.
2. Select **Fill**. In the Inspector:
   - **Rect Transform**: same as Background — Anchor **stretch–stretch**, **Left/Right/Top/Bottom** = **0**.
   - **Image** component:
     - **Color**: set to red (e.g. R 0.9, G 0.2, B 0.2).
     - **Source Image**: must have a sprite (the default **UISprite** is fine; if it says “None”, use the circle to assign one from **Assets/TextMesh Pro/Resources** or any sprite).
     - **Image Type**: click the dropdown (it usually says “Simple”). Change it to **Filled**.
     - After choosing **Filled**, extra options appear below:
       - **Fill Method**: set to **Horizontal**.
       - **Fill Origin**: set to **Left** (so the bar empties from the right).
       - **Fill Amount**: set to **1** for now (the script will drive this; 1 = full, 0 = empty).

### Step 4 – Wire up the Player Health Bar script

1. In the Hierarchy, select the **Player** (the root player object, not HealthBarCanvas).
2. In the Inspector, ensure the **Player Health Bar** component is there. If not: **Add Component** → search **Player Health Bar** → add it.
3. In **Player Health Bar** you’ll see two slots:
   - **Bar Root**: drag **HealthBarCanvas** from the Hierarchy into this field (or use the circle and select it).
   - **Fill Image**: drag the **Fill** object (the child of HealthBarCanvas) into this field. It must be the one with the **Image** component you set to Filled.

### Step 5 – Initial state

- Leave **HealthBarCanvas** active (checkbox ticked in the Inspector). The script will turn it off when the player is at full health and turn it on when they take damage.

---

## 2. XP bar and timer (on the main Canvas)

1. **HUD container**
   - Select the **Canvas** (the one used for Upgrade / Game Over).
   - Right‑click Canvas → **Create Empty**. Rename to **HUD**.
   - Set **Rect Transform**: Anchor **Top Stretch** (top‑center), **Top** = 0, **Height** e.g. **80**, **Left/Right** = 0.

2. **XP bar**
   - Right‑click **HUD** → **UI** → **Image**. Rename to **XPBarBg**.
   - Anchor **Top Stretch**, **Top** = 0, **Height** e.g. **12**, **Left** = 10, **Right** = 10.
   - Set **Color** to a dark colour (e.g. dark grey).
   - Right‑click **XPBarBg** → **UI** → **Image**. Rename to **XPBarFill**.
   - Stretch to fill XPBarBg (anchor min 0,0 max 1,1, offsets 0).
   - Set **Color** to blue (or your choice).
   - **Image Type** = **Filled**, **Fill Method** = **Horizontal**, **Fill Origin** = **Left**, **Fill Amount** = 0 (script will update it).
   - Ensure this Image has a **Sprite** assigned (e.g. UISprite or any sprite).

3. **Timer**
   - Right‑click **HUD** → **UI** → **Text - TextMeshPro** (or **Text (Legacy)** if you prefer). Rename to **Timer**.
   - Anchor **Top Center**, **Pos Y** e.g. **-28**, **Width** 200, **Height** 30.
   - Set text to **00:00**, font size e.g. **24**, alignment **Center**.
   - If using TMP, ensure the **Font Asset** is assigned (e.g. LiberationSans SDF).

4. **Wire up the script**
   - Select the **Canvas** (or the object that has **Game HUD**).
   - Add **Game HUD** if not already there.
   - Assign **Xp Bar Fill** → the **XPBarFill** Image (the child of XPBarBg).
   - Assign **Timer Text** → the **Timer** text component.

---

## Summary

- **Player**: Health bar = **HealthBarCanvas** (World Space) under the player with **Background** + **Fill** (Image, Filled horizontal). **Player Health Bar** script: **Bar Root** = HealthBarCanvas, **Fill Image** = Fill.
- **Canvas**: **HUD** with **XPBarBg** → **XPBarFill** (Filled) and **Timer** text. **Game HUD** script: **Xp Bar Fill** = XPBarFill, **Timer Text** = Timer.
