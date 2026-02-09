using UnityEngine;

public enum UpgradeType
{
    WeaponFireRate,
    WeaponDamage,
    WeaponSize,
    MoveSpeed,
    MagnetRange
}

[CreateAssetMenu(menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public UpgradeType type;
    public float value;
}
