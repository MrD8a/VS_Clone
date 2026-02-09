# Weapons Setup

## Size stat

All weapons use a **Size** stat from `WeaponData`:

- **Assault rifle**: scales the projectile (localScale).
- **Railgun**: line width.
- **Shotgun**: cone radius/scale.

You can add a future upgrade type (e.g. `WeaponSize`) and call `Weapon.ModifySize`, `RailgunWeapon.ModifySize`, `ShotgunWeapon.ModifySize` from `UpgradeUI`.

---

## Assault rifle (existing)

Uses `Weapon` + `WeaponData` with **Projectile Prefab** set. Size scales the projectile when fired.

---

## Railgun

1. **WeaponData**  
   Create a Weapon Data asset (or duplicate one). Set:
   - **Line Prefab** → your railgun line prefab (see below).
   - **Line Length** → world length of the line (e.g. 12).
   - **Line Duration** → how long the line stays visible (e.g. 0.08).
   - **Size** → line width.
   - **Spawn Offset From Player** → distance from player center so the line starts at the player’s edge (e.g. 0.5).
   - Cooldown, Damage, Range as usual.

2. **Railgun line prefab**  
   - Empty GameObject + **RailgunLine** script + **BoxCollider2D** (script requires it).
   - Set the BoxCollider2D to **Trigger**. Size/offset are set in code from weapon data.
   - **SpriteRenderer**: single sprite (e.g. 128×32). Length along local X, width along local Y.
   - Optional: **Sorting Layer / Order**.

3. **In scene**  
   Add **RailgunWeapon** to the player (or a child). Assign the Weapon Data that has **Line Prefab** set.

---

## Shotgun

1. **WeaponData**  
   Create a Weapon Data asset. Set:
   - **Cone Prefab** → your shotgun cone prefab (see below).
   - **Cone Angle** → half-angle in degrees (e.g. 25 = 50° total cone).
   - **Cone Duration** → how long the cone stays visible (e.g. 0.1).
   - **Size** → cone radius/scale.
   - **Spawn Offset From Player** → distance from player center so the cone base is at the player’s edge (e.g. 0.5).
   - Cooldown, Damage, Range as usual.

2. **Shotgun cone prefab**  
   - Empty GameObject + **ShotgunCone** script + **PolygonCollider2D** (script requires it).
   - Set the PolygonCollider2D to **Trigger**. The script sets the wedge path in Setup.
   - **SpriteRenderer**: single sprite, cone pointing along local X (right).
   - Set the prefab's **Transform scale** for cone size at **Size = 1**; Size stat multiplies it.
   - Optional: **Sorting Layer / Order**.

3. **In scene**  
   Add **ShotgunWeapon** to the player (or a child). Assign the Weapon Data that has **Cone Prefab** set.

The cone always fires **horizontally left or right** from the player (from `PlayerController.FacingDirection`), and spawns at the player position (no offset).

---

## Weapon Manager – editor setup

**WeaponManager** applies upgrades (cooldown, damage, size) to every equipped weapon. You assign **scene objects** (the GameObjects that have the weapon scripts), not the ScriptableObjects.

### 1. Add and configure WeaponManager

1. In the **Hierarchy**, pick the object that should own the manager (e.g. **Player** or a **GameManager**).
2. **Add Component** → **Weapon Manager** (script `WeaponManager`).
3. In the Inspector you’ll see:
   - **Assault Rifle** (object reference)
   - **Railgun** (object reference)
   - **Shotgun** (object reference)

4. For each slot, assign the **GameObject that has that weapon component**:
   - **Assault Rifle** → drag the GameObject that has the **Weapon** component (e.g. a child of Player named “AssaultRifle” or “Weapon”). Do **not** assign the WeaponData asset.
   - **Railgun** → drag the GameObject that has the **RailgunWeapon** component.
   - **Shotgun** → drag the GameObject that has the **ShotgunWeapon** component.

   Leave a slot **empty** if that weapon is not in the scene. The manager only applies upgrades to non‑null references.

**Summary:** WeaponManager expects **scene GameObjects with weapon components**, not WeaponData assets. The WeaponData assets stay assigned on each weapon component (Weapon, RailgunWeapon, ShotgunWeapon) as before.

### 2. Connect Upgrade UI to WeaponManager

1. Select the **GameObject that has the UpgradeUI script** (e.g. your Canvas or a child of it).
2. In the Inspector, find the **Upgrade UI** component.
3. Find the **Weapon Manager** field.
4. Drag the **GameObject that has the WeaponManager script** into this field (e.g. your Player or GameManager). Unity will store the **WeaponManager component** reference.

If you leave **Weapon Manager** empty, UpgradeUI will try to find a WeaponManager in the scene at runtime; assigning it in the editor is more reliable.

## Upgrades

`UpgradeUI` uses **WeaponManager** for weapon upgrades. To support new weapon types:

- Add the new weapon reference to **WeaponManager** and handle its upgrade type in `ApplyWeaponUpgrade`.

---

## Hit detection (trigger colliders)

Railgun and shotgun use **trigger colliders** for hits:

- **Railgun:** **BoxCollider2D** (trigger). Script sets size and offset in Setup so the box covers the line plus the spawn-offset strip. Enemies that overlap the box get damage once (tracked in a set).
- **Shotgun:** **PolygonCollider2D** (trigger). Script sets a wedge path in Setup (apex at cone base, radius = spawn offset + forward extent). Enemies inside the wedge get damage once.

**OnTriggerEnter2D** and **OnTriggerStay2D** are used so enemies already overlapping when the effect spawns are still hit. Each enemy is damaged only once per shot.

**How shape and sprite stay aligned**  
One set of dimensions drives both the hitbox and the sprite. (1) Weapon creates a shape (rectangle or cone) from data and the Size stat. (2) The same values set transform.localScale so the shape scales with stats. (3) Align the sprite by making the prefab 1 unit at scale (1,1): **Railgun** – set PPU so the sprite is 1x1 units (length along X, width along Y). **Shotgun** – set prefab scale or PPU so at scale (1,1) the sprite extends 1 unit along local X. No bounds logic; hitbox and sprite stay in sync.

**Tuning**
- **Railgun:** The box is centered on the line’s transform and has size `(length, width)`; the line’s pivot should be at the line center.
- **Shotgun:** The cone’s reach uses `lossyScale.x` (forward extent) and the cone angle; if the visual is smaller, reduce the cone prefab’s scale or add a radius multiplier in `ShotgunCone`.
- To match the sprite precisely, add a **BoxCollider2D** (railgun) or **PolygonCollider2D** (shotgun wedge) to the prefab, set to Trigger, and switch the scripts to use **OnTriggerEnter2D** and a “already hit” set instead of Overlap.

---

## Replacing with animations later

The line and cone prefabs each use a single sprite today. To swap to animations later:

- Replace the **SpriteRenderer** with an **Animator** and an Animation that plays over the same duration as **Line Duration** / **Cone Duration**.
- Keep the same **RailgunLine** / **ShotgunCone** scripts; they only control damage timing and lifetime, not the visual.
