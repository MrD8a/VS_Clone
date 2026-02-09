using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// On level-up, shows 3 choices: level up an equipped weapon or gain a new weapon.
/// Each choice is applied via WeaponManager (item-based upgrades only).
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;

    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject panel;
    [SerializeField] private WeaponManager weaponManager;

    private void Awake()
    {
        Instance = this;
    }

    public bool PanelActive => panel != null && panel.activeSelf;

    public void Show()
    {
        panel.SetActive(true);
        if (weaponManager == null)
            weaponManager = FindFirstObjectByType<WeaponManager>();

        List<WeaponUpgradeChoice> options = BuildOptions();
        Shuffle(options);
        int count = Mathf.Min(buttons.Length, options.Count);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < count)
            {
                WeaponUpgradeChoice choice = options[i];
                string label = choice.IsLevelUp
                    ? $"{choice.WeaponData.DisplayName} → Lv{choice.NextLevel}"
                    : $"Get {choice.WeaponData.DisplayName}";
                buttons[i].GetComponentInChildren<TMP_Text>().text = label;
                buttons[i].gameObject.SetActive(true);
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => SelectChoice(choice));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private struct WeaponUpgradeChoice
    {
        public WeaponData WeaponData;
        public bool IsLevelUp;
        public int NextLevel;
    }

    private List<WeaponUpgradeChoice> BuildOptions()
    {
        var options = new List<WeaponUpgradeChoice>();
        if (weaponManager == null) return options;

        foreach (WeaponData data in weaponManager.GetLevelUpOptions())
        {
            int current = weaponManager.GetLevel(data);
            if (current < data.MaxLevel)
                options.Add(new WeaponUpgradeChoice { WeaponData = data, IsLevelUp = true, NextLevel = current + 1 });
        }
        if (weaponManager.Equipped.Count < WeaponManager.MaxSlots)
        {
            foreach (WeaponData data in weaponManager.GetAvailableNewWeapons())
                options.Add(new WeaponUpgradeChoice { WeaponData = data, IsLevelUp = false, NextLevel = 1 });
        }

        return options;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void SelectChoice(WeaponUpgradeChoice choice)
    {
        if (weaponManager == null) return;
        if (choice.IsLevelUp)
            weaponManager.LevelUpWeapon(choice.WeaponData);
        else
            weaponManager.Equip(choice.WeaponData, 1);
        Time.timeScale = 1f;
        panel.SetActive(false);
    }
}
