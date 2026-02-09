using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Pause menu that toggles with the Escape key (new Input System).
/// Shows a panel with Resume, Restart, and Quit buttons.
///
/// Pause is blocked while <see cref="GameOverUI"/> or <see cref="UpgradeUI"/> panels
/// are active so the player can't open overlapping menus.
///
/// Uses the same singleton + panel pattern as <see cref="GameOverUI"/>.
/// </summary>
public class PauseUI : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────

    /// <summary>Global instance so other scripts can check PanelActive.</summary>
    public static PauseUI Instance { get; private set; }

    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Panel root; activated/deactivated to show/hide the pause menu.")]
    [SerializeField] private GameObject panel;

    [Tooltip("Button that resumes gameplay.")]
    [SerializeField] private Button resumeButton;

    [Tooltip("Button that restarts the current scene.")]
    [SerializeField] private Button restartButton;

    [Tooltip("Button that quits the application.")]
    [SerializeField] private Button quitButton;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Register singleton and wire up button listeners.</summary>
    private void Awake()
    {
        Instance = this;

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    /// <summary>Clear singleton reference when destroyed.</summary>
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Check for Escape key each frame to toggle the pause menu.</summary>
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    // ── Toggle logic ──────────────────────────────────────────────────

    /// <summary>
    /// Toggle between paused and unpaused. Blocked if another overlay is active.
    /// </summary>
    private void TogglePause()
    {
        if (IsBlocked()) return;

        if (panel != null && panel.activeSelf)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Returns true if another overlay panel (game over, upgrade) is active,
    /// preventing the pause menu from opening on top of it.
    /// </summary>
    private bool IsBlocked()
    {
        if (GameOverUI.Instance != null && GameOverUI.Instance.PanelActive)
            return true;
        if (UpgradeUI.Instance != null && UpgradeUI.Instance.PanelActive)
            return true;
        return false;
    }

    // ── Pause / resume ────────────────────────────────────────────────

    /// <summary>Freeze time and show the pause panel.</summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);
    }

    /// <summary>Resume time and hide the pause panel.</summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }

    // ── Button handlers ───────────────────────────────────────────────

    /// <summary>Resume time and reload the current scene.</summary>
    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Resume time and quit the application (stops play mode in the editor).</summary>
    private void OnQuitClicked()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
