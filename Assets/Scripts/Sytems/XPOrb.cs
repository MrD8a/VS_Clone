using UnityEngine;

/// <summary>
/// An XP pickup dropped by enemies on death. When the player's trigger collider
/// overlaps this orb, it grants XP via <see cref="PlayerExperience.AddXP"/> and
/// destroys itself.
///
/// Should have a <see cref="MagnetPullable"/> component so the player's magnet
/// can pull it within collection range.
/// </summary>
public class XPOrb : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Amount of XP granted to the player on collection.")]
    [SerializeField] private int xpValue = 1;

    // ── Trigger callback ──────────────────────────────────────────────

    /// <summary>
    /// When the player's trigger overlaps, grant XP and destroy this orb.
    /// The player is identified by the "Player" tag.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerExperience xp = other.GetComponent<PlayerExperience>();
            if (xp != null)
                xp.AddXP(xpValue);

            Destroy(gameObject);
        }
    }
}
