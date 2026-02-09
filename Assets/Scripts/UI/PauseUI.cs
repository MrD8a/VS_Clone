using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Pause menu. Toggle with Escape. Panel has Resume, Restart, Quit. Same pattern as GameOverUI.
/// </summary>
public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Instance = this;
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void TogglePause()
    {
        if (IsBlocked()) return;

        if (panel != null && panel.activeSelf)
            Resume();
        else
            Pause();
    }

    private bool IsBlocked()
    {
        if (GameOverUI.Instance != null && GameOverUI.Instance.PanelActive)
            return true;
        if (UpgradeUI.Instance != null && UpgradeUI.Instance.PanelActive)
            return true;
        return false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
