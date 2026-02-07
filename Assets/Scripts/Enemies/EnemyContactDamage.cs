using UnityEngine;

/// <summary>
/// Attach to an enemy. Defines how much damage the player takes per contact tick while overlapping this enemy.
/// Enemy's collider should be set to trigger so it doesn't physically collide with the player.
/// </summary>
public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int damagePerTick = 1;

    public int DamagePerTick => damagePerTick;
}
