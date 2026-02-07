using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the health bar under the player. Assign the bar root and fill image in the editor.
/// Bar is only visible when the player has missing health.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject barRoot;
    [SerializeField] private Image fillImage;

    private PlayerHealth _health;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }

    private void LateUpdate()
    {
        if (_health == null || barRoot == null || fillImage == null) return;

        bool show = _health.CurrentHealth < _health.MaxHealth && _health.CurrentHealth > 0;
        barRoot.SetActive(show);
        fillImage.fillAmount = _health.NormalizedHealth;
    }
}
