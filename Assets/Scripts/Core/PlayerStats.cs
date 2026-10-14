using System;
using UnityEngine;

public interface IStat<T>
{
    public T GetValue();
}

public interface ILevelable
{
    public void LevelUp();
    public void LevelDown();
    public int CurrentLevel();
}

[Serializable]
public abstract class LevelScaledStat : IStat<float>, ILevelable
{
    private readonly float BaseValue;
    private readonly float PerLevel;
    private readonly int SoftCap;
    private readonly int HardCap;
    public readonly int MaxLevel;

    private int Level;
    private float Value;

    // How much each point is worth in each zone
    private const float BeforeSoftCap = 1.0f;
    private const float BetweenCaps = 0.5f;
    private const float AfterHardCap = 0.2f;

    public event Action<float> OnLevelChanged;

    protected LevelScaledStat(float baseValue, float perLevel, int initialLevel,
                              int softCap, int hardCap, int maxLevel = 99)
    {
        this.BaseValue = baseValue;
        this.PerLevel = perLevel;
        this.SoftCap = softCap;
        this.HardCap = Mathf.Max(hardCap, softCap); // keep the caps in order
        this.MaxLevel = maxLevel;

        Level = Mathf.Clamp(initialLevel, 0, maxLevel);
        Value = Calculate();
    }

    private float Calculate()
    {
        float effectiveLevels =
            Mathf.Min(Level, SoftCap) * BeforeSoftCap +
            Mathf.Clamp(Level - SoftCap, 0, HardCap - SoftCap) * BetweenCaps +
            Mathf.Max(Level - HardCap, 0) * AfterHardCap;

        return BaseValue + effectiveLevels * PerLevel;
    }

    public float GetValue() => Value;
    public int CurrentLevel() => Level;

    public void LevelUp()
    {
        if (Level >= MaxLevel)
            return;
        
        Level++;
        Value = Calculate();

        OnLevelChanged?.Invoke(Value);
    }

    public void LevelDown()
    {
        if (Level <= 0) 
            return;
        
        Level--;
        Value = Calculate();

        OnLevelChanged?.Invoke(Value);
    }
}

[Serializable]
public class Vigor : LevelScaledStat
{
    public Vigor(float healthPerLevel, float baseMaxHealth, int initialLevel, int softCap, int hardCap)
        : base(baseMaxHealth, healthPerLevel, initialLevel, softCap, hardCap) { }
}

[Serializable]
public class Strength : LevelScaledStat
{
    public Strength(float damagePerLevel, float baseDamage, int initialLevel, int softCap, int hardCap)
        : base(baseDamage, damagePerLevel, initialLevel, softCap, hardCap) { }
}