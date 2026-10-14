using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Rb.linearVelocity = Vector2.zero;

        if(!Enemy.IsFollowingPlayer)
        {
            Enemy.StartCoroutine(IdleTimer());
        }

        Anim.SetBool("IsIdle", true);
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsIdle", false);
    }

    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Enemy.IdleTime);

        Enemy.FlipDirection();

        Enemy.ChangeState(Enemy.PatrolState);
    }
}