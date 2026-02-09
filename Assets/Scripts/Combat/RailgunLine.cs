using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Railgun instant-hit line. Uses a trigger BoxCollider2D for hit detection; configure size in Setup.
/// Prefab: add a BoxCollider2D (trigger). Sprite length along local X, width along local Y.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RailgunLine : MonoBehaviour
{
    private float _damage;
    private float _width;
    private float _length;
    private float _spawnOffset;
    private float _duration;
    private readonly HashSet<Collider2D> _hit = new HashSet<Collider2D>();

    public void Setup(float damage, float width, float length, float duration, float spawnOffsetFromPlayer = 0f)
    {
        _damage = damage;
        _width = width;
        _length = length;
        _spawnOffset = spawnOffsetFromPlayer;
        _duration = duration;
        _hit.Clear();

        transform.localScale = new Vector3(length, width, 1f);

        var box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        // Hitbox exactly matches visible line: local (1,1) under scale (length, width) = world size length x width, centered.
        box.size = Vector2.one;
        box.offset = Vector2.zero;

        Invoke(nameof(EndLine), duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (_hit.Contains(other)) return;
        if (!other.TryGetComponent(out EnemyHealth enemy)) return;

        _hit.Add(other);
        enemy.TakeDamage(_damage);
    }

    private void EndLine()
    {
        Destroy(gameObject);
    }
}
