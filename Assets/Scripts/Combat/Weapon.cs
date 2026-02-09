using UnityEngine;

/// <summary>
/// Fires at nearest enemy in range. Stats and prefab come from WeaponData when assigned;
/// otherwise uses legacy serialized fields so existing scenes keep working.
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;

    [Header("Optional: use when data is set and you want a pre-existing pool")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private int poolInitialSize = 20;

    [Header("Legacy (used only when data is null)")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float range = 10f;

    private float _timer;
    private float _fireRate;
    private float _range;
    private float _currentDamage;
    private float _size;

    private void Awake()
    {
        if (data != null)
        {
            _fireRate = data.FireRate;
            _range = data.Range;
            _currentDamage = data.Damage;
            _size = data.Size;
        }
        else
        {
            _fireRate = fireRate;
            _range = range;
            _currentDamage = projectilePrefab != null ? projectilePrefab.Damage : 1f;
            _size = 1f;
        }
    }

    private void Start()
    {
        if (data != null && data.ProjectilePrefab != null && projectilePool == null)
        {
            var poolGo = new GameObject($"{data.name}_Pool");
            poolGo.transform.SetParent(transform);
            projectilePool = poolGo.AddComponent<ProjectilePool>();
            projectilePool.Init(data.ProjectilePrefab, poolInitialSize);
        }
    }

    private void Update()
    {
        if (projectilePool == null) return;

        _timer += Time.deltaTime;
        float interval = _fireRate > 0f ? 1f / _fireRate : 1f;
        if (_timer >= interval)
        {
            Fire();
            _timer = 0f;
        }
    }

    private void Fire()
    {
        EnemyHealth target = FindNearestEnemy();
        if (target == null) return;

        Vector2 spawnPosition = GetSpawnPosition();
        Vector2 direction = (target.transform.position - (Vector3)spawnPosition).normalized;

        Projectile proj = projectilePool.Get();
        proj.transform.position = spawnPosition;
        proj.pool = projectilePool;
        proj.Initialize(direction, _currentDamage, _size);
    }

    /// <summary>
    /// Spawn at player center so projectiles don't inherit the weapon child's position offset.
    /// </summary>
    private Vector2 GetSpawnPosition()
    {
        var player = GetComponentInParent<PlayerController>();
        if (player != null)
            return player.transform.position;
        return transform.position;
    }

    private EnemyHealth FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth closest = null;
        float minDist = _range;

        Vector2 origin = GetSpawnPosition();
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

    public void ModifyFireRate(float amount)
    {
        _fireRate = Mathf.Max(0.1f, _fireRate + amount);
    }

    public void ModifyDamage(float amount)
    {
        _currentDamage += amount;
    }

    public void ModifySize(float amount)
    {
        _size = Mathf.Max(0.1f, _size + amount);
    }

    /// <summary>
    /// Apply stats from assigned WeaponData at the given level (for item upgrade path).
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
