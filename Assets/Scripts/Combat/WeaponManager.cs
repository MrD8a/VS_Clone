using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages 3 weapon slots. Starting weapon is equipped at level 1; on level-up the player can
/// level up an equipped weapon or add a new one. Each weapon has a preset upgrade path (WeaponData.UpgradeTiers).
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon components (one per type)")]
    [SerializeField] private Weapon assaultRifle;
    [SerializeField] private RailgunWeapon railgun;
    [SerializeField] private ShotgunWeapon shotgun;

    [Header("Starting loadout")]
    [Tooltip("Weapon equipped at game start (e.g. Machine Gun).")]
    [SerializeField] private WeaponData startingWeapon;
    [Tooltip("All weapons that can be offered as 'gain new weapon' choices.")]
    [SerializeField] private List<WeaponData> allWeapons = new List<WeaponData>();

    public const int MaxSlots = 3;
    private readonly List<EquippedWeapon> _equipped = new List<EquippedWeapon>();

    public IReadOnlyList<EquippedWeapon> Equipped => _equipped;

    [System.Serializable]
    public class EquippedWeapon
    {
        public WeaponData Data;
        public int Level;
    }

    private void Start()
    {
        _equipped.Clear();
        RefreshComponentVisibility();
        if (startingWeapon != null)
            Equip(startingWeapon, 1);
    }

    private MonoBehaviour GetComponentFor(WeaponKind kind)
    {
        switch (kind)
        {
            case WeaponKind.MachineGun: return assaultRifle;
            case WeaponKind.Railgun: return railgun;
            case WeaponKind.Shotgun: return shotgun;
            default: return null;
        }
    }

    private void RefreshComponentVisibility()
    {
        SetWeaponEnabled(WeaponKind.MachineGun, false);
        SetWeaponEnabled(WeaponKind.Railgun, false);
        SetWeaponEnabled(WeaponKind.Shotgun, false);

        foreach (var eq in _equipped)
        {
            if (eq.Data == null) continue;
            SetWeaponEnabled(eq.Data.Kind, true);
            ApplyLevelToComponent(eq.Data.Kind, eq.Level);
        }
    }

    private void SetWeaponEnabled(WeaponKind kind, bool enabled)
    {
        var c = GetComponentFor(kind);
        if (c != null) c.gameObject.SetActive(enabled);
    }

    private void ApplyLevelToComponent(WeaponKind kind, int level)
    {
        switch (kind)
        {
            case WeaponKind.MachineGun:
                if (assaultRifle != null) assaultRifle.SetLevel(level);
                break;
            case WeaponKind.Railgun:
                if (railgun != null) railgun.SetLevel(level);
                break;
            case WeaponKind.Shotgun:
                if (shotgun != null) shotgun.SetLevel(level);
                break;
        }
    }

    /// <summary>
    /// Equip a new weapon at the given level. Fails if slots full or already equipped.
    /// </summary>
    public bool Equip(WeaponData data, int level)
    {
        if (data == null || _equipped.Count >= MaxSlots) return false;
        foreach (var eq in _equipped)
        {
            if (eq.Data == data) return false;
        }
        _equipped.Add(new EquippedWeapon { Data = data, Level = level });
        RefreshComponentVisibility();
        return true;
    }

    /// <summary>
    /// Level up an equipped weapon by one. Returns true if successful.
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

    /// <summary>
    /// Whether this weapon is currently equipped.
    /// </summary>
    public bool IsEquipped(WeaponData data)
    {
        if (data == null) return false;
        foreach (var eq in _equipped)
            if (eq.Data == data) return true;
        return false;
    }

    /// <summary>
    /// Current level of this weapon (0 if not equipped).
    /// </summary>
    public int GetLevel(WeaponData data)
    {
        if (data == null) return 0;
        foreach (var eq in _equipped)
            if (eq.Data == data) return eq.Level;
        return 0;
    }

    /// <summary>
    /// Weapons that can be offered as "gain new weapon" (not equipped, in allWeapons).
    /// </summary>
    public List<WeaponData> GetAvailableNewWeapons()
    {
        var list = new List<WeaponData>();
        if (allWeapons == null) return list;
        foreach (var w in allWeapons)
        {
            if (w != null && !IsEquipped(w))
                list.Add(w);
        }
        return list;
    }

    /// <summary>
    /// Options for "level up existing weapon" (equipped and not at max level).
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
