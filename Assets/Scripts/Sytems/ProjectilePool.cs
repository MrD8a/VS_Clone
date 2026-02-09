using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 20;

    private Queue<Projectile> pool = new();
    private bool _initialized;

    private void Awake()
    {
        if (prefab != null)
            FillPool();
    }

    /// <summary>
    /// Initialize pool at runtime (e.g. when Weapon creates pool from WeaponData).
    /// No-op if already initialized.
    /// </summary>
    public void Init(Projectile projectilePrefab, int size = 20)
    {
        if (_initialized || projectilePrefab == null) return;
        prefab = projectilePrefab;
        initialSize = size;
        _initialized = true;
        FillPool();
    }

    private void FillPool()
    {
        if (prefab == null) return;
        _initialized = true;
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
