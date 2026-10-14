using UnityEngine;

public struct DamageData
{
    public float Damage;
}

public struct HealData
{
    public int HealAmount;
}

public interface IDamageable
{
    public void TakeDamage(in DamageData damageData);
}

public interface IHealable
{
    public void Heal(in HealData healData);
}