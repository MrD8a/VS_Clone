using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the player's weapon slots. Holds references to one component per weapon type
/// and maintains a list of currently equipped weapons with their levels.
///
/// At game start, only the <see cref="startingWeapon"/> is equipped at level 1.
/// On level-up, <see cref="UpgradeUI"/> queries this manager for available options
/// (level up an existing weapon, or gain a new one) and applies the player's choice.
///
/// The manager enables/disables weapon GameObjects so only equipped weapons fire.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Header("Weapon Components (one per type, assign in editor)")]
    [Tooltip("GameObject with MachineGunWeapon component.")]
    [SerializeField] private MachineGunWeapon machineGun;

    [Tooltip("GameObject with RailgunWeapon component.")]
    [SerializeField] private RailgunWeapon railgun;

    [Tooltip("GameObject with ShotgunWeapon component.")]
    [SerializeField] private ShotgunWeapon shotgun;

    [Header("Starting Loadout")]
    [Tooltip("Weapon equipped at game start (e.g. Machine Gun asset).")]
    [SerializeField] private WeaponData startingWeapon;

    [Tooltip("All weapons that can be offered as 'gain new weapon' choices.")]
    [SerializeField] private List<WeaponData> allWeapons = new List<WeaponData>();

    // ── Constants ─────────────────────────────────────────────────────

    /// <summary>Maximum number of weapons the player can equip simultaneously.</summary>
    public const int MaxSlots = 3;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>List of currently equipped weapons and their levels.</summary>
    private readonly List<EquippedWeapon> _equipped = new List<EquippedWeapon>();

    /// <summary>Read-only view of equipped weapons (for UpgradeUI).</summary>
    public IReadOnlyList<EquippedWeapon> Equipped => _equipped;

    /// <summary>
    /// Tracks one equipped weapon: its data asset and current level.
    /// </summary>
    [System.Serializable]
    public class EquippedWeapon
    {
        /// <summary>Reference to the weapon's ScriptableObject data.</summary>
        public WeaponData Data;

        /// <summary>Current upgrade level (1 = base, 2+ = upgraded).</summary>
        public int Level;
    }

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>
    /// Disable all weapon components, then equip the starting weapon.
    /// </summary>
    private void Start()
    {
        _equipped.Clear();
        RefreshComponentVisibility();

        if (startingWeapon != null)
            Equip(startingWeapon, 1);
    }

    // ── Component mapping ─────────────────────────────────────────────

    /// <summary>
    /// Returns the weapon MonoBehaviour that corresponds to the given <see cref="WeaponKind"/>.
    /// </summary>
    private MonoBehaviour GetComponentFor(WeaponKind kind)
    {
        switch (kind)
        {
            case WeaponKind.MachineGun: return machineGun;
            case WeaponKind.Railgun:    return railgun;
            case WeaponKind.Shotgun:    return shotgun;
            default:                    return null;
        }
    }

    // ── Visibility / level refresh ────────────────────────────────────

    /// <summary>
    /// Disable all weapon GameObjects, then enable and apply levels only for equipped weapons.
    /// Called after any equip or level-up change.
    /// </summary>
    private void RefreshComponentVisibility()
    {
        // Disable all weapon types first.
        SetWeaponEnabled(WeaponKind.MachineGun, false);
        SetWeaponEnabled(WeaponKind.Railgun, false);
        SetWeaponEnabled(WeaponKind.Shotgun, false);

        // Enable and apply stats for each equipped weapon.
        foreach (var eq in _equipped)
        {
            if (eq.Data == null) continue;
            SetWeaponEnabled(eq.Data.Kind, true);
            ApplyLevelToComponent(eq.Data.Kind, eq.Level);
        }
    }

    /// <summary>
    /// Enable or disable the GameObject for the given weapon type.
    /// </summary>
    private void SetWeaponEnabled(WeaponKind kind, bool enabled)
    {
        var component = GetComponentFor(kind);
        if (component != null)
            component.gameObject.SetActive(enabled);
    }

    /// <summary>
    /// Call SetLevel on the correct weapon component so its runtime stats
    /// match the upgrade tier for the given level.
    /// </summary>
    private void ApplyLevelToComponent(WeaponKind kind, int level)
    {
        switch (kind)
        {
            case WeaponKind.MachineGun:
                if (machineGun != null) machineGun.SetLevel(level);
                break;
            case WeaponKind.Railgun:
                if (railgun != null) railgun.SetLevel(level);
                break;
            case WeaponKind.Shotgun:
                if (shotgun != null) shotgun.SetLevel(level);
                break;
        }
    }

    // ── Public API (called by UpgradeUI) ──────────────────────────────

    /// <summary>
    /// Equip a new weapon at the given level.
    /// Returns false if slots are full or the weapon is already equipped.
    /// </summary>
    public bool Equip(WeaponData data, int level)
    {
        if (data == null || _equipped.Count >= MaxSlots) return false;

        // Prevent duplicates.
        foreach (var eq in _equipped)
        {
            if (eq.Data == data) return false;
        }

        _equipped.Add(new EquippedWeapon { Data = data, Level = level });
        RefreshComponentVisibility();
        return true;
    }

    /// <summary>
    /// Level up an equipped weapon by one tier.
    /// Returns false if the weapon isn't equipped or is already at max level.
    /// </summary>
    public bool LevelUpWeapon(WeaponData data)
    {
        if (data == null) return false;

        foreach (var eq in _equipped)
        {
            if (eq.Data != data) continue;
            if (eq.Level >= data.MaxLevel) return false;

            eq.Level++;
            RefreshComponentVisibility();
            return true;
        }

        return false;
    }

    /// <summary>Whether the given weapon data is currently equipped.</summary>
    public bool IsEquipped(WeaponData data)
    {
        if (data == null) return false;
        foreach (var eq in _equipped)
            if (eq.Data == data) return true;
        return false;
    }

    /// <summary>Current level of the given weapon (0 if not equipped).</summary>
    public int GetLevel(WeaponData data)
    {
        if (data == null) return 0;
        foreach (var eq in _equipped)
            if (eq.Data == data) return eq.Level;
        return 0;
    }

    // ── Upgrade option queries (called by UpgradeUI.BuildOptions) ─────

    /// <summary>
    /// Returns weapons that can be offered as "gain new weapon" options:
    /// present in <see cref="allWeapons"/> but not currently equipped.
    /// </summary>
    public List<WeaponData> GetAvailableNewWeapons()
    {
        var list = new List<WeaponData>();
        if (allWeapons == null) return list;

        foreach (var weapon in allWeapons)
        {
            if (weapon != null && !IsEquipped(weapon))
                list.Add(weapon);
        }

        return list;
    }

    /// <summary>
    /// Returns equipped weapons that can still be levelled up
    /// (current level &lt; max level defined by their upgrade tiers).
    /// </summary>
    public List<WeaponData> GetLevelUpOptions()
    {
        var list = new List<WeaponData>();

        foreach (var eq in _equipped)
        {
            if (eq.Data != null && eq.Level < eq.Data.MaxLevel)
                list.Add(eq.Data);
        }

        return list;
    }
}
