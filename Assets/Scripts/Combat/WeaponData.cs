using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Identifies which weapon component type this data asset drives.
/// Used by <see cref="WeaponManager"/> to enable the correct component per slot.
/// </summary>
public enum WeaponKind
{
    /// <summary>Uses <see cref="MachineGunWeapon"/>.</summary>
    MachineGun,
    /// <summary>Uses <see cref="RailgunWeapon"/>.</summary>
    Railgun,
    /// <summary>Uses <see cref="ShotgunWeapon"/>.</summary>
    Shotgun
}

/// <summary>
/// One tier of the upgrade path. Each tier defines per-stat multipliers that are applied
/// cumulatively when the weapon reaches that level.
///
/// Example: Level 2 with fireRateMultiplier 1.2 = 20% more shots per second on top of base.
/// Level 3 with damageMultiplier 1.1 = +10% damage on top of all previous multipliers.
/// A multiplier of 1 means "no change" for that stat at this tier.
/// </summary>
[System.Serializable]
public class WeaponUpgradeTier
{
    /// <summary>The level this tier applies at (2, 3, 4, …).</summary>
    public int level = 2;

    [Tooltip("Fire rate multiplier (e.g. 1.2 = 20% more shots per second).")]
    [Range(0.1f, 3f)]
    public float fireRateMultiplier = 1f;

    [Tooltip("Damage multiplier (e.g. 1.1 = +10% damage).")]
    [Range(0.1f, 3f)]
    public float damageMultiplier = 1f;

    [Tooltip("Size multiplier (e.g. 1.2 = 20% larger projectile / line / cone).")]
    [Range(0.1f, 3f)]
    public float sizeMultiplier = 1f;
}

