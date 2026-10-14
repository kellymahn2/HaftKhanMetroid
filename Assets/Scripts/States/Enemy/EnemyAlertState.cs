using System.Collections;
using UnityEngine;

public class EnemyAlertState : EnemyState
{
    public EnemyAlertState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsIdle", true);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        Vector3 enemyToPlayer = Enemy.Player.transform.position - Enemy.transform.position;

        Enemy.SetDirection(enemyToPlayer.x > 0);

        if(!Enemy.CheckWall() && !Enemy.CheckCliff())
        {
            Enemy.ChangeState(Enemy.FollowState);
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsIdle", false);
    }
}