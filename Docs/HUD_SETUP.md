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

These sit at the top of the screen on your main Canvas (the one used for Upgrade / Game Over).

### Step 1 – Create the HUD container

1. In the **Hierarchy**, select the **Canvas** (the one with Upgrade UI / Game Over UI).
2. Right‑click the Canvas → **Create Empty**. Rename the new object to **HUD**.
3. Select **HUD**. In the **Inspector**, **Rect Transform**:
   - Click the **Anchor** square. Choose **top‑stretch** (top row, middle: anchor at top, stretches horizontally). Or set **Min** (0, 1), **Max** (1, 1).
   - **Pos X** = 0, **Pos Y** = 0, **Pos Z** = 0.
   - **Left** = 0, **Right** = 0, **Top** = 0, **Bottom** = **-80** (so **Height** is 80). Or set **Height** = 80 and **Top** = 0.

### Step 2 – Add the XP bar background

1. Right‑click **HUD** in the Hierarchy → **UI** → **Image**. Rename to **XPBarBg**.
2. Select **XPBarBg**. In the Inspector:
   - **Rect Transform**: Anchor **top‑stretch** (same as HUD). **Left** = **10**, **Right** = **10**, **Top** = **0**, **Height** = **12** (or **Top** = 0, **Bottom** = -12).
   - **Image** component: **Color** = dark grey (e.g. R 0.15, G 0.15, B 0.2). Leave **Source Image** as default or assign any sprite.

### Step 3 – Add the XP bar fill (the bar that fills with XP)

1. Right‑click **XPBarBg** in the Hierarchy → **UI** → **Image**. Rename to **XPBarFill**.
2. Select **XPBarFill**. In the Inspector:
   - **Rect Transform**: Anchor **stretch–stretch** (fill parent). **Left**, **Right**, **Top**, **Bottom** all = **0**.
   - **Image** component:
     - **Color**: e.g. blue (R 0.3, G 0.6, B 1).
     - **Source Image**: must have a sprite (default UISprite is fine).
     - **Image Type**: change from **Simple** to **Filled**.
     - After **Filled**, set **Fill Method** = **Horizontal**, **Fill Origin** = **Left**, **Fill Amount** = **0** (script will drive this; 0 = empty, 1 = full for next level).

### Step 4 – Add the timer text

1. Right‑click **HUD** in the Hierarchy → **UI** → **Text - TextMeshPro**. (If prompted to import TMP essentials, do so.) Rename to **Timer**.
2. Select **Timer**. In the Inspector:
   - **Rect Transform**: Anchor **top center** (top row, middle). **Pos X** = 0, **Pos Y** = **-28**, **Width** = **200**, **Height** = **30**.
   - **TextMeshPro - Text (UI)** (or **Text Input**): **Text** = **00:00**, **Font Size** = **24**, **Alignment** = horizontal and vertical center. Assign **Font Asset** (e.g. LiberationSans SDF from TextMesh Pro/Resources/Fonts & Materials) if it says “None”.

### Step 5 – Wire up the Game HUD script

1. Select the **Canvas** in the Hierarchy (the same one that has Upgrade UI).
2. In the Inspector, add **Game HUD** if it’s not there: **Add Component** → search **Game HUD**.
3. In **Game HUD**:
   - **Xp Bar Fill**: drag **XPBarFill** from the Hierarchy (the child of XPBarBg) into this field.
   - **Timer Text**: drag **Timer** from the Hierarchy into this field.

---

## Summary

- **Player**: Health bar = **HealthBarCanvas** (World Space) under the player with **Background** + **Fill** (Image, Filled horizontal). **Player Health Bar** script: **Bar Root** = HealthBarCanvas, **Fill Image** = Fill.
- **Canvas**: **HUD** with **XPBarBg** → **XPBarFill** (Filled) and **Timer** text. **Game HUD** script: **Xp Bar Fill** = XPBarFill, **Timer Text** = Timer.
