using System;
using UnityEngine;

/// <summary>
/// One enemy type entry within a spawn phase: the prefab and how often it spawns.
/// </summary>
[Serializable]
public class SpawnEntry
{
    /// <summary>Enemy prefab to instantiate.</summary>
    public GameObject prefab;

    [Tooltip("Seconds between spawns for this enemy type.")]
    public float interval = 1.5f;
}

/// <summary>
/// A spawn phase: defines when it becomes active and which enemy types spawn during it.
/// Phases are listed in ascending <see cref="startTime"/> order inside a
/// <see cref="LevelSpawnConfig"/> asset.
/// </summary>
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
/// Create via Assets → Create → Spawn / Level Spawn Config.
///
/// <see cref="EnemySpawner"/> reads this asset to determine the current phase and
/// spawn enemies accordingly.
/// </summary>
[CreateAssetMenu(menuName = "Spawn/Level Spawn Config", fileName = "LevelSpawnConfig")]
public class LevelSpawnConfig : ScriptableObject
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Phases in ascending startTime order (e.g. 0, 60, 120).")]
    [SerializeField] private SpawnPhase[] phases = Array.Empty<SpawnPhase>();

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Returns the index of the phase active at the given level time
    /// (last phase whose <c>startTime &lt;= levelTime</c>).
    /// Returns -1 if no phase applies.
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

    /// <summary>Whether the given index points to a valid phase.</summary>
    public bool HasPhase(int index)
    {
        return index >= 0 && index < phases.Length;
    }

    /// <summary>Returns the phase at the given index, or null if out of range.</summary>
    public SpawnPhase GetPhase(int index)
    {
        return HasPhase(index) ? phases[index] : null;
    }
}
