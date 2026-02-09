using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic object pool for any <see cref="Component"/> type. Pre-instantiates a set of
/// deactivated objects; <see cref="Get"/> activates and returns one, <see cref="ReturnToPool"/>
/// deactivates it back.
///
/// Note: this generic pool is not currently used (weapons use <see cref="ProjectilePool"/>
/// instead). Kept for future use with other poolable objects.
/// </summary>
public class ObjectPool<T> : MonoBehaviour where T : Component
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Prefab to pool.")]
    [SerializeField] private T prefab;

    [Tooltip("Number of instances to pre-instantiate.")]
    [SerializeField] private int initialSize = 10;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Queue of deactivated objects ready for reuse.</summary>
    private readonly Queue<T> _objects = new();

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Fill the pool on Awake.</summary>
    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            _objects.Enqueue(obj);
        }
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Get an object from the pool (activates it). Creates a new one if the pool is empty.
    /// </summary>
    public T Get()
    {
        if (_objects.Count > 0)
        {
            T obj = _objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        // Pool exhausted; create a new instance.
        T newObj = Instantiate(prefab);
        return newObj;
    }

    /// <summary>
    /// Return an object to the pool (deactivates it).
    /// </summary>
    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }
}
