using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [SerializeField] private int xpValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerExperience xp = other.GetComponent<PlayerExperience>();
            xp.AddXP(xpValue);
            Destroy(gameObject);
        }
    }
}
