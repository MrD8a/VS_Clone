using UnityEngine;

/// <summary>
/// Pulls pickups with MagnetPullable toward the player when they are within magnet range.
/// Magnet range is a player stat that can be increased via upgrades (ModifyMagnetRange).
/// </summary>
public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRange = 3f;
    [SerializeField] private float pullSpeed = 8f;

    private Transform _transform;

    public float MagnetRange => magnetRange;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        Vector2 playerPos = _transform.position;
        MagnetPullable[] pullables = FindObjectsByType<MagnetPullable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (MagnetPullable pullable in pullables)
        {
            if (pullable == null) continue;

            Vector2 toPlayer = playerPos - (Vector2)pullable.transform.position;
            float dist = toPlayer.magnitude;
            if (dist <= 0f || dist > magnetRange) continue;

            float speed = pullSpeed * pullable.PullSpeedMultiplier * Time.deltaTime;
            pullable.transform.position += (Vector3)(toPlayer.normalized * Mathf.Min(speed, dist));
        }
    }

    public void ModifyMagnetRange(float amount)
    {
        magnetRange = Mathf.Max(0f, magnetRange + amount);
    }
}
