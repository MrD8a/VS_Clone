using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates an infinite map by loading/unloading chunks around the player.
/// Each chunk has a ground tile and a few decor sprites; terrain varies by chunk position.
/// </summary>
public class ChunkManager : MonoBehaviour
{
    [Header("Chunk settings")]
    [SerializeField] private float chunkSize = 20f;
    [SerializeField] private int loadRadius = 2;

    [Header("Ground")]
    [SerializeField] private Sprite[] groundSprites;
    [SerializeField] private Color groundTint = Color.white;
    [Tooltip("World units per tile. 0 = use sprite size from Pixels Per Unit (no gaps). e.g. 1 for 16x16 @ 16 PPU")]
    [SerializeField] private float tileSizeWorldUnits = 0f;

    [Header("Decor (optional)")]
    [SerializeField] private Sprite[] decorSprites;
    [SerializeField] private int decorMinPerChunk = 0;
    [SerializeField] private int decorMaxPerChunk = 5;
    [SerializeField] private bool decorBlockMovement;

    [Header("References")]
    [SerializeField] private Transform player;

    private readonly Dictionary<Vector2Int, GameObject> _chunks = new();
    private Transform _chunkParent;

    private const int SeedMultiplier = 0x1A7B3C9D;

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

    private void Update()
    {
        if (player == null) return;

        Vector2Int playerChunk = WorldToChunk(player.position);

        for (int x = playerChunk.x - loadRadius; x <= playerChunk.x + loadRadius; x++)
        for (int y = playerChunk.y - loadRadius; y <= playerChunk.y + loadRadius; y++)
        {
            var key = new Vector2Int(x, y);
            if (!_chunks.ContainsKey(key))
                _chunks[key] = CreateChunk(x, y);
        }

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

    private Vector2Int WorldToChunk(Vector3 world)
    {
        int cx = Mathf.FloorToInt(world.x / chunkSize);
        int cy = Mathf.FloorToInt(world.y / chunkSize);
        return new Vector2Int(cx, cy);
    }

    private GameObject CreateChunk(int chunkX, int chunkY)
    {
        int seed = (chunkX * SeedMultiplier) ^ (chunkY * (SeedMultiplier >> 1));
        Random.InitState(seed);

        float baseX = chunkX * chunkSize;
        float baseY = chunkY * chunkSize;

        var root = new GameObject($"Chunk_{chunkX}_{chunkY}");
        root.transform.SetParent(_chunkParent);
        root.transform.position = new Vector3(baseX, baseY, 0f);

        // Ground: tile size comes from sprite (PPU) or from override so tiles sit flush.
        if (groundSprites != null && groundSprites.Length > 0)
        {
            Sprite firstSprite = groundSprites[0];
            float spriteWorldSize = firstSprite.bounds.size.x;

            float effectiveTileSize = tileSizeWorldUnits > 0f ? tileSizeWorldUnits : spriteWorldSize;
            int tilesPerAxis = Mathf.CeilToInt(chunkSize / effectiveTileSize);
            float scale = effectiveTileSize / spriteWorldSize;

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

                float tileScale = effectiveTileSize / chosen.bounds.size.x;
                tile.transform.localScale = new Vector3(tileScale, tileScale, 1f);
            }

            // One large collider for the whole chunk so nothing falls through
            var groundCollider = root.AddComponent<BoxCollider2D>();
            groundCollider.size = new Vector2(chunkSize, chunkSize);
            groundCollider.offset = new Vector2(chunkSize * 0.5f, chunkSize * 0.5f);
            groundCollider.isTrigger = true;
        }

        // Decor
        if (decorSprites != null && decorSprites.Length > 0 && decorMaxPerChunk > 0)
        {
            int count = Random.Range(decorMinPerChunk, decorMaxPerChunk + 1);
            float margin = 1.5f;
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
