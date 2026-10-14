using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyFollowState : EnemyState
{
    public EnemyFollowState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Enemy.IsWall = false;
        Enemy.IsCliff = false;

        Anim.SetBool("IsRunning", true);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if(Enemy.CheckWall())
        {
            Enemy.ChangeState(Enemy.AlertState);
            Enemy.IsWall = true;
            return;
        }
        if(Enemy.CheckCliff())
        {
            Enemy.ChangeState(Enemy.AlertState);
            Enemy.IsCliff = true;
            return;
        }

        if(Vector3.Distance(Enemy.transform.position, Enemy.Player.transform.position) < Enemy.AttackRadius)
        {
            Enemy.ChangeState(Enemy.AttackState);
            return;
        }
        else
        {
            Vector3 directionToPlayer = (Enemy.Player.transform.position - Enemy.transform.position).normalized;

            bool right = directionToPlayer.x > 0.0f;

            Enemy.SetDirection(right);

            Rb.linearVelocity = Enemy.RunSpeed * Enemy.GetForward() * Vector2.right;
        }
    }
    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsRunning", false);
    }

}