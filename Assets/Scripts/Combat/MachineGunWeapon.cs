using UnityEngine;

/// <summary>
/// Machine gun weapon component. Fires projectiles at the nearest enemy within range.
/// Projectiles are drawn from a <see cref="ProjectilePool"/> for performance; the pool is
/// created automatically at Start if one isn't already assigned.
///
/// Stats (fire rate, damage, range, size) are read from the assigned <see cref="WeaponData"/>
/// ScriptableObject. The <see cref="SetLevel"/> method re-applies stats for the weapon's
/// current upgrade tier so the upgrade path drives all stat changes.
///
/// Spawns projectiles at the player center (via <see cref="GetPlayerPosition"/>)
/// so the weapon child object's local offset doesn't affect firing origin.
/// </summary>
public class MachineGunWeapon : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Header("Weapon Data")]
    [Tooltip("ScriptableObject that holds base stats and the projectile prefab.")]
    [SerializeField] private WeaponData data;

    [Header("Projectile Pool")]
    [Tooltip("Optional pre-existing pool. If null, one is created at Start from WeaponData.")]
    [SerializeField] private ProjectilePool projectilePool;

    [Tooltip("Number of projectiles to pre-instantiate when creating the pool.")]
    [SerializeField] private int poolInitialSize = 20;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Accumulated time since last shot (seconds).</summary>
    private float _timer;

    /// <summary>Current fire rate (shots per second). Higher = faster.</summary>
    private float _fireRate;

    /// <summary>Max distance to acquire a target (world units).</summary>
    private float _range;

    /// <summary>Damage dealt per projectile.</summary>
    private float _currentDamage;

    /// <summary>Scale multiplier applied to each projectile's transform.</summary>
    private float _size;

    /// <summary>Cached reference to the player (for spawn position).</summary>
    private Transform _playerTransform;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>
    /// Read base stats from WeaponData and cache the player transform.
    /// </summary>
    private void Awake()
    {
        // Cache player reference for GetPlayerPosition().
        var player = GetComponentInParent<PlayerController>();
        if (player != null)
            _playerTransform = player.transform;

        // Read base stats from assigned data; fall back to safe defaults.
        if (data != null)
        {
            _fireRate = data.FireRate;
            _range = data.Range;
            _currentDamage = data.Damage;
            _size = data.Size;
        }
        else
        {
            _fireRate = 1f;
            _range = 10f;
            _currentDamage = 1f;
            _size = 1f;
        }
    }

    /// <summary>
    /// Create a projectile pool if one wasn't assigned in the Inspector.
    /// </summary>
    private void Start()
    {
        if (data != null && data.ProjectilePrefab != null && projectilePool == null)
        {
            // Create a child GameObject to hold the pool component.
            var poolGo = new GameObject($"{data.name}_Pool");
            poolGo.transform.SetParent(transform);
            projectilePool = poolGo.AddComponent<ProjectilePool>();
            projectilePool.Init(data.ProjectilePrefab, poolInitialSize);
        }
    }

    /// <summary>
    /// Tick the fire timer; fire when the interval (1 / fireRate) has elapsed.
    /// </summary>
    private void Update()
    {
        // Can't fire without a pool.
        if (projectilePool == null) return;

        _timer += Time.deltaTime;

        // Convert fire rate (shots/sec) to interval (sec/shot).
        float interval = _fireRate > 0f ? 1f / _fireRate : 1f;

        if (_timer >= interval)
        {
            Fire();
            _timer = 0f;
        }
    }

    // ── Firing ────────────────────────────────────────────────────────

    /// <summary>
    /// Acquire the nearest enemy and launch a pooled projectile toward it.
    /// </summary>
    private void Fire()
    {
        EnemyHealth target = FindNearestEnemy();
        if (target == null) return;

        // Spawn at the player center, aim toward the target.
        Vector2 origin = GetPlayerPosition();
        Vector2 direction = ((Vector2)target.transform.position - origin).normalized;

        Projectile proj = projectilePool.Get();
        proj.transform.position = origin;
        proj.pool = projectilePool;
        proj.Initialize(direction, _currentDamage, _size);
    }

    /// <summary>
    /// Returns the player's world position so projectiles originate from the player center,
    /// not from this weapon child object's (potentially offset) position.
    /// Falls back to this transform if no player is found.
    /// </summary>
    private Vector2 GetPlayerPosition()
    {
        if (_playerTransform != null)
            return _playerTransform.position;
        return transform.position;
    }

    /// <summary>
    /// Scans all enemies and returns the closest one within <see cref="_range"/>.
    /// Uses the player position as the distance origin for consistency.
    /// Returns null if no enemy is in range.
    /// </summary>
    private EnemyHealth FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth closest = null;
        float minDist = _range;

        Vector2 origin = GetPlayerPosition();

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(origin, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    // ── Public stat modifiers (legacy, used by old UpgradeData) ──────

    /// <summary>Increase fire rate by a flat amount (clamped to 0.1 minimum).</summary>
    public void ModifyFireRate(float amount)
    {
        _fireRate = Mathf.Max(0.1f, _fireRate + amount);
    }

    /// <summary>Increase damage by a flat amount.</summary>
    public void ModifyDamage(float amount)
    {
        _currentDamage += amount;
    }

    /// <summary>Increase projectile size by a flat amount (clamped to 0.1 minimum).</summary>
    public void ModifySize(float amount)
    {
        _size = Mathf.Max(0.1f, _size + amount);
    }

    // ── Upgrade path ─────────────────────────────────────────────────

    /// <summary>
    /// Re-apply stats from the assigned <see cref="WeaponData"/> at the given level.
    /// Called by <see cref="WeaponManager"/> when the weapon is equipped or levelled up.
    /// Each level applies the cumulative tier multipliers defined in the data asset.
    /// </summary>
    public void SetLevel(int level)
    {
        if (data == null) return;

        _fireRate = data.GetFireRateAtLevel(level);
        _range = data.Range;
        _currentDamage = data.GetDamageAtLevel(level);
        _size = data.GetSizeAtLevel(level);
    }
}
