using UnityEngine;

/// <summary>
/// Shotgun weapon component. Fires an instant cone in the player's facing direction.
/// All enemies inside the cone take damage once per shot.
///
/// Requires a <see cref="WeaponData"/> with <c>ConePrefab</c> assigned. The cone prefab
/// must have a <see cref="ShotgunCone"/> component and a <c>PolygonCollider2D</c> set to trigger.
///
/// Stats (fire rate, damage, range, size) are read from the assigned WeaponData.
/// The Size stat controls the cone's overall scale. The <see cref="SetLevel"/> method
/// re-applies stats for the weapon's current upgrade tier.
///
/// Uses the player center and facing direction for spawn position and orientation,
/// consistent with other weapon scripts.
/// </summary>
public class ShotgunWeapon : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Header("Weapon Data")]
    [Tooltip("ScriptableObject that holds base stats and the cone prefab.")]
    [SerializeField] private WeaponData data;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Accumulated time since last shot (seconds).</summary>
    private float _timer;

    /// <summary>Current fire rate (shots per second). Higher = faster.</summary>
    private float _fireRate;

    /// <summary>Max distance to acquire a target (world units). Reserved for future use.</summary>
    private float _range;

    /// <summary>Damage dealt to each enemy hit by the cone.</summary>
    private float _currentDamage;

    /// <summary>Scale multiplier applied to the cone prefab.</summary>
    private float _size;

    /// <summary>Cached reference to the player transform (for spawn position).</summary>
    private Transform _playerTransform;

    /// <summary>Cached reference to the player controller (for facing direction).</summary>
    private PlayerController _playerController;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>
    /// Read base stats from WeaponData and cache the player references.
    /// </summary>
    private void Awake()
    {
        // Cache player references for GetPlayerPosition() and facing direction.
        _playerController = GetComponentInParent<PlayerController>();
        if (_playerController != null)
            _playerTransform = _playerController.transform;
        else
            _playerTransform = transform;

        if (data == null) return;

        _fireRate = data.FireRate;
        _range = data.Range;
        _currentDamage = data.Damage;
        _size = data.Size;
    }

    /// <summary>
    /// Tick the fire timer; fire when the interval (1 / fireRate) has elapsed.
    /// </summary>
    private void Update()
    {
        // Can't fire without a cone prefab.
        if (data?.ConePrefab == null) return;

        _timer += Time.deltaTime;

        // Convert fire rate (shots/sec) to interval (sec/shot).
        float interval = _fireRate > 0f ? 1f / _fireRate : 1f;

        if (_timer >= interval)
        {
            Fire();
            _timer = 0f;
        }
    }

    // ── Firing ────────────────────────────────────────────────────────

    /// <summary>
    /// Instantiate the shotgun cone in the player's horizontal facing direction.
    /// The cone snaps to pure left or right (no diagonal) so the visual stays clean.
    /// </summary>
    private void Fire()
    {
        // Determine horizontal facing: right or left.
        Vector2 facing = _playerController != null ? _playerController.FacingDirection : Vector2.right;
        if (facing.sqrMagnitude < 0.01f)
            facing = Vector2.right;
        facing = facing.x >= 0f ? Vector2.right : Vector2.left;

        // Spawn the cone offset from the player center in the facing direction.
        Vector2 origin = GetPlayerPosition();
        Vector2 spawnPos = origin + facing * data.SpawnOffsetFromPlayer;
        float angle = facing.x >= 0f ? 0f : 180f;

        GameObject coneGo = Instantiate(data.ConePrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));

        // Configure the ShotgunCone component (damage, size, angle, duration).
        if (coneGo.TryGetComponent(out ShotgunCone cone))
            cone.Setup(_currentDamage, _size, data.ConeAngle, data.ConeDuration, data.SpawnOffsetFromPlayer);
        else
            Destroy(coneGo);
    }

    /// <summary>
    /// Returns the player's world position for consistent spawn and distance calculations.
    /// Falls back to this transform if no player is found.
    /// </summary>
    private Vector2 GetPlayerPosition()
    {
        if (_playerTransform != null)
            return _playerTransform.position;
        return transform.position;
    }

    // ── Public stat modifiers (legacy, used by old UpgradeData) ──────

    /// <summary>Increase fire rate by a flat amount (clamped to 0.1 minimum).</summary>
    public void ModifyFireRate(float amount)
    {
        _fireRate = Mathf.Max(0.1f, _fireRate + amount);
    }

    /// <summary>Increase damage by a flat amount.</summary>
    public void ModifyDamage(float amount)
    {
        _currentDamage += amount;
    }

    /// <summary>Increase cone scale by a flat amount (clamped to 0.1 minimum).</summary>
    public void ModifySize(float amount)
    {
        _size = Mathf.Max(0.1f, _size + amount);
    }

    // ── Upgrade path ─────────────────────────────────────────────────

    /// <summary>
    /// Re-apply stats from the assigned <see cref="WeaponData"/> at the given level.
    /// Called by <see cref="WeaponManager"/> when the weapon is equipped or levelled up.
    /// </summary>
    public void SetLevel(int level)
    {
        if (data == null) return;

        _fireRate = data.GetFireRateAtLevel(level);
        _range = data.Range;
        _currentDamage = data.GetDamageAtLevel(level);
        _size = data.GetSizeAtLevel(level);
    }
}
