using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// In-game heads-up display. Updates the XP bar fill and the elapsed-time timer text.
///
/// Uses <see cref="GameTimer"/> as the single source of truth for elapsed time so the HUD
/// and spawn phases stay in sync. Falls back to <c>Time.time</c> if no timer is assigned.
/// </summary>
public class GameHUD : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Image whose fillAmount represents XP progress toward next level.")]
    [SerializeField] private Image xpBarFill;

    [Tooltip("Text element showing the elapsed game time as MM:SS.")]
    [SerializeField] private TMP_Text timerText;

    [Tooltip("Reference to the GameTimer (found automatically if not assigned).")]
    [SerializeField] private GameTimer gameTimer;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Cached reference to the player's XP tracker.</summary>
    private PlayerExperience _playerExperience;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Find references that weren't assigned in the Inspector.</summary>
    private void Start()
    {
        _playerExperience = FindFirstObjectByType<PlayerExperience>();

        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();
    }

    /// <summary>Update the timer text and XP bar each frame.</summary>
    private void Update()
    {
        // Timer display: MM:SS format.
        float elapsed = gameTimer != null ? gameTimer.ElapsedTime : Time.time;
        if (timerText != null)
            timerText.text = $"{Mathf.FloorToInt(elapsed / 60f):D2}:{Mathf.FloorToInt(elapsed % 60f):D2}";

        // XP bar fill.
        if (xpBarFill != null && _playerExperience != null)
            xpBarFill.fillAmount = _playerExperience.NormalizedXP;
    }
}
