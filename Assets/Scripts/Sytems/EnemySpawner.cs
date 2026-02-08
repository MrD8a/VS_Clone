using UnityEngine;

/// <summary>
/// Spawns enemies based on level time: uses LevelSpawnConfig for phases (spawn rate + which types).
/// Assign a GameTimer and a LevelSpawnConfig in the editor.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private LevelSpawnConfig spawnConfig;
    [SerializeField] private float spawnDistance = 12f;

    [Header("Fallback (no config)")]
    [SerializeField] private GameObject fallbackEnemyPrefab;
    [SerializeField] private float fallbackSpawnInterval = 1.5f;

    private Transform _player;
    private int _currentPhaseIndex = -1;
    private float[] _entryTimers;
    private float _fallbackTimer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();
    }

    private void Update()
    {
        float levelTime = gameTimer != null ? gameTimer.ElapsedTime : Time.time;

        if (spawnConfig != null && spawnConfig.HasPhase(spawnConfig.GetPhaseIndexAt(levelTime)))
        {
            int phaseIndex = spawnConfig.GetPhaseIndexAt(levelTime);
            if (phaseIndex != _currentPhaseIndex)
            {
                _currentPhaseIndex = phaseIndex;
                SpawnPhase phase = spawnConfig.GetPhase(_currentPhaseIndex);
                _entryTimers = new float[phase.entries.Length];
            }

            SpawnPhase phaseNow = spawnConfig.GetPhase(_currentPhaseIndex);
            float dt = Time.deltaTime;
            for (int i = 0; i < phaseNow.entries.Length; i++)
            {
                SpawnEntry e = phaseNow.entries[i];
                if (e.prefab == null || e.interval <= 0f) continue;
                _entryTimers[i] += dt;
                if (_entryTimers[i] >= e.interval)
                {
                    SpawnAt(e.prefab);
                    _entryTimers[i] = 0f;
                }
            }
        }
        else
        {
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

    private void SpawnAt(GameObject prefab)
    {
        if (_player == null) return;
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(spawnDir * spawnDistance);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
