using UnityEngine;

/// <summary>
/// Tracks the player's experience points and level. When enough XP is accumulated,
/// the player levels up: time is paused and the <see cref="UpgradeUI"/> is shown
/// so the player can choose an upgrade.
///
/// XP is granted by collecting <see cref="XPOrb"/>s, which call <see cref="AddXP"/>.
/// Each level requires progressively more XP (scaled by 1.5× per level).
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Current player level (starts at 1).")]
    [SerializeField] private int currentLevel = 1;

    [Tooltip("XP accumulated toward the next level.")]
    [SerializeField] private int currentXP = 0;

    [Tooltip("XP required to reach the next level (increases each level).")]
    [SerializeField] private int xpToNextLevel = 5;

    // ── Public accessors (used by GameHUD) ────────────────────────────

    /// <summary>Current player level.</summary>
    public int CurrentLevel => currentLevel;

    /// <summary>XP accumulated toward the next level.</summary>
    public int CurrentXP => currentXP;

    /// <summary>XP required to reach the next level.</summary>
    public int XpToNextLevel => xpToNextLevel;

    /// <summary>XP progress toward next level as a 0–1 fraction (for the XP bar fill).</summary>
    public float NormalizedXP => xpToNextLevel > 0 ? Mathf.Clamp01((float)currentXP / xpToNextLevel) : 0f;

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Grant XP to the player. If the total reaches the threshold, trigger a level-up.
    /// </summary>
    /// <param name="amount">Amount of XP to add.</param>
    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
            LevelUp();
    }

    // ── Level-up ──────────────────────────────────────────────────────

    /// <summary>
    /// Increment the level, subtract the spent XP, increase the next threshold,
    /// pause the game, and show the upgrade selection UI.
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        // Pause the game while the player picks an upgrade.
        Time.timeScale = 0f;
        UpgradeUI.Instance.Show();
    }
}
