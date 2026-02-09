using UnityEngine;

/// <summary>
/// Locks the camera to the target (typically the player). Supports optional smooth follow.
///
/// The Z position is preserved from the camera's initial position so the 2D camera
/// stays at the correct depth. The target is found by tag ("Player") if not assigned.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Transform to follow (found by tag if not assigned).")]
    [SerializeField] private Transform target;

    [Tooltip("Smooth damp time in seconds (lower = snappier).")]
    [SerializeField] private float smoothTime = 0.15f;

    [Tooltip("Whether to use SmoothDamp or snap directly to the target.")]
    [SerializeField] private bool useSmoothing = true;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Velocity ref for SmoothDamp.</summary>
    private Vector3 _velocity = Vector3.zero;

    /// <summary>Initial Z depth of the camera (preserved during follow).</summary>
    private float _zPosition;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Store the initial Z and find the player if no target is assigned.</summary>
    private void Awake()
    {
        _zPosition = transform.position.z;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    /// <summary>
    /// Follow the target in LateUpdate so the camera moves after all other
    /// position updates have been applied.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 goal = new Vector3(target.position.x, target.position.y, _zPosition);

        if (useSmoothing && smoothTime > 0f)
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref _velocity, smoothTime);
        else
            transform.position = goal;
    }
}
