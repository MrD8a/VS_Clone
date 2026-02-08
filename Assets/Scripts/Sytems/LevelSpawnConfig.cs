using System;
using UnityEngine;

[Serializable]
public class SpawnEntry
{
    public GameObject prefab;
    [Tooltip("Seconds between spawns for this enemy type.")]
    public float interval = 1.5f;
}

[Serializable]
public class SpawnPhase
{
    [Tooltip("Phase starts when level time (seconds) >= this value.")]
    public float startTime;
    [Tooltip("Enemy types that can spawn in this phase; each uses its own interval.")]
    public SpawnEntry[] entries = Array.Empty<SpawnEntry>();
}

/// <summary>
/// Per-level spawn schedule: at which times spawn rate changes and which enemy types appear.
/// Create via Assets → Create → Spawn/Level Spawn Config.
/// </summary>
[CreateAssetMenu(menuName = "Spawn/Level Spawn Config", fileName = "LevelSpawnConfig")]
public class LevelSpawnConfig : ScriptableObject
{
    [Tooltip("Phases in ascending startTime order (e.g. 0, 60, 120).")]
    [SerializeField] private SpawnPhase[] phases = Array.Empty<SpawnPhase>();

    /// <summary>
    /// Gets the phase index active at the given level time (last phase where startTime &lt;= time).
    /// Returns -1 if no phase applies (e.g. before first phase).
    /// </summary>
    public int GetPhaseIndexAt(float levelTime)
    {
        int index = -1;
        for (int i = 0; i < phases.Length; i++)
        {
            if (phases[i].startTime <= levelTime)
                index = i;
        }
        return index;
    }

    public bool HasPhase(int index)
    {
        return index >= 0 && index < phases.Length;
    }

    public SpawnPhase GetPhase(int index)
    {
        return HasPhase(index) ? phases[index] : null;
    }
}
