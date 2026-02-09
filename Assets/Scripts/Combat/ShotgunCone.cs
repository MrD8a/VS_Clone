using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Shotgun instant-hit cone visual and damage handler.
/// Spawned by <see cref="ShotgunWeapon.Fire"/>; lives for a short duration, then self-destructs.
///
/// Uses a trigger <see cref="PolygonCollider2D"/> shaped as a wedge for hit detection.
/// The wedge path is computed in <see cref="Setup"/> from the cone angle and sprite bounds
/// so the hitbox closely matches the visible cone.
///
/// Each enemy is damaged only once per shot (tracked by <see cref="_hit"/>).
/// Both <see cref="OnTriggerEnter2D"/> and <see cref="OnTriggerStay2D"/> are used
/// so enemies already overlapping when the cone spawns still take damage.
///
/// Prefab requirements:
///   - PolygonCollider2D (trigger) — path is set by code.
///   - SpriteRenderer — cone pointing along local X (right).
///   - ShotgunCone component (this script).
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
public class ShotgunCone : MonoBehaviour
{
    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Damage dealt to each enemy that overlaps the cone.</summary>
    private float _damage;

    /// <summary>How long the cone stays active before being destroyed.</summary>
    private float _duration;

    /// <summary>Set of colliders already damaged this shot (prevents double-hits).</summary>
    private readonly HashSet<Collider2D> _hit = new HashSet<Collider2D>();

    // ── Setup ─────────────────────────────────────────────────────────

    /// <summary>
    /// Configure the cone's stats, scale, and collider wedge. Called by
    /// <see cref="ShotgunWeapon"/> immediately after instantiation.
    /// </summary>
    /// <param name="damage">Damage per enemy hit.</param>
    /// <param name="size">Scale multiplier applied to the prefab's base scale.</param>
    /// <param name="halfAngleDeg">Half-angle of the cone in degrees (e.g. 25 = 50° total).</param>
    /// <param name="duration">How long the cone persists (seconds).</param>
    /// <param name="spawnOffsetFromPlayer">Offset from player center (used for wedge apex).</param>
    public void Setup(float damage, float size, float halfAngleDeg, float duration, float spawnOffsetFromPlayer = 0f)
    {
        _damage = damage;
        _duration = duration;
        _hit.Clear();

        // Scale the cone by the Size stat (applied on top of the prefab's base scale).
        Vector3 baseScale = transform.localScale;
        transform.localScale = baseScale * size;

        // Build the wedge collider from the cone angle and the sprite's forward extent.
        float forwardExtent = GetSpriteForwardExtent();
        float radiusWorld = spawnOffsetFromPlayer + forwardExtent;
        float halfRad = halfAngleDeg * Mathf.Deg2Rad;
        float scaleX = Mathf.Max(0.001f, transform.lossyScale.x);

        // Apex sits behind the spawn point by the offset amount (in local space).
        Vector2 apex = new Vector2(-spawnOffsetFromPlayer / scaleX, 0f);
        float r = radiusWorld / scaleX;

        // Two outer points of the wedge: upper and lower edges at the half-angle.
        Vector2 upperEdge = new Vector2(Mathf.Cos(halfRad), Mathf.Sin(halfRad)) * r;
        Vector2 lowerEdge = new Vector2(Mathf.Cos(halfRad), -Mathf.Sin(halfRad)) * r;
        Vector2[] points = { apex, apex + upperEdge, apex + lowerEdge };

        // Apply the wedge path to the polygon collider.
        var poly = GetComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.SetPath(0, points);

        // Schedule self-destruction after the visual duration expires.
        Invoke(nameof(EndCone), duration);
    }

    // ── Sprite measurement ────────────────────────────────────────────

    /// <summary>
    /// Returns the forward extent (along local X) of the attached sprite in world units.
    /// Used to size the wedge collider so it matches the visible cone.
    /// Falls back to the lossy scale if no sprite is attached.
    /// </summary>
    private float GetSpriteForwardExtent()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.enabled)
        {
            Vector2 right = transform.right;
            Bounds b = sr.bounds;

            // Project bounds corners onto the forward axis to find the farthest point.
            float extent = Mathf.Max(
                Vector2.Dot((Vector2)(b.max - transform.position), right),
                Vector2.Dot((Vector2)(b.min - transform.position), right));
            return Mathf.Max(0f, extent);
        }

        return transform.lossyScale.x;
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

    /// <summary>Destroy this cone GameObject after its duration expires.</summary>
    private void EndCone()
    {
        Destroy(gameObject);
    }
}
