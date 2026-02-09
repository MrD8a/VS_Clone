using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Identifies which weapon component (MachineGun/Railgun/Shotgun) this data uses.
/// Used by WeaponManager to enable the correct component per slot.
/// </summary>
public enum WeaponKind
{
    MachineGun,
    Railgun,
    Shotgun
}

/// <summary>
/// One tier of the upgrade path. At this level, base stats are multiplied by these values (1 = no change).
/// E.g. level 2: fire rate 1.2 = 20% more shots per second; level 3: damage 1.1 = +10% damage.
/// </summary>
[System.Serializable]
public class WeaponUpgradeTier
{
    public int level = 2;
    [Tooltip("Fire rate multiplier (e.g. 1.2 = 20% more shots per second).")]
    [Range(0.1f, 3f)]
    public float fireRateMultiplier = 1f;
    [Tooltip("Damage multiplier (e.g. 1.1 = +10% damage).")]
    [Range(0.1f, 3f)]
    public float damageMultiplier = 1f;
    [Tooltip("Size multiplier.")]
    [Range(0.1f, 3f)]
    public float sizeMultiplier = 1f;
}

/// <summary>
/// Data for one weapon: projectile prefab and stats. Create via Assets → Create → Combat → Weapon Data.
/// Assign to Weapon so new weapons are "add SO + prefab" without code changes.
/// </summary>
[CreateAssetMenu(menuName = "Combat/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon type (for slots)")]
    [SerializeField] private WeaponKind kind = WeaponKind.MachineGun;
    [Header("Upgrade path (level 2, 3, 4... apply these multipliers)")]
    [SerializeField] private List<WeaponUpgradeTier> upgradeTiers = new List<WeaponUpgradeTier>();

    [Header("Projectile (Assault Rifle)")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Railgun")]
    [SerializeField] private GameObject linePrefab;
    [Tooltip("World length of the railgun line.")]
    [SerializeField] private float lineLength = 12f;
    [Tooltip("How long the line stays visible (seconds).")]
    [SerializeField] private float lineDuration = 0.08f;

    [Header("Shotgun")]
    [SerializeField] private GameObject conePrefab;
    [Tooltip("Half-angle of the cone in degrees (e.g. 30 = 60° total).")]
    [SerializeField] private float coneAngle = 25f;
    [Tooltip("How long the cone stays visible (seconds).")]
    [SerializeField] private float coneDuration = 0.1f;

    [Header("Stats")]
    [Tooltip("Distance from player center to spawn weapon visuals (e.g. player radius). Railgun/shots use this so the sprite appears just outside the player.")]
    [SerializeField] private float spawnOffsetFromPlayer = 0.5f;
    [Tooltip("Fire rate: shots per second (higher = better).")]
    [SerializeField] private float fireRate = 1f;
    [Tooltip("Base damage per hit.")]
    [SerializeField] private float damage = 1f;
    [Tooltip("Max range for acquiring targets.")]
    [SerializeField] private float range = 10f;
    [Tooltip("Scale of projectile / line width / cone size. Affects AOE size per weapon type.")]
    [SerializeField] private float size = 1f;

    [Header("Optional (for UI / future)")]
    [SerializeField] private string displayName;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    public Projectile ProjectilePrefab => projectilePrefab;
    public GameObject LinePrefab => linePrefab;
    public float LineLength => lineLength;
    public float LineDuration => lineDuration;
    public GameObject ConePrefab => conePrefab;
    public float ConeAngle => coneAngle;
    public float ConeDuration => coneDuration;
    public float SpawnOffsetFromPlayer => spawnOffsetFromPlayer;
    public float FireRate => fireRate;
    public float Damage => damage;
    public float Range => range;
    public float Size => size;
    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
    public string Description => description;
    public WeaponKind Kind => kind;
    public IReadOnlyList<WeaponUpgradeTier> UpgradeTiers => upgradeTiers;

    /// <summary>
    /// Computes effective fire rate (shots per second) for a given level (base * product of tier multipliers for level 2..level).
    /// </summary>
    public float GetFireRateAtLevel(int level)
    {
        float mult = 1f;
        if (upgradeTiers != null)
        {
            foreach (var t in upgradeTiers)
            {
                if (t.level >= 2 && t.level <= level)
                    mult *= t.fireRateMultiplier;
            }
        }
        return fireRate * mult;
    }

    public float GetDamageAtLevel(int level)
    {
        float mult = 1f;
        if (upgradeTiers != null)
        {
            foreach (var t in upgradeTiers)
            {
                if (t.level >= 2 && t.level <= level)
                    mult *= t.damageMultiplier;
            }
        }
        return damage * mult;
    }

    public float GetSizeAtLevel(int level)
    {
        float mult = 1f;
        if (upgradeTiers != null)
        {
            foreach (var t in upgradeTiers)
            {
                if (t.level >= 2 && t.level <= level)
                    mult *= t.sizeMultiplier;
            }
        }
        return size * mult;
    }

    /// <summary>
    /// Max level = 1 + number of defined tiers (e.g. tiers at 2,3,4 -> max level 4).
    /// </summary>
    public int MaxLevel => 1 + (upgradeTiers != null ? upgradeTiers.Count : 0);
}
