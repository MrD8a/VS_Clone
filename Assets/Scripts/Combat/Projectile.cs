using UnityEngine;

/// <summary>
/// A single projectile fired by <see cref="MachineGunWeapon"/>. Moves in a straight line,
/// damages the first enemy it hits, then returns to its <see cref="ProjectilePool"/>
/// (or self-destructs if no pool is assigned).
///
/// Projectiles are recycled via object pooling for performance. When returned to the pool
/// the GameObject is deactivated; when re-used, <see cref="Initialize"/> resets direction,
/// damage, scale, and lifetime.
/// </summary>
public class Projectile : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Travel speed in world units per second.")]
    [SerializeField] private float speed = 10f;

    [Tooltip("Base damage (overwritten by Initialize each shot).")]
    [SerializeField] private float damage = 1f;

    [Tooltip("Seconds before the projectile auto-returns to pool.")]
    [SerializeField] private float lifetime = 3f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Normalized travel direction set by <see cref="Initialize"/>.</summary>
    private Vector2 _direction;

    /// <summary>Remaining seconds before auto-return.</summary>
    private float _lifetimeRemaining;

    /// <summary>Pool this projectile belongs to (assigned by the weapon before Initialize).</summary>
    [HideInInspector] public ProjectilePool pool;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Current damage value.</summary>
    public float Damage => damage;

    // ── Initialization ────────────────────────────────────────────────

    /// <summary>
    /// Prepare the projectile for a new shot. Resets direction, damage, scale, and lifetime.
    /// Called by <see cref="MachineGunWeapon.Fire"/> after pulling from the pool.
    /// </summary>
    /// <param name="dir">Direction to travel (will be normalized).</param>
    /// <param name="damageAmount">Damage to deal on hit.</param>
    /// <param name="sizeScale">Scale multiplier (1 = default size).</param>
    public void Initialize(Vector2 dir, float damageAmount, float sizeScale = 1f)
    {
        _direction = dir.normalized;
        damage = damageAmount;
        _lifetimeRemaining = lifetime;
        transform.localScale = Vector3.one * Mathf.Max(0.1f, sizeScale);
    }

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>
    /// Move forward and count down the lifetime each frame.
    /// Returns to pool when lifetime expires.
    /// </summary>
    private void Update()
    {
        // Count down lifetime; return to pool when expired.
        _lifetimeRemaining -= Time.deltaTime;
        if (_lifetimeRemaining <= 0f)
        {
            ReturnOrDestroy();
            return;
        }

        // Move in the set direction at constant speed.
        transform.position += (Vector3)(_direction * speed * Time.deltaTime);
    }

    // ── Collision ─────────────────────────────────────────────────────

    /// <summary>
    /// On trigger collision with an enemy, deal damage and return to pool.
    /// The projectile only reacts to GameObjects with an <see cref="EnemyHealth"/> component.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            ReturnOrDestroy();
        }
    }

    // ── Pool return ───────────────────────────────────────────────────

    /// <summary>
    /// Return this projectile to its pool (deactivates the GameObject).
    /// If no pool is assigned, destroy the GameObject instead.
    /// </summary>
    private void ReturnOrDestroy()
    {
        if (pool != null)
            pool.ReturnToPool(this);
        else
            Destroy(gameObject);
    }
}
