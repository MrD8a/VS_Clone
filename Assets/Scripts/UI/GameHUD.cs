using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates the XP bar fill and timer text. Assign in the editor.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TMP_Text timerText;

    private float _elapsedTime;
    private PlayerExperience _playerExperience;

    private void Start()
    {
        _playerExperience = FindFirstObjectByType<PlayerExperience>();
    }

    private void Update()
    {
        if (Time.timeScale > 0f)
            _elapsedTime += Time.deltaTime;

        if (timerText != null)
            timerText.text = $"{Mathf.FloorToInt(_elapsedTime / 60f):D2}:{Mathf.FloorToInt(_elapsedTime % 60f):D2}";

        if (xpBarFill != null && _playerExperience != null)
            xpBarFill.fillAmount = _playerExperience.NormalizedXP;
    }
}
