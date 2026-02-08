using UnityEngine;

/// <summary>
/// Attach to any pickup (XP orbs, future items) that should be pulled toward the player when in magnet range.
/// The magnet system finds all objects with this component within range and moves them toward the player.
/// </summary>
public class MagnetPullable : MonoBehaviour
{
    [Tooltip("Multiplier for pull speed (1 = default). Use for heavier/slower pickups later.")]
    [SerializeField] private float pullSpeedMultiplier = 1f;

    public float PullSpeedMultiplier => pullSpeedMultiplier;
}
