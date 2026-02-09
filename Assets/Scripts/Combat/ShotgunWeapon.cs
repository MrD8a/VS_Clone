using UnityEngine;

/// <summary>
/// Fires an instant cone in the direction the player is facing. All enemies in the cone take damage once.
/// Requires WeaponData with ConePrefab assigned. Size stat = cone radius/scale.
/// </summary>
public class ShotgunWeapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;

    private Transform _player;
    private PlayerController _playerController;
    private float _timer;
    private float _fireRate;
    private float _range;
    private float _currentDamage;
    private float _size;

    private void Awake()
    {
        if (data == null) return;
        _fireRate = data.FireRate;
        _range = data.Range;
        _currentDamage = data.Damage;
        _size = data.Size;
    }

    private void Start()
    {
        _player = transform;
        var root = _player.root;
        _playerController = root.GetComponentInChildren<PlayerController>();
    }

    private void Update()
    {
        if (data?.ConePrefab == null) return;

        _timer += Time.deltaTime;
        float interval = _fireRate > 0f ? 1f / _fireRate : 1f;
        if (_timer >= interval)
        {
            Fire();
            _timer = 0f;
        }
    }

    private void Fire()
    {
        Vector2 facing = _playerController != null ? _playerController.FacingDirection : Vector2.right;
        if (facing.sqrMagnitude < 0.01f)
            facing = Vector2.right;
        facing = facing.x >= 0f ? Vector2.right : Vector2.left;

        Vector2 spawnPos = (Vector2)_player.position + facing * data.SpawnOffsetFromPlayer;
        float angle = facing.x >= 0f ? 0f : 180f;
        GameObject coneGo = Instantiate(data.ConePrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));

        if (coneGo.TryGetComponent(out ShotgunCone cone))
            cone.Setup(_currentDamage, _size, data.ConeAngle, data.ConeDuration, data.SpawnOffsetFromPlayer);
        else
            Destroy(coneGo);
    }

    public void ModifyFireRate(float amount)
    {
        _fireRate = Mathf.Max(0.1f, _fireRate + amount);
    }

    public void ModifyDamage(float amount)
    {
        _currentDamage += amount;
    }

    public void ModifySize(float amount)
    {
        _size = Mathf.Max(0.1f, _size + amount);
    }

    /// <summary>
    /// Apply stats from assigned WeaponData at the given level (for item upgrade path).
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
