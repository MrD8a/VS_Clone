using UnityEngine;

/// <summary>
/// Data for one weapon: projectile prefab and stats. Create via Assets → Create → Combat → Weapon Data.
/// Assign to Weapon so new weapons are "add SO + prefab" without code changes.
/// </summary>
[CreateAssetMenu(menuName = "Combat/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
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
    [Tooltip("Seconds between shots.")]
    [SerializeField] private float cooldown = 1f;
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
    public float Cooldown => cooldown;
    public float Damage => damage;
    public float Range => range;
    public float Size => size;
    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
    public string Description => description;
}
