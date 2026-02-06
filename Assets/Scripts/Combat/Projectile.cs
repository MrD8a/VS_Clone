using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;

    [HideInInspector] public ProjectilePool pool;

    public int Damage => damage;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);

            if (pool != null)
                pool.ReturnToPool(this);
            else
                Destroy(gameObject);
        }
    }

}