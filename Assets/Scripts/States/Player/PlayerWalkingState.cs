using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Anim.SetBool("IsWalking", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(Player.JumpPressed)
        {
            Player.ChangeState(Player.JumpState);            
            return;
        }
        if(Player.RollPressed)
        {
            Player.ChangeState(Player.RollState);    
            return;
        }
        if(Player.AttackPressed && !Player.AttackState.InComboTime)
        {
            Player.ChangeState(Player.AttackState);
            return;
        }

        if(ShouldFall)
        {
            Player.ChangeState(Player.FallState);
            return;
        }

        if(Player.MoveInput.x > 0.0f)
        {
            Player.SetDirection(true);
        }
        else if(Player.MoveInput.x < 0.0f)
        {
            Player.SetDirection(false);
        }
        else
        {
            Player.ChangeState(Player.IdleState);
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        float velocityX = Mathf.Sign(Player.MoveInput.x) * Player.MoveSpeed;

        Rb.linearVelocity = Vector2.right * velocityX;
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsWalking", false);
    }
}