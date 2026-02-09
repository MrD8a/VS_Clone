using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Railgun instant-hit line visual and damage handler.
/// Spawned by <see cref="RailgunWeapon.Fire"/>; lives for a short duration, then self-destructs.
///
/// Uses a trigger <see cref="BoxCollider2D"/> for hit detection. The collider is set to
/// local size (1,1) and the transform scale is set to (length, width) so the hitbox
/// exactly matches the visible sprite.
///
/// Each enemy is damaged only once per shot (tracked by <see cref="_hit"/>).
/// Both <see cref="OnTriggerEnter2D"/> and <see cref="OnTriggerStay2D"/> are used
/// so enemies already overlapping when the line spawns still take damage.
///
/// Prefab requirements:
///   - BoxCollider2D (trigger) — size is set by code.
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

        // Scale the transform so the sprite fills (length x width) world units.
        transform.localScale = new Vector3(length, width, 1f);

        // Set collider to local (1,1) — under the scale above this becomes (length, width) in world space.
        var box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = Vector2.one;
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
