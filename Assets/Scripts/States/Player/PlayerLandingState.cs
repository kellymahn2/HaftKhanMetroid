using UnityEngine;

public class PlayerLandingState : PlayerState
{
    public PlayerLandingState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Player.IsGrounded = true;
        Rb.linearVelocity = Vector2.zero;

        Rb.gravityScale = 1.0f;

        Anim.SetBool("IsLanding", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        Debug.Log(Rb.linearVelocity);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        Player.ChangeState(Player.IdleState);
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsLanding", false);
    }
}