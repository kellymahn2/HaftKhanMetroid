using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsAttacking", true);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if(Vector3.Distance(Enemy.transform.position, Enemy.Player.transform.position) > Enemy.AttackRadius)
        {
            Enemy.ChangeState(Enemy.FollowState);
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsAttacking", false);
    }
}