using UnityEngine;

//TODO: handle reloading.
public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsDying", true);
    }

    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        Enemy.enabled = false;
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsDying", false);
    }
}