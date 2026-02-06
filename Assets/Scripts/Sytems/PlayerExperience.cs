using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 5;

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        ApplyLevelUpBonus();

        Time.timeScale = 0f;
        UpgradeUI.Instance.Show();
    }


    private void ApplyLevelUpBonus()
    {
        Weapon weapon = GetComponent<Weapon>();
        weapon.SendMessage("OnLevelUp", SendMessageOptions.DontRequireReceiver);
    }
}
