# Map & Camera Setup (Playtesting)

## 1. Camera locked to player

1. Open **Assets/Scenes/GameScene**.
2. Select **Main Camera** in the Hierarchy.
3. Add component: **Camera Follow** (script `CameraFollow`).
4. Leave **Target** empty to auto-find the GameObject with tag **Player**, or drag your Player into **Target**.
5. **Smooth Time**: `0.15` (slight smoothing) or `0` for instant follow.
6. Ensure your player GameObject has tag **Player** (Inspector → Tag → Player).

## 2. Infinite map (chunk-based)

1. In the Hierarchy, create an empty GameObject: **Right-click → Create Empty**. Rename it to **MapManager** (or **ChunkManager**).
2. Add component: **Chunk Manager** (script `ChunkManager`).
3. Assign **Player**: drag the Player from the Hierarchy into the **Player** field (optional; if empty, it finds by tag **Player**).
4. **Chunk Size**: `20` (world units per chunk). **Load Radius**: `2` (loads 5×5 chunks around the player).
5. **Ground sprites** (required for terrain):
   - Expand **Ground Sprites** and set **Size** to `5`.
   - Assign **Assets/Art/DungeonTilesetII/floor_1** through **floor_5** (drag the sprites into the slots). This gives per-chunk variation.
   - **Tile Size (World Units)**: leave at `0` to use each sprite’s size from its **Pixels Per Unit** (no gaps). For 16×16 sprites, set the import **Pixels Per Unit** to `16` so one tile = 1 world unit. To force a size (e.g. `1`), set **Tile Size (World Units)** to that value.
6. **Decor sprites** (optional):
   - Set **Size** to `3` and assign e.g. **column_wall**, **column**, **crate** from the same folder.
   - **Decor Min/Max Per Chunk**: e.g. `0` and `5` so each chunk gets 0–5 decor objects.
   - **Decor Block Movement**: leave unchecked so decor is visual only; check if you want obstacles.
7. Save the scene and enter Play mode. Move the player in any direction; new chunks generate as you go and old ones unload.

## Tips

- Use **Ground Tint** to tint all ground (e.g. slight color variation later).
- If the camera is 3D, set the camera’s **Z** to `-10` and keep the **CameraFollow** script (it preserves Z).
- For different feel, increase **Chunk Size** or **Load Radius** (larger radius = more chunks loaded, heavier for low-end devices).
