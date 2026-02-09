using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Game over panel displayed when the player dies. Shows a "New Game" button
/// that reloads the current scene.
///
/// Follows the same singleton + panel pattern as <see cref="PauseUI"/> and
/// <see cref="UpgradeUI"/>. <see cref="PlayerHealth.Die"/> calls <see cref="Show"/>.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────

    /// <summary>Global instance so other scripts can call Show() and check PanelActive.</summary>
    public static GameOverUI Instance { get; private set; }

    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Panel root; activated to show the game over screen.")]
    [SerializeField] private GameObject panel;

    [Tooltip("Button that starts a new game (reloads the scene).")]
    [SerializeField] private Button newGameButton;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Register singleton and wire up the button listener.</summary>
    private void Awake()
    {
        Instance = this;

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
    }

    /// <summary>Clear singleton reference when destroyed.</summary>
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Whether the game over panel is currently visible.</summary>
    public bool PanelActive => panel != null && panel.activeSelf;

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>Show the game over panel.</summary>
    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    // ── Button handler ────────────────────────────────────────────────

    /// <summary>Resume time and reload the current scene to start a new game.</summary>
    private void OnNewGameClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
