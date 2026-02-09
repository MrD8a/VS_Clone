using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player health system with passive regeneration and contact damage from overlapping enemies.
///
/// Requires a <see cref="Collider2D"/> set to trigger on this GameObject to detect enemy overlap.
/// Enemies should NOT physically collide with the player; their colliders should also be triggers.
///
/// Contact damage is applied at a fixed interval (<see cref="contactDamageInterval"/>) by
/// summing <see cref="EnemyContactDamage.DamagePerTick"/> from all currently overlapping enemies.
///
/// When health reaches zero, <see cref="GameOverUI.Show"/> is called and time is paused.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Header("Health")]
    [Tooltip("Maximum health points.")]
    [SerializeField] private int maxHealth = 10;

    [Tooltip("Health regenerated per second (passive).")]
    [SerializeField] private float healthRegenPerSecond = 0.5f;

    [Header("Contact Damage")]
    [Tooltip("Seconds between contact-damage ticks from overlapping enemies.")]
    [SerializeField] private float contactDamageInterval = 0.25f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Current health (float to support fractional regen).</summary>
    private float _currentHealth;

    /// <summary>Countdown to the next contact-damage tick.</summary>
    private float _contactDamageTimer;

    /// <summary>Set of enemies currently overlapping the player's trigger.</summary>
    private readonly HashSet<EnemyContactDamage> _overlappingEnemies = new();

    /// <summary>Whether the player is dead (prevents duplicate death logic).</summary>
    private bool _isDead;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Current health value.</summary>
    public float CurrentHealth => _currentHealth;

    /// <summary>Maximum health value.</summary>
    public int MaxHealth => maxHealth;

    /// <summary>Health as a 0–1 fraction (for health bar fill).</summary>
    public float NormalizedHealth => maxHealth > 0 ? Mathf.Clamp01(_currentHealth / maxHealth) : 0f;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Ensure the collider is a trigger and set health to max.</summary>
    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;

        _currentHealth = maxHealth;
    }

    /// <summary>
    /// Each frame: regenerate health, count down the contact-damage timer,
    /// and apply accumulated contact damage when the timer fires.
    /// </summary>
    private void Update()
    {
        if (_isDead) return;

        // Passive regeneration (capped at max health).
        _currentHealth = Mathf.Min(_currentHealth + healthRegenPerSecond * Time.deltaTime, maxHealth);

        // Contact-damage tick.
        _contactDamageTimer -= Time.deltaTime;
        if (_contactDamageTimer <= 0f)
        {
            _contactDamageTimer = contactDamageInterval;
            ApplyContactDamage();
        }
    }

    // ── Contact damage ────────────────────────────────────────────────

    /// <summary>
    /// Sum damage from all overlapping enemies and apply it as a single hit.
    /// Removes null (destroyed) entries from the set.
    /// </summary>
    private void ApplyContactDamage()
    {
        int totalDamage = 0;
        var toRemove = new List<EnemyContactDamage>();

        foreach (var enemy in _overlappingEnemies)
        {
            if (enemy == null)
            {
                toRemove.Add(enemy);
                continue;
            }
            totalDamage += enemy.DamagePerTick;
        }

        // Clean up destroyed enemies.
        foreach (var e in toRemove)
            _overlappingEnemies.Remove(e);

        if (totalDamage > 0)
            TakeDamage(totalDamage);
    }

    // ── Damage / death ────────────────────────────────────────────────

    /// <summary>Reduce health by the given amount. Triggers death if health hits zero.</summary>
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        if (_currentHealth <= 0)
            Die();
    }

    /// <summary>Pause the game and show the game over screen.</summary>
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Time.timeScale = 0f;
        GameOverUI.Instance?.Show();
    }

    // ── Public stat modifiers ─────────────────────────────────────────

    /// <summary>Adjust max health (for upgrades). Clamps current health to the new max.</summary>
    public void ModifyMaxHealth(int delta)
    {
        maxHealth = Mathf.Max(1, maxHealth + delta);
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
    }

    /// <summary>Adjust passive regen rate (for upgrades).</summary>
    public void ModifyRegenPerSecond(float delta)
    {
        healthRegenPerSecond = Mathf.Max(0f, healthRegenPerSecond + delta);
    }

    // ── Trigger callbacks ─────────────────────────────────────────────

    /// <summary>Track enemies that enter the player's trigger zone.</summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyContactDamage enemy))
            _overlappingEnemies.Add(enemy);
    }

    /// <summary>Remove enemies that leave the player's trigger zone.</summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyContactDamage enemy))
            _overlappingEnemies.Remove(enemy);
    }
}
