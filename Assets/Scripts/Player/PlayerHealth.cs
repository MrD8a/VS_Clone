using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player health with passive regen and contact damage from overlapping enemies at a fixed interval.
/// Requires a Collider2D set to trigger to detect enemy overlap. Enemies should not physically collide with the player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float healthRegenPerSecond = 0.5f;

    [Header("Contact damage (from overlapping enemies)")]
    [SerializeField] private float contactDamageInterval = 0.25f;

    private float _currentHealth;
    private float _contactDamageTimer;
    private readonly HashSet<EnemyContactDamage> _overlappingEnemies = new();
    private bool _isDead;

    public float CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public float NormalizedHealth => maxHealth > 0 ? Mathf.Clamp01(_currentHealth / maxHealth) : 0f;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;

        _currentHealth = maxHealth;
    }

    private void Update()
    {
        if (_isDead) return;

        _currentHealth = Mathf.Min(_currentHealth + healthRegenPerSecond * Time.deltaTime, maxHealth);

        _contactDamageTimer -= Time.deltaTime;
        if (_contactDamageTimer <= 0f)
        {
            _contactDamageTimer = contactDamageInterval;
            ApplyContactDamage();
        }
    }

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
        foreach (var e in toRemove)
            _overlappingEnemies.Remove(e);

        if (totalDamage > 0)
            TakeDamage(totalDamage);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        if (_currentHealth <= 0)
            Die();
    }

    public void ModifyMaxHealth(int delta)
    {
        maxHealth = Mathf.Max(1, maxHealth + delta);
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
    }

    public void ModifyRegenPerSecond(float delta)
    {
        healthRegenPerSecond = Mathf.Max(0f, healthRegenPerSecond + delta);
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Time.timeScale = 0f;
        GameOverUI.Instance?.Show();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyContactDamage enemy))
            _overlappingEnemies.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyContactDamage enemy))
            _overlappingEnemies.Remove(enemy);
    }
}
