using System;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        Rb.linearVelocity = Vector2.zero;

        Player.IsGrounded = true;
        Rb.gravityScale = 1.0f;

        Anim.SetBool("IsIdle", true);
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
        if(Mathf.Abs(Player.MoveInput.x) > 0.1f)
        {
            Player.ChangeState(Player.WalkingState);
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsIdle", false);
    }
}