using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;

    [SerializeField] private List<UpgradeData> allUpgrades;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        panel.SetActive(true);

        List<UpgradeData> choices = new(allUpgrades);
        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeData upgrade = choices[Random.Range(0, choices.Count)];
            choices.Remove(upgrade);

            buttons[i].GetComponentInChildren<TMP_Text>().text = upgrade.upgradeName;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => SelectUpgrade(upgrade));
        }
    }

    private void SelectUpgrade(UpgradeData upgrade)
    {
        ApplyUpgrade(upgrade);
        Time.timeScale = 1f;
        panel.SetActive(false);
    }

    private void ApplyUpgrade(UpgradeData upgrade)
    {
        Weapon weapon = FindFirstObjectByType<Weapon>();
        PlayerController player = FindFirstObjectByType<PlayerController>();

        switch (upgrade.type)
        {
            case UpgradeType.WeaponCooldown:
                weapon.ModifyCooldown(upgrade.value);
                break;
            case UpgradeType.WeaponDamage:
                weapon.ModifyDamage(Mathf.RoundToInt(upgrade.value));
                break;
            case UpgradeType.MoveSpeed:
                player.ModifyMoveSpeed(upgrade.value);
                break;
        }
    }
}
