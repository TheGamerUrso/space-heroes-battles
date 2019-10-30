using System;
using UnityEngine;
[Serializable]
public class LevelSystem
{
    public Action OnLevelUp;

    [Header("Level System")]
    [SerializeField]private int Level;
    [SerializeField]private float xp;
    [SerializeField]private float xpToLevel;
    [SerializeField] private int MaxLevel { get; set; }

    public LevelSystem(int Level, float xp, float xpToLevel, int MaxLevel = 20)
    {
        this.Level = Level;
        this.xp = xp;
        this.xpToLevel = xpToLevel;
        this.MaxLevel = MaxLevel;
    }

    public LevelSystem()
    {
        Level = 1;
        xp = 0;
        xpToLevel = 100;
    }

    public void AddXP(int ammount)
    {
        if (Level < MaxLevel)
        {
            xp += ammount;
            if (xp >= xpToLevel)
            {
                Level++;
                xp -= xpToLevel;
                xpToLevel = (Level / 10 + Level % 10) * 100 * Mathf.Pow(10, Level / 10);
                if (OnLevelUp != null)
                {
                    OnLevelUp();
                }
            }
        }
        else
        {
            Level = MaxLevel;
            xp = 0;
        }
    }

    public void SetLevel(int Level)
    {
        this.Level = Level;
    }

    public int GetLevel()
    {
        return Level;
    }
    public float GetXpToLevel()
    {
        return xpToLevel;
    }
    public float GetXP()
    {
        return xp;
    }
    public float GetXPNormalized()
    {
        return (float)xp / xpToLevel;
    }

    public float GetMaxLevel()
    {
        return MaxLevel;
    }

    public void SetMaxLevel(int ammount)
    {
        MaxLevel = ammount;
    }
}
