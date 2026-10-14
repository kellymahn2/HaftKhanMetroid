using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Player.IsGrounded = false;

        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, Player.JumpForce);
        Player.JumpPressed = false;

        Rb.gravityScale = Player.JumpGravity;

        Anim.SetBool("IsJumping", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(Player.MoveInput.x > 0.0f)
        {
            Player.SetDirection(true);
        }
        else if(Player.MoveInput.x < 0.0f)
        {
            Player.SetDirection(false);
        }   
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if(Player.JumpReleased && Rb.linearVelocity.y > 0.0f)
        {
            Rb.linearVelocity *= new Vector2(1.0f, Player.JumpCutMultiplier);
            Player.JumpReleased = false;
        }

        if(Rb.linearVelocity.y < 0.1f)
        {
            Player.ChangeState(Player.FallState);
            Player.FallState.AllowJump = false;
        }

        float velocityX = Player.MoveInput.x * Player.AirMoveSpeed;

        Rb.linearVelocity = new Vector2(velocityX, Rb.linearVelocity.y);
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsJumping", false);
    }
}