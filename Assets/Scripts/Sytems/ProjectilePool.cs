using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pool specialized for <see cref="Projectile"/> instances.
/// Pre-instantiates a set of deactivated projectiles; <see cref="Get"/> activates and
/// returns one, <see cref="ReturnToPool"/> deactivates it back.
///
/// Can be initialized either via Inspector fields (prefab + initialSize) or at runtime
/// by calling <see cref="Init"/> (used by <see cref="MachineGunWeapon"/> when creating
/// the pool from WeaponData).
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Projectile prefab to instantiate.")]
    [SerializeField] private Projectile prefab;

    [Tooltip("Number of projectiles to pre-instantiate.")]
    [SerializeField] private int initialSize = 20;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Queue of deactivated projectiles ready for reuse.</summary>
    private readonly Queue<Projectile> _pool = new();

    /// <summary>Whether the pool has already been filled (prevents double-init).</summary>
    private bool _initialized;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>
    /// If a prefab is assigned in the Inspector, fill the pool on Awake.
    /// </summary>
    private void Awake()
    {
        if (prefab != null)
            FillPool();
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Initialize the pool at runtime (e.g. when MachineGunWeapon creates a pool from WeaponData).
    /// No-op if already initialized.
    /// </summary>
    /// <param name="projectilePrefab">Prefab to pool.</param>
    /// <param name="size">Number of instances to pre-instantiate.</param>
    public void Init(Projectile projectilePrefab, int size = 20)
    {
        if (_initialized || projectilePrefab == null) return;

        prefab = projectilePrefab;
        initialSize = size;
        _initialized = true;
        FillPool();
    }

    /// <summary>
    /// Get a projectile from the pool. Activates and returns it.
    /// If the pool is empty (or contains destroyed objects), instantiates a new one.
    /// </summary>
    public Projectile Get()
    {
        // Skip any destroyed objects that might still be in the queue.
        while (_pool.Count > 0)
        {
            Projectile obj = _pool.Dequeue();
            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // Pool exhausted; create a new instance on the fly.
        Projectile newObj = Instantiate(prefab);
        return newObj;
    }

    /// <summary>
    /// Return a projectile to the pool (deactivates the GameObject).
    /// </summary>
    public void ReturnToPool(Projectile obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    // ── Private helper ────────────────────────────────────────────────

    /// <summary>
    /// Pre-instantiate <see cref="initialSize"/> deactivated projectiles.
    /// </summary>
    private void FillPool()
    {
        if (prefab == null) return;
        _initialized = true;

        for (int i = 0; i < initialSize; i++)
        {
            Projectile obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
