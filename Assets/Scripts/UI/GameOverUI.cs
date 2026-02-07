using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Game over panel. Same pattern as UpgradeUI: script on Canvas, panel is a child. Show() just activates the panel.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Button newGameButton;

    private void Awake()
    {
        Instance = this;
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    private void OnNewGameClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
