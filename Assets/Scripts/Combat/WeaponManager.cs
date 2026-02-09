using UnityEngine;

/// <summary>
/// Single place to apply weapon upgrades to all equipped weapons (assault, railgun, shotgun).
/// Assign weapon references in the editor. UpgradeUI calls ApplyWeaponUpgrade so all weapons benefit.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Weapon assaultRifle;
    [SerializeField] private RailgunWeapon railgun;
    [SerializeField] private ShotgunWeapon shotgun;

    /// <summary>
    /// Applies a weapon upgrade to all equipped weapons of the given type.
    /// </summary>
    public void ApplyWeaponUpgrade(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.WeaponCooldown:
                if (assaultRifle != null) assaultRifle.ModifyCooldown(value);
                if (railgun != null) railgun.ModifyCooldown(value);
                if (shotgun != null) shotgun.ModifyCooldown(value);
                break;
            case UpgradeType.WeaponDamage:
                if (assaultRifle != null) assaultRifle.ModifyDamage(value);
                if (railgun != null) railgun.ModifyDamage(value);
                if (shotgun != null) shotgun.ModifyDamage(value);
                break;
            case UpgradeType.WeaponSize:
                if (assaultRifle != null) assaultRifle.ModifySize(value);
                if (railgun != null) railgun.ModifySize(value);
                if (shotgun != null) shotgun.ModifySize(value);
                break;
        }
    }
}
