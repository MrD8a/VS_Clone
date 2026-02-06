using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 20;

    private Queue<Projectile> pool = new();

    private void Awake()
    {
        // Pre-instantiate projectiles
        for (int i = 0; i < initialSize; i++)
        {
            Projectile obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public Projectile Get()
    {
        // Remove any destroyed objects from the queue
        while (pool.Count > 0)
        {
            Projectile obj = pool.Dequeue();
            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // Pool exhausted, create new projectile
        Projectile newObj = Instantiate(prefab);
        return newObj;
    }

    public void ReturnToPool(Projectile obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
