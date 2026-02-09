using UnityEngine;

/// <summary>
/// Enemy health component. Tracks hit points and spawns an XP orb on death.
///
/// Weapons deal damage by calling <see cref="TakeDamage"/>. When health reaches zero
/// the enemy is destroyed and an <see cref="XPOrb"/> prefab is instantiated at its position.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Maximum (and starting) health.")]
    [SerializeField] private int maxHealth = 3;

    [Tooltip("XP orb prefab to spawn on death.")]
    [SerializeField] private GameObject xpOrbPrefab;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Current health (float to support fractional weapon damage).</summary>
    private float _currentHealth;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Set health to max on spawn.</summary>
    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Reduce health by <paramref name="damage"/>. Destroys the enemy and spawns
    /// an XP orb if health reaches zero.
    /// </summary>
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            // Spawn XP orb at the enemy's position before destroying.
            if (xpOrbPrefab != null)
                Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
