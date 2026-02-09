using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health bar displayed under the player. Only visible when the player has missing health.
/// Attach to the same GameObject as <see cref="PlayerHealth"/>.
///
/// Assign the bar root GameObject and a fill <see cref="Image"/> in the Inspector.
/// The fill amount is driven by <see cref="PlayerHealth.NormalizedHealth"/>.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealthBar : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Root GameObject of the health bar (shown/hidden based on health).")]
    [SerializeField] private GameObject barRoot;

    [Tooltip("Image whose fillAmount represents current health.")]
    [SerializeField] private Image fillImage;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Cached reference to the sibling PlayerHealth component.</summary>
    private PlayerHealth _health;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Cache the PlayerHealth reference on this GameObject.</summary>
    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// Update bar visibility and fill every frame (LateUpdate so health changes
    /// from Update are already applied).
    /// </summary>
    private void LateUpdate()
    {
        if (_health == null || barRoot == null || fillImage == null) return;

        // Show the bar only when health is below max and the player is alive.
        bool show = _health.CurrentHealth < _health.MaxHealth && _health.CurrentHealth > 0;
        barRoot.SetActive(show);
        fillImage.fillAmount = _health.NormalizedHealth;
    }
}
