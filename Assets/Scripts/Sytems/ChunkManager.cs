using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates an infinite, procedural map by loading and unloading chunks around the player.
///
/// Each chunk is a square of <see cref="chunkSize"/> world units containing tiled ground sprites
/// and optional decor sprites. Chunks within <see cref="loadRadius"/> of the player's current
/// chunk coordinate are created; those outside the radius are destroyed.
///
/// Ground tile variation and decor placement use a deterministic seed derived from the chunk
/// coordinates so the same chunk always looks the same regardless of visit order.
/// </summary>
public class ChunkManager : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Header("Chunk Settings")]
    [Tooltip("Size of each chunk in world units (square).")]
    [SerializeField] private float chunkSize = 20f;

    [Tooltip("How many chunks around the player to keep loaded (Manhattan distance).")]
    [SerializeField] private int loadRadius = 2;

    [Header("Ground")]
    [Tooltip("Sprites randomly chosen for each ground tile.")]
    [SerializeField] private Sprite[] groundSprites;

    [Tooltip("Tint applied to every ground tile.")]
    [SerializeField] private Color groundTint = Color.white;

    [Tooltip("World units per tile. 0 = derive from sprite Pixels Per Unit (no gaps).")]
    [SerializeField] private float tileSizeWorldUnits = 0f;

    [Header("Decor (optional)")]
    [Tooltip("Sprites randomly placed as decorations within each chunk.")]
    [SerializeField] private Sprite[] decorSprites;

    [Tooltip("Minimum decor objects per chunk.")]
    [SerializeField] private int decorMinPerChunk = 0;

    [Tooltip("Maximum decor objects per chunk.")]
    [SerializeField] private int decorMaxPerChunk = 5;

    [Tooltip("Whether decor objects should have a collider that blocks movement.")]
    [SerializeField] private bool decorBlockMovement;

    [Header("References")]
    [Tooltip("Player transform (found by tag if not assigned).")]
    [SerializeField] private Transform player;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Map of chunk coordinate → chunk root GameObject.</summary>
    private readonly Dictionary<Vector2Int, GameObject> _chunks = new();

    /// <summary>Parent transform that holds all chunk root objects (for hierarchy cleanliness).</summary>
    private Transform _chunkParent;

    /// <summary>Constant used to derive a deterministic seed per chunk coordinate.</summary>
    private const int SeedMultiplier = 0x1A7B3C9D;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Create the chunks parent object and find the player if not assigned.</summary>
    private void Awake()
    {
        _chunkParent = new GameObject("Chunks").transform;
        _chunkParent.SetParent(transform);

        if (player == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                player = playerGo.transform;
        }
    }

    /// <summary>
    /// Each frame: load any missing chunks within radius, unload chunks outside radius.
    /// </summary>
    private void Update()
    {
        if (player == null) return;

        Vector2Int playerChunk = WorldToChunk(player.position);

        // Load missing chunks within the load radius.
        for (int x = playerChunk.x - loadRadius; x <= playerChunk.x + loadRadius; x++)
        for (int y = playerChunk.y - loadRadius; y <= playerChunk.y + loadRadius; y++)
        {
            var key = new Vector2Int(x, y);
            if (!_chunks.ContainsKey(key))
                _chunks[key] = CreateChunk(x, y);
        }

        // Collect chunks that are now outside the load radius.
        List<Vector2Int> toRemove = null;
        foreach (var kv in _chunks)
        {
            int dx = kv.Key.x - playerChunk.x;
            int dy = kv.Key.y - playerChunk.y;
            if (Mathf.Abs(dx) > loadRadius || Mathf.Abs(dy) > loadRadius)
            {
                toRemove ??= new List<Vector2Int>();
                toRemove.Add(kv.Key);
            }
        }

        // Destroy and remove out-of-range chunks.
        if (toRemove != null)
        {
            foreach (var key in toRemove)
            {
                if (_chunks.TryGetValue(key, out GameObject go))
                {
                    Destroy(go);
                    _chunks.Remove(key);
                }
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Convert a world position to a chunk grid coordinate.
    /// </summary>
    private Vector2Int WorldToChunk(Vector3 world)
    {
        int cx = Mathf.FloorToInt(world.x / chunkSize);
        int cy = Mathf.FloorToInt(world.y / chunkSize);
        return new Vector2Int(cx, cy);
    }

    /// <summary>
    /// Create a chunk at the given grid coordinate. The chunk contains ground tiles
    /// and (optionally) decor sprites. Uses a deterministic seed for consistency.
    /// </summary>
    private GameObject CreateChunk(int chunkX, int chunkY)
    {
        // Deterministic seed so the same chunk always looks the same.
        int seed = (chunkX * SeedMultiplier) ^ (chunkY * (SeedMultiplier >> 1));
        Random.InitState(seed);

        float baseX = chunkX * chunkSize;
        float baseY = chunkY * chunkSize;

        // Create the chunk root GameObject.
        var root = new GameObject($"Chunk_{chunkX}_{chunkY}");
        root.transform.SetParent(_chunkParent);
        root.transform.position = new Vector3(baseX, baseY, 0f);

        // ── Ground tiles ──────────────────────────────────────────────
        if (groundSprites != null && groundSprites.Length > 0)
        {
            Sprite firstSprite = groundSprites[0];
            float spriteWorldSize = firstSprite.bounds.size.x;

            // Use the override tile size or derive from sprite PPU.
            float effectiveTileSize = tileSizeWorldUnits > 0f ? tileSizeWorldUnits : spriteWorldSize;
            int tilesPerAxis = Mathf.CeilToInt(chunkSize / effectiveTileSize);

            for (int tx = 0; tx < tilesPerAxis; tx++)
            for (int ty = 0; ty < tilesPerAxis; ty++)
            {
                var tile = new GameObject("Ground");
                tile.transform.SetParent(root.transform);
                tile.transform.localPosition = new Vector3(
                    tx * effectiveTileSize + effectiveTileSize * 0.5f,
                    ty * effectiveTileSize + effectiveTileSize * 0.5f,
                    0f);

                var sr = tile.AddComponent<SpriteRenderer>();
                Sprite chosen = groundSprites[Random.Range(0, groundSprites.Length)];
                sr.sprite = chosen;
                sr.color = groundTint;
                sr.sortingOrder = -10;

                // Scale the tile so it fills exactly one effectiveTileSize cell.
                float tileScale = effectiveTileSize / chosen.bounds.size.x;
                tile.transform.localScale = new Vector3(tileScale, tileScale, 1f);
            }

            // One large trigger collider for the whole chunk (for potential ground detection).
            var groundCollider = root.AddComponent<BoxCollider2D>();
            groundCollider.size = new Vector2(chunkSize, chunkSize);
            groundCollider.offset = new Vector2(chunkSize * 0.5f, chunkSize * 0.5f);
            groundCollider.isTrigger = true;
        }

        // ── Decor ─────────────────────────────────────────────────────
        if (decorSprites != null && decorSprites.Length > 0 && decorMaxPerChunk > 0)
        {
            int count = Random.Range(decorMinPerChunk, decorMaxPerChunk + 1);
            float margin = 1.5f; // Keep decor away from chunk edges.

            for (int i = 0; i < count; i++)
            {
                float px = Random.Range(margin, chunkSize - margin);
                float py = Random.Range(margin, chunkSize - margin);

                var decor = new GameObject("Decor");
                decor.transform.SetParent(root.transform);
                decor.transform.localPosition = new Vector3(px, py, 0f);

                var sr = decor.AddComponent<SpriteRenderer>();
                sr.sprite = decorSprites[Random.Range(0, decorSprites.Length)];
                sr.sortingOrder = -5;

                // Optionally add a collider so decor blocks movement.
                if (decorBlockMovement)
                {
                    var col = decor.AddComponent<BoxCollider2D>();
                    col.size = Vector2.one * 0.8f;
                }
            }
        }

        return root;
    }
}
