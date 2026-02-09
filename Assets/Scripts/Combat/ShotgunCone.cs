using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Shotgun instant-hit cone. Uses a trigger PolygonCollider2D (wedge) for hit detection.
/// Prefab: add a PolygonCollider2D (trigger). Cone forward = local X. Script sets the wedge shape in Setup.
/// Wedge extent is taken from the sprite bounds so the hitbox matches the visible cone.
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
public class ShotgunCone : MonoBehaviour
{
    private float _damage;
    private float _duration;
    private readonly HashSet<Collider2D> _hit = new HashSet<Collider2D>();

    public void Setup(float damage, float size, float halfAngleDeg, float duration, float spawnOffsetFromPlayer = 0f)
    {
        _damage = damage;
        _duration = duration;
        _hit.Clear();

        Vector3 baseScale = transform.localScale;
        transform.localScale = baseScale * size;

        float forwardExtent = GetSpriteForwardExtent();
        float radiusWorld = spawnOffsetFromPlayer + forwardExtent;
        float halfRad = halfAngleDeg * Mathf.Deg2Rad;
        float s = Mathf.Max(0.001f, transform.lossyScale.x);

        Vector2 apex = new Vector2(-spawnOffsetFromPlayer / s, 0f);
        float r = radiusWorld / s;
        Vector2 right = new Vector2(Mathf.Cos(halfRad), Mathf.Sin(halfRad)) * r;
        Vector2 left = new Vector2(Mathf.Cos(halfRad), -Mathf.Sin(halfRad)) * r;
        Vector2[] points = { apex, apex + right, apex + left };

        var poly = GetComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.SetPath(0, points);

        Invoke(nameof(EndCone), duration);
    }

    private float GetSpriteForwardExtent()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.enabled)
        {
            Vector2 right = transform.right;
            Bounds b = sr.bounds;
            float extent = Mathf.Max(
                Vector2.Dot((Vector2)(b.max - transform.position), right),
                Vector2.Dot((Vector2)(b.min - transform.position), right));
            return Mathf.Max(0f, extent);
        }
        return transform.lossyScale.x;
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

    private void EndCone()
    {
        Destroy(gameObject);
    }
}
