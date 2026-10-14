using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public MonoBehaviour DamageableComponent;

    private IDamageable Damageable = null;

    void Awake()
    {
        if(DamageableComponent is not IDamageable)
        {
            Debug.LogError($"Damageable component {DamageableComponent.name} assigned to {name} does not implement IDamagable");
        }
        else
        {
            Damageable = DamageableComponent as IDamageable;
        }
    }

    public void TakeHit(in HitData hitData)
    {
        DamageData damageData = new DamageData();
        damageData.Damage = hitData.Damage;

        Damageable?.TakeDamage(damageData);
    }

}