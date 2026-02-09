using UnityEngine;

/// <summary>
/// Defines how much contact damage this enemy deals to the player per tick.
/// Attach to each enemy prefab alongside <see cref="EnemyHealth"/>.
///
/// The enemy's collider should be set to trigger so it doesn't physically push the player.
/// <see cref="PlayerHealth"/> reads <see cref="DamagePerTick"/> from all overlapping enemies
/// at a fixed interval.
/// </summary>
public class EnemyContactDamage : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Damage dealt to the player per contact-damage tick.")]
    [SerializeField] private int damagePerTick = 1;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Damage dealt per contact tick.</summary>
    public int DamagePerTick => damagePerTick;
}
