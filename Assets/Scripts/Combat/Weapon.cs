using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float range = 10f;

    private float timer;
    private int currentDamage;

    private void Awake()
    {
        currentDamage = projectilePrefab.Damage;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            Fire();
            timer = 0f;
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
        proj.Initialize(direction, currentDamage);
    }

    private EnemyHealth FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth closest = null;
        float minDist = range;

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

    private void OnLevelUp()
    {
        cooldown = Mathf.Max(0.1f, cooldown * 0.9f);
    }

    public void ModifyCooldown(float amount)
    {
        cooldown = Mathf.Max(0.1f, cooldown - amount);
    }

    public void ModifyDamage(int amount)
    {
        currentDamage += amount;
    }

}
