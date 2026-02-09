using UnityEngine;

/// <summary>
/// Fires an instant, piercing line toward the nearest enemy. All enemies in the line take damage once.
/// Requires WeaponData with LinePrefab assigned. Size stat = line width.
/// </summary>
public class RailgunWeapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;

    private float _timer;
    private float _fireRate;
    private float _range;
    private float _currentDamage;
    private float _size;

    private void Awake()
    {
        if (data == null) return;
        _fireRate = data.FireRate;
        _range = data.Range;
        _currentDamage = data.Damage;
        _size = data.Size;
    }

    private void Update()
    {
        if (data?.LinePrefab == null) return;

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

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float offset = data.SpawnOffsetFromPlayer;
        Vector2 lineCenter = (Vector2)transform.position + direction * (offset + data.LineLength * 0.5f);

        GameObject lineGo = Instantiate(data.LinePrefab, lineCenter, Quaternion.identity);
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        lineGo.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (lineGo.TryGetComponent(out RailgunLine line))
            line.Setup(_currentDamage, _size, data.LineLength, data.LineDuration, data.SpawnOffsetFromPlayer);
        else
            Destroy(lineGo);
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
