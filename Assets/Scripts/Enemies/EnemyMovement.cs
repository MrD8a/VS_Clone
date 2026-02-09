using UnityEngine;

/// <summary>
/// Moves this enemy toward the player every physics frame.
/// Requires a <see cref="Rigidbody2D"/> (added automatically via RequireComponent).
///
/// The player is located by tag ("Player") at Start. If the player is destroyed,
/// the enemy stops moving.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Movement speed in world units per second.")]
    [SerializeField] private float moveSpeed = 2f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Physics body used for movement.</summary>
    private Rigidbody2D _rb;

    /// <summary>Cached reference to the player's transform.</summary>
    private Transform _player;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Cache the Rigidbody2D.</summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>Find the player by tag.</summary>
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// Move toward the player at a constant speed each physics frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (_player == null) return;

        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;
    }
}
