using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy)
    {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        Enemy.IsWall = false;
        Enemy.IsCliff = false;

        Anim.SetBool("IsWalking", true);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if(Enemy.CheckWall())
        {
            if(Enemy.IdleTime != 0.0f)
            {
                Enemy.ChangeState(Enemy.IdleState);
                Enemy.IsWall = true;
                return;
            }
            else
            {
                Enemy.FlipDirection();
            }
        }
        else if(Enemy.CheckCliff())
        {
            if(Enemy.IdleTime != 0.0f)
            {
                Enemy.ChangeState(Enemy.IdleState);
                Enemy.IsCliff = true;
                return;
            }
            else
            {
                Enemy.FlipDirection();
            }
        }

        Rb.linearVelocity = Enemy.MoveSpeed * Enemy.GetForward() * Vector2.right;

        if(DetectPlayer())
        {
            Enemy.IsFollowingPlayer = true;
            Enemy.ChangeState(Enemy.FollowState);
        }
    }

    public bool DetectPlayer()
    {
        float distance = Vector3.Distance(Enemy.transform.position, Enemy.Player.transform.position);

        //Distance check
        if(distance > Enemy.DetectionRadius)
        {
            return false;
        }

        //Direction check
        Vector3 directionToPlayer = (Enemy.Player.transform.position - Enemy.transform.position).normalized;

        float dot = Vector3.Dot(directionToPlayer, Enemy.transform.right);

        //Player behind
        if(dot < 0.0f)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(Enemy.transform.position, directionToPlayer, distance, Enemy.GroundLayer);

        if(hit.collider && !hit.collider.CompareTag("Player"))
        {
            return false;
        }

        return true;
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsWalking", false);
    }

    
}