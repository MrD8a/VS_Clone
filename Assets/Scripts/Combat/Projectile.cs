using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;

    [HideInInspector] public ProjectilePool pool;

    public float Damage => damage;

    private float lifetimeRemaining;

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Initialize the projectile for a shot. Use this when spawning from pool so lifetime returns to pool instead of destroying.
    /// </summary>
    /// <param name="sizeScale">Scale multiplier for the projectile (1 = default size).</param>
    public void Initialize(Vector2 dir, float damageAmount, float sizeScale = 1f)
    {
        direction = dir.normalized;
        damage = damageAmount;
        lifetimeRemaining = lifetime;
        transform.localScale = Vector3.one * Mathf.Max(0.1f, sizeScale);
    }

    private void Update()
    {
        lifetimeRemaining -= Time.deltaTime;
        if (lifetimeRemaining <= 0f)
        {
            ReturnOrDestroy();
            return;
        }
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void ReturnOrDestroy()
    {
        if (pool != null)
            pool.ReturnToPool(this);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            ReturnOrDestroy();
        }
    }

}