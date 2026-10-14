using UnityEngine;

public class PlayerFallState : PlayerState
{

    private float LastVelocityY;

    private float TimeEntered;

    public bool AllowJump = true;

    public PlayerFallState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Player.IsGrounded = false;

        Rb.gravityScale = Player.FallGravity;

        Anim.SetBool("IsFalling", true);

        TimeEntered = Time.time;

        AllowJump = true;
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

        if(Rb.linearVelocity.y < -Player.MaxFallSpeed)
        {
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, -Player.MaxFallSpeed);
            //No more falling
            Rb.gravityScale = 0.0f;
        }
        else
        {
            Rb.gravityScale = Player.FallGravity;
        }

        if(Player.GroundChecker.CheckGrounded(Player.GroundLayer))
        {
            if(LastVelocityY < -Player.HardFallMinSpeed)
            {
                Player.ChangeState(Player.LandingState);
            }
            else
            {
                Player.ChangeState(Player.IdleState);
            }
            return;
        }

        if(AllowJump)
        {
            if(Player.JumpPressed && Time.time - TimeEntered < Player.JumpCoyoteTime)
            {
                Player.ChangeState(Player.JumpState);
                return;
            }
        }

        float velocityX = Player.MoveInput.x * Player.AirMoveSpeed;

        Rb.linearVelocity = new Vector2(velocityX, Rb.linearVelocity.y);

        LastVelocityY = Rb.linearVelocity.y;
    }

    public override void OnExit()
    {
        base.OnExit();

        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsFalling", false);
    }
}