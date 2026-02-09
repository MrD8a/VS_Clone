using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Railgun instant-hit line visual and damage handler.
/// Spawned by <see cref="RailgunWeapon.Fire"/>; lives for a short duration, then self-destructs.
///
/// Uses a trigger <see cref="BoxCollider2D"/> for hit detection. The collider is sized to
/// match the sprite's local bounds, and the transform scale is computed relative to those
/// bounds so that **both** the sprite and collider end up at exactly (length x width) in
/// world space, regardless of the sprite's native pixel dimensions / PPU.
///
/// Each enemy is damaged only once per shot (tracked by <see cref="_hit"/>).
/// Both <see cref="OnTriggerEnter2D"/> and <see cref="OnTriggerStay2D"/> are used
/// so enemies already overlapping when the line spawns still take damage.
///
/// Prefab requirements:
///   - BoxCollider2D (trigger) — size is overridden by code.
///   - SpriteRenderer — sprite length along local X, width along local Y.
///   - RailgunLine component (this script).
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RailgunLine : MonoBehaviour
{
    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Damage dealt to each enemy that overlaps the line.</summary>
    private float _damage;

    /// <summary>Visual width of the line (set from weapon Size stat).</summary>
    private float _width;

    /// <summary>Visual length of the line (set from WeaponData.LineLength).</summary>
    private float _length;

    /// <summary>Offset from player center (currently unused after hitbox simplification).</summary>
    private float _spawnOffset;

    /// <summary>How long the line stays active before being destroyed.</summary>
    private float _duration;

    /// <summary>Set of colliders already damaged this shot (prevents double-hits).</summary>
    private readonly HashSet<Collider2D> _hit = new HashSet<Collider2D>();

    // ── Setup ─────────────────────────────────────────────────────────

    /// <summary>
    /// Configure the line's stats, scale, and collider. Called by <see cref="RailgunWeapon"/>
    /// immediately after instantiation.
    /// </summary>
    /// <param name="damage">Damage per enemy hit.</param>
    /// <param name="width">Line width (from Size stat).</param>
    /// <param name="length">Line length (from WeaponData.LineLength).</param>
    /// <param name="duration">How long the line persists (seconds).</param>
    /// <param name="spawnOffsetFromPlayer">Offset from player center (informational).</param>
    public void Setup(float damage, float width, float length, float duration, float spawnOffsetFromPlayer = 0f)
    {
        _damage = damage;
        _width = width;
        _length = length;
        _spawnOffset = spawnOffsetFromPlayer;
        _duration = duration;
        _hit.Clear();

        // ── Compute scale relative to the sprite's actual local dimensions ──
        // The sprite's local size (before any scaling) is determined by its pixel
        // dimensions and Pixels Per Unit. We must scale relative to those dimensions
        // so both the visual and the collider end up at exactly (length x width)
        // in world space.
        Vector2 spriteLocalSize = Vector2.one; // safe fallback
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            spriteLocalSize = sr.sprite.bounds.size;

        transform.localScale = new Vector3(
            length / spriteLocalSize.x,
            width  / spriteLocalSize.y,
            1f);

        // Size the collider to the sprite's local bounds so that under the scale
        // computed above, the world-space collider = (length x width) — exactly
        // matching the visible sprite.
        var box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size   = spriteLocalSize;
        box.offset = Vector2.zero;

        // Schedule self-destruction after the visual duration expires.
        Invoke(nameof(EndLine), duration);
    }

    // ── Trigger callbacks ─────────────────────────────────────────────

    /// <summary>Called when a new collider enters the trigger.</summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    /// <summary>Called every physics frame for colliders already inside the trigger.</summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    /// <summary>
    /// Attempt to damage the enemy attached to <paramref name="other"/>.
    /// Each enemy is damaged only once per shot.
    /// </summary>
    private void TryDamage(Collider2D other)
    {
        if (_hit.Contains(other)) return;
        if (!other.TryGetComponent(out EnemyHealth enemy)) return;

        _hit.Add(other);
        enemy.TakeDamage(_damage);
    }

    // ── Cleanup ───────────────────────────────────────────────────────

    /// <summary>Destroy this line GameObject after its duration expires.</summary>
    private void EndLine()
    {
        Destroy(gameObject);
    }
}
