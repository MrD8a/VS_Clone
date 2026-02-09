using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Level-up upgrade selection UI. When the player levels up, <see cref="PlayerExperience"/>
/// pauses the game and calls <see cref="Show"/>. This panel presents up to 3 choices:
///   - Level up an equipped weapon (e.g. "Machine Gun → Lv2").
///   - Gain a new weapon if a slot is free (e.g. "Get Railgun").
///
/// Each choice is fulfilled through <see cref="WeaponManager.LevelUpWeapon"/> or
/// <see cref="WeaponManager.Equip"/>. After the player picks, time resumes and the panel hides.
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────

    /// <summary>Global instance so other scripts (e.g. PlayerExperience) can call Show().</summary>
    public static UpgradeUI Instance;

    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Array of UI buttons (one per upgrade slot). Typically 3.")]
    [SerializeField] private Button[] buttons;

    [Tooltip("Panel root; activated/deactivated to show/hide the upgrade choices.")]
    [SerializeField] private GameObject panel;

    [Tooltip("Reference to the WeaponManager that owns weapon slots and levels.")]
    [SerializeField] private WeaponManager weaponManager;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Register the singleton instance.</summary>
    private void Awake()
    {
        Instance = this;
    }

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Whether the upgrade panel is currently visible.</summary>
    public bool PanelActive => panel != null && panel.activeSelf;

    // ── Show / build options ──────────────────────────────────────────

    /// <summary>
    /// Activate the panel and populate buttons with up to 3 shuffled upgrade choices.
    /// Buttons beyond the available option count are hidden.
    /// </summary>
    public void Show()
    {
        panel.SetActive(true);

        // Lazy-find the weapon manager if not assigned in the Inspector.
        if (weaponManager == null)
            weaponManager = FindFirstObjectByType<WeaponManager>();

        // Build and shuffle the list of possible choices.
        List<WeaponUpgradeChoice> options = BuildOptions();
        Shuffle(options);

        int count = Mathf.Min(buttons.Length, options.Count);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < count)
            {
                // Configure this button with the choice label and click handler.
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
                // Hide unused buttons when fewer options than slots.
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    // ── Choice data structure ─────────────────────────────────────────

    /// <summary>
    /// Lightweight struct representing one upgrade option shown on a button.
    /// </summary>
    private struct WeaponUpgradeChoice
    {
        /// <summary>The weapon this choice refers to.</summary>
        public WeaponData WeaponData;

        /// <summary>True if this is a level-up of an equipped weapon; false if it's a new weapon.</summary>
        public bool IsLevelUp;

        /// <summary>The level the weapon would reach if this choice is picked.</summary>
        public int NextLevel;
    }

    // ── Option building ───────────────────────────────────────────────

    /// <summary>
    /// Collect all valid upgrade options from the <see cref="WeaponManager"/>:
    ///   1. Level-up options for each equipped weapon that isn't at max level.
    ///   2. New-weapon options for each unequipped weapon (if a slot is free).
    /// </summary>
    private List<WeaponUpgradeChoice> BuildOptions()
    {
        var options = new List<WeaponUpgradeChoice>();
        if (weaponManager == null) return options;

        // Add "level up" options for equipped weapons.
        foreach (WeaponData data in weaponManager.GetLevelUpOptions())
        {
            int current = weaponManager.GetLevel(data);
            if (current < data.MaxLevel)
            {
                options.Add(new WeaponUpgradeChoice
                {
                    WeaponData = data,
                    IsLevelUp = true,
                    NextLevel = current + 1
                });
            }
        }

        // Add "gain new weapon" options if slots are available.
        if (weaponManager.Equipped.Count < WeaponManager.MaxSlots)
        {
            foreach (WeaponData data in weaponManager.GetAvailableNewWeapons())
            {
                options.Add(new WeaponUpgradeChoice
                {
                    WeaponData = data,
                    IsLevelUp = false,
                    NextLevel = 1
                });
            }
        }

        return options;
    }

    // ── Shuffle utility ───────────────────────────────────────────────

    /// <summary>
    /// Fisher–Yates shuffle for randomizing option order.
    /// </summary>
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // ── Selection handler ─────────────────────────────────────────────

    /// <summary>
    /// Apply the chosen upgrade via WeaponManager, resume time, and hide the panel.
    /// </summary>
    private void SelectChoice(WeaponUpgradeChoice choice)
    {
        if (weaponManager == null) return;

        if (choice.IsLevelUp)
            weaponManager.LevelUpWeapon(choice.WeaponData);
        else
            weaponManager.Equip(choice.WeaponData, 1);

        // Resume gameplay and close the panel.
        Time.timeScale = 1f;
        panel.SetActive(false);
    }
}
