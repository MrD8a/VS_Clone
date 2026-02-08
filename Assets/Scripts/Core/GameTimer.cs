using UnityEngine;

/// <summary>
/// Single source of truth for level elapsed time. Use for spawn phases, HUD, etc.
/// </summary>
public class GameTimer : MonoBehaviour
{
    private float _elapsedTime;

    public float ElapsedTime => _elapsedTime;

    private void Update()
    {
        if (Time.timeScale > 0f)
            _elapsedTime += Time.deltaTime;
    }

    public void ResetTimer()
    {
        _elapsedTime = 0f;
    }
}
