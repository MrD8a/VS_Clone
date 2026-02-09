using UnityEngine;

/// <summary>
/// Types of legacy stat upgrades. These are used by the old <see cref="UpgradeData"/>
/// ScriptableObjects. The new upgrade flow uses <see cref="WeaponUpgradeTier"/> and
/// <see cref="WeaponManager"/> instead, but these remain for backward compatibility.
/// </summary>
public enum UpgradeType
{
    /// <summary>Increase weapon fire rate (shots per second).</summary>
    WeaponFireRate,

    /// <summary>Increase weapon damage.</summary>
    WeaponDamage,

    /// <summary>Increase weapon size / AOE.</summary>
    WeaponSize,

    /// <summary>Increase player move speed.</summary>
    MoveSpeed,

    /// <summary>Increase magnet pickup range.</summary>
    MagnetRange
}

/// <summary>
/// ScriptableObject for a single legacy stat upgrade (flat value applied directly).
/// Create via Assets → Create → Upgrades → Upgrade.
/// The new weapon upgrade flow uses <see cref="WeaponUpgradeTier"/> instead;
/// this class is kept for non-weapon upgrades (MoveSpeed, MagnetRange).
/// </summary>
[CreateAssetMenu(menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    /// <summary>Display name shown in the upgrade UI.</summary>
    public string upgradeName;

    /// <summary>Short description for tooltip or info panel.</summary>
    public string description;

    /// <summary>Which stat this upgrade modifies.</summary>
    public UpgradeType type;

    /// <summary>Flat value to add/subtract (interpretation depends on type).</summary>
    public float value;
}
