using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates the XP bar fill and timer text. Assign in the editor.
/// Uses GameTimer when assigned so HUD and spawn phases stay in sync.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameTimer gameTimer;

    private PlayerExperience _playerExperience;

    private void Start()
    {
        _playerExperience = FindFirstObjectByType<PlayerExperience>();
        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();
    }

    private void Update()
    {
        float elapsed = gameTimer != null ? gameTimer.ElapsedTime : Time.time;
        if (timerText != null)
            timerText.text = $"{Mathf.FloorToInt(elapsed / 60f):D2}:{Mathf.FloorToInt(elapsed % 60f):D2}";

        if (xpBarFill != null && _playerExperience != null)
            xpBarFill.fillAmount = _playerExperience.NormalizedXP;
    }
}
