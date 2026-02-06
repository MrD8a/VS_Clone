using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;

    [HideInInspector] public ProjectilePool pool;

    public int Damage => damage;

    private float lifetimeRemaining;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Initialize the projectile for a shot. Use this when spawning from pool so lifetime returns to pool instead of destroying.
    /// </summary>
    public void Initialize(Vector2 dir, int damageAmount)
    {
        direction = dir.normalized;
        damage = damageAmount;
        lifetimeRemaining = lifetime;
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