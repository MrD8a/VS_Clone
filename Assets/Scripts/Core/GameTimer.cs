using UnityEngine;

/// <summary>
/// Single source of truth for level elapsed time. Used by <see cref="EnemySpawner"/>
/// (spawn phases) and <see cref="GameHUD"/> (timer display) so they stay in sync.
///
/// Only ticks when <c>Time.timeScale &gt; 0</c> (i.e. not during pause or upgrades).
/// </summary>
public class GameTimer : MonoBehaviour
{
    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Seconds elapsed since the level started.</summary>
    private float _elapsedTime;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>Seconds elapsed since the level started.</summary>
    public float ElapsedTime => _elapsedTime;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Tick the timer each frame (only when the game is not paused).</summary>
    private void Update()
    {
        if (Time.timeScale > 0f)
            _elapsedTime += Time.deltaTime;
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>Reset elapsed time to zero (e.g. on scene reload).</summary>
    public void ResetTimer()
    {
        _elapsedTime = 0f;
    }
}