/// <summary>
/// ScriptableObject that holds all data for one weapon: prefabs, base stats, and upgrade path.
/// Create via Assets → Create → Combat → Weapon Data.
///
/// Each weapon type reads only the fields relevant to it:
///   - Machine Gun → <see cref="ProjectilePrefab"/>
///   - Railgun     → <see cref="LinePrefab"/>, <see cref="LineLength"/>, <see cref="LineDuration"/>
///   - Shotgun     → <see cref="ConePrefab"/>, <see cref="ConeAngle"/>, <see cref="ConeDuration"/>
///
/// Shared stats: <see cref="FireRate"/>, <see cref="Damage"/>, <see cref="Range"/>, <see cref="Size"/>.
/// </summary>
[CreateAssetMenu(menuName = "Combat/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    // ── Identity ──────────────────────────────────────────────────────

    [Header("Weapon Type")]
    [Tooltip("Which weapon component this data asset is for.")]
    [SerializeField] private WeaponKind kind = WeaponKind.MachineGun;

    // ── Upgrade path ──────────────────────────────────────────────────

    [Header("Upgrade Path")]
    [Tooltip("Per-level multiplier tiers. Level 2, 3, 4, etc. Max level = 1 + count.")]
    [SerializeField] private List<WeaponUpgradeTier> upgradeTiers = new List<WeaponUpgradeTier>();

    // ── Machine Gun prefab ────────────────────────────────────────────

    [Header("Machine Gun")]
    [Tooltip("Projectile prefab (used only by MachineGunWeapon).")]
    [SerializeField] private Projectile projectilePrefab;

    // ── Railgun prefab ────────────────────────────────────────────────

    [Header("Railgun")]
    [Tooltip("Line prefab with RailgunLine + BoxCollider2D (used only by RailgunWeapon).")]
    [SerializeField] private GameObject linePrefab;

    [Tooltip("World length of the railgun line.")]
    [SerializeField] private float lineLength = 12f;

    [Tooltip("How long the line stays visible (seconds).")]
    [SerializeField] private float lineDuration = 0.08f;

    // ── Shotgun prefab ────────────────────────────────────────────────

    [Header("Shotgun")]
    [Tooltip("Cone prefab with ShotgunCone + PolygonCollider2D (used only by ShotgunWeapon).")]
    [SerializeField] private GameObject conePrefab;

    [Tooltip("Half-angle of the cone in degrees (e.g. 25 = 50° total).")]
    [SerializeField] private float coneAngle = 25f;

    [Tooltip("How long the cone stays visible (seconds).")]
    [SerializeField] private float coneDuration = 0.1f;

    // ── Shared stats ──────────────────────────────────────────────────

    [Header("Stats (shared by all weapon types)")]
    [Tooltip("Distance from player center to spawn the visual (line/cone offset).")]
    [SerializeField] private float spawnOffsetFromPlayer = 0.5f;

    [Tooltip("Fire rate: shots per second (higher = better).")]
    [SerializeField] private float fireRate = 1f;

    [Tooltip("Base damage per hit.")]
    [SerializeField] private float damage = 1f;

    [Tooltip("Max range for acquiring targets (world units).")]
    [SerializeField] private float range = 10f;

    [Tooltip("Scale of projectile / line width / cone size.")]
    [SerializeField] private float size = 1f;

    // ── Display info ──────────────────────────────────────────────────

    [Header("Display (UI)")]
    [Tooltip("Name shown in the upgrade UI. Falls back to the asset name if empty.")]
    [SerializeField] private string displayName;

    [Tooltip("Short description for future tooltip/info panels.")]
    [TextArea(2, 4)]
    [SerializeField] private string description;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Which weapon component type this data is for.</summary>
    public WeaponKind Kind => kind;

    /// <summary>Read-only list of upgrade tiers.</summary>
    public IReadOnlyList<WeaponUpgradeTier> UpgradeTiers => upgradeTiers;

    /// <summary>Projectile prefab (Machine Gun only).</summary>
    public Projectile ProjectilePrefab => projectilePrefab;

    /// <summary>Line prefab (Railgun only).</summary>
    public GameObject LinePrefab => linePrefab;
    /// <summary>World length of the railgun line.</summary>
    public float LineLength => lineLength;
    /// <summary>How long the railgun line stays visible.</summary>
    public float LineDuration => lineDuration;

    /// <summary>Cone prefab (Shotgun only).</summary>
    public GameObject ConePrefab => conePrefab;
    /// <summary>Half-angle of the shotgun cone in degrees.</summary>
    public float ConeAngle => coneAngle;
    /// <summary>How long the shotgun cone stays visible.</summary>
    public float ConeDuration => coneDuration;

    /// <summary>Distance from player center to spawn the visual.</summary>
    public float SpawnOffsetFromPlayer => spawnOffsetFromPlayer;
    /// <summary>Base fire rate (shots per second).</summary>
    public float FireRate => fireRate;
    /// <summary>Base damage per hit.</summary>
    public float Damage => damage;
    /// <summary>Max range for target acquisition.</summary>
    public float Range => range;
    /// <summary>Base size/scale multiplier.</summary>
    public float Size => size;

    /// <summary>Display name for the upgrade UI; falls back to the asset name.</summary>
    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
    /// <summary>Short description for future UI panels.</summary>
    public string Description => description;

    // ── Stat-at-level helpers ─────────────────────────────────────────

    /// <summary>
    /// Computes effective fire rate at the given level by multiplying the base fire rate
    /// with all tier multipliers from level 2 up to <paramref name="level"/>.
    /// </summary>
    public float GetFireRateAtLevel(int level)
    {
        return fireRate * GetCumulativeMultiplier(level, t => t.fireRateMultiplier);
    }

    /// <summary>
    /// Computes effective damage at the given level by multiplying the base damage
    /// with all tier multipliers from level 2 up to <paramref name="level"/>.
    /// </summary>
    public float GetDamageAtLevel(int level)
    {
        return damage * GetCumulativeMultiplier(level, t => t.damageMultiplier);
    }

    /// <summary>
    /// Computes effective size at the given level by multiplying the base size
    /// with all tier multipliers from level 2 up to <paramref name="level"/>.
    /// </summary>
    public float GetSizeAtLevel(int level)
    {
        return size * GetCumulativeMultiplier(level, t => t.sizeMultiplier);
    }

    /// <summary>
    /// Max level = 1 + number of defined tiers (e.g. 2 tiers → max level 3).
    /// </summary>
    public int MaxLevel => 1 + (upgradeTiers != null ? upgradeTiers.Count : 0);

    // ── Private helper ────────────────────────────────────────────────

    /// <summary>
    /// Multiplies all tier values (selected by <paramref name="selector"/>) whose level
    /// is between 2 and <paramref name="level"/> inclusive.
    /// </summary>
    private float GetCumulativeMultiplier(int level, System.Func<WeaponUpgradeTier, float> selector)
    {
        float mult = 1f;
        if (upgradeTiers == null) return mult;

        foreach (var tier in upgradeTiers)
        {
            if (tier.level >= 2 && tier.level <= level)
                mult *= selector(tier);
        }

        return mult;
    }
}
