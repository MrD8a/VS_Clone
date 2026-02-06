using UnityEngine;

/// <summary>
/// Locks the camera to the target (e.g. player). Optionally smooths follow.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private bool useSmoothing = true;

    private Vector3 _velocity = Vector3.zero;
    private float _zPosition;

    private void Awake()
    {
        _zPosition = transform.position.z;
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 goal = new Vector3(target.position.x, target.position.y, _zPosition);

        if (useSmoothing && smoothTime > 0f)
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref _velocity, smoothTime);
        else
            transform.position = goal;
    }
}
