using UnityEngine;

/// <summary>
/// Marks a pickup (XP orb, future items) as pullable by the <see cref="PlayerMagnet"/>.
/// The magnet system finds all active objects with this component and moves those within
/// range toward the player.
///
/// <see cref="PullSpeedMultiplier"/> allows individual pickups to be pulled faster or
/// slower than the default speed (e.g. heavier items move slower).
/// </summary>
public class MagnetPullable : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Multiplier for pull speed (1 = default). Use < 1 for heavier pickups.")]
    [SerializeField] private float pullSpeedMultiplier = 1f;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Speed multiplier applied when the magnet pulls this pickup.</summary>
    public float PullSpeedMultiplier => pullSpeedMultiplier;
}
