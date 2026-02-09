using UnityEngine;

/// <summary>
/// Spawns enemies around the player based on the current <see cref="LevelSpawnConfig"/> phase.
///
/// Each phase defines one or more enemy types and their spawn intervals. The active phase
/// is determined by comparing <see cref="GameTimer.ElapsedTime"/> against each phase's
/// start time. If no config is assigned or no phase is active, falls back to spawning
/// a single <see cref="fallbackEnemyPrefab"/> at a fixed interval.
///
/// Enemies are spawned at a random position on a circle of <see cref="spawnDistance"/>
/// radius centered on the player.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Reference to the GameTimer (found automatically if not assigned).")]
    [SerializeField] private GameTimer gameTimer;

    [Tooltip("Level spawn schedule: phases, enemy types, and intervals.")]
    [SerializeField] private LevelSpawnConfig spawnConfig;

    [Tooltip("Distance from the player at which enemies spawn.")]
    [SerializeField] private float spawnDistance = 12f;

    [Header("Fallback (no config)")]
    [Tooltip("Enemy prefab to spawn if no LevelSpawnConfig is assigned.")]
    [SerializeField] private GameObject fallbackEnemyPrefab;

    [Tooltip("Seconds between fallback spawns.")]
    [SerializeField] private float fallbackSpawnInterval = 1.5f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Cached player transform (found by tag at Start).</summary>
    private Transform _player;

    /// <summary>Index of the currently active spawn phase (-1 = none).</summary>
    private int _currentPhaseIndex = -1;

    /// <summary>Per-entry spawn timers for the current phase.</summary>
    private float[] _entryTimers;

    /// <summary>Timer for fallback spawning (when no config is active).</summary>
    private float _fallbackTimer;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Find the player and game timer references.</summary>
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();
    }

    /// <summary>
    /// Each frame: determine the active phase, tick per-entry timers, and spawn enemies.
    /// Falls back to simple spawning if no config or phase is active.
    /// </summary>
    private void Update()
    {
        float levelTime = gameTimer != null ? gameTimer.ElapsedTime : Time.time;

        if (spawnConfig != null && spawnConfig.HasPhase(spawnConfig.GetPhaseIndexAt(levelTime)))
        {
            int phaseIndex = spawnConfig.GetPhaseIndexAt(levelTime);

            // Detect phase transitions and reset per-entry timers.
            if (phaseIndex != _currentPhaseIndex)
            {
                _currentPhaseIndex = phaseIndex;
                SpawnPhase phase = spawnConfig.GetPhase(_currentPhaseIndex);
                _entryTimers = new float[phase.entries.Length];
            }

            // Tick each entry's spawn timer.
            SpawnPhase currentPhase = spawnConfig.GetPhase(_currentPhaseIndex);
            float dt = Time.deltaTime;

            for (int i = 0; i < currentPhase.entries.Length; i++)
            {
                SpawnEntry entry = currentPhase.entries[i];
                if (entry.prefab == null || entry.interval <= 0f) continue;

                _entryTimers[i] += dt;
                if (_entryTimers[i] >= entry.interval)
                {
                    SpawnAt(entry.prefab);
                    _entryTimers[i] = 0f;
                }
            }
        }
        else
        {
            // Fallback: no config or no active phase.
            if (fallbackEnemyPrefab != null)
            {
                _fallbackTimer += Time.deltaTime;
                if (_fallbackTimer >= fallbackSpawnInterval)
                {
                    SpawnAt(fallbackEnemyPrefab);
                    _fallbackTimer = 0f;
                }
            }
        }
    }

    // ── Spawning ──────────────────────────────────────────────────────

    /// <summary>
    /// Instantiate <paramref name="prefab"/> at a random point on a circle around the player.
    /// </summary>
    private void SpawnAt(GameObject prefab)
    {
        if (_player == null) return;

        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(spawnDir * spawnDistance);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
