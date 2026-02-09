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
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float range = 10f;

    private float _timer;
    private float _cooldown;
    private float _range;
    private float _currentDamage;
    private float _size;

    private void Awake()
    {
        if (data != null)
        {
            _cooldown = data.Cooldown;
            _range = data.Range;
            _currentDamage = data.Damage;
            _size = data.Size;
        }
        else
        {
            _cooldown = cooldown;
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
        if (_timer >= _cooldown)
        {
            Fire();
            _timer = 0f;
        }
    }

    private void Fire()
    {
        EnemyHealth target = FindNearestEnemy();
        if (target == null) return;

        Vector2 direction = (target.transform.position - transform.position).normalized;

        Projectile proj = projectilePool.Get();
        proj.transform.position = transform.position;
        proj.pool = projectilePool;
        proj.Initialize(direction, _currentDamage, _size);
    }

    private EnemyHealth FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth closest = null;
        float minDist = _range;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    public void ModifyCooldown(float amount)
    {
        _cooldown = Mathf.Max(0.1f, _cooldown - amount);
    }

    public void ModifyDamage(float amount)
    {
        _currentDamage += amount;
    }

    public void ModifySize(float amount)
    {
        _size = Mathf.Max(0.1f, _size + amount);
    }
}
