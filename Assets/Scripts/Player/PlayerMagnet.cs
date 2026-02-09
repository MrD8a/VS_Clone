using UnityEngine;

/// <summary>
/// Pulls pickups with <see cref="MagnetPullable"/> toward the player when they are
/// within <see cref="magnetRange"/>.
///
/// Each frame, all active <see cref="MagnetPullable"/> objects are scanned. Those within
/// range are moved toward the player at <see cref="pullSpeed"/> (scaled by the pullable's
/// individual multiplier). The range can be increased via upgrades.
/// </summary>
public class PlayerMagnet : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Radius within which pickups are pulled toward the player (world units).")]
    [SerializeField] private float magnetRange = 3f;

    [Tooltip("Base speed at which pickups are pulled (world units per second).")]
    [SerializeField] private float pullSpeed = 8f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Cached transform for performance (avoids repeated property access).</summary>
    private Transform _transform;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Current magnet range (world units).</summary>
    public float MagnetRange => magnetRange;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Cache our own transform reference.</summary>
    private void Awake()
    {
        _transform = transform;
    }

    /// <summary>
    /// Each frame, find all active pullable objects and move those within range
    /// toward the player position.
    /// </summary>
    private void Update()
    {
        Vector2 playerPos = _transform.position;
        MagnetPullable[] pullables = FindObjectsByType<MagnetPullable>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (MagnetPullable pullable in pullables)
        {
            if (pullable == null) continue;

            Vector2 toPlayer = playerPos - (Vector2)pullable.transform.position;
            float dist = toPlayer.magnitude;

            // Skip if too far or already at the player position.
            if (dist <= 0f || dist > magnetRange) continue;

            // Move toward the player, clamped so we don't overshoot.
            float speed = pullSpeed * pullable.PullSpeedMultiplier * Time.deltaTime;
            pullable.transform.position += (Vector3)(toPlayer.normalized * Mathf.Min(speed, dist));
        }
    }

    // ── Public stat modifiers ─────────────────────────────────────────

    /// <summary>Adjust magnet range by a flat amount (for upgrades).</summary>
    public void ModifyMagnetRange(float amount)
    {
        magnetRange = Mathf.Max(0f, magnetRange + amount);
    }
}
