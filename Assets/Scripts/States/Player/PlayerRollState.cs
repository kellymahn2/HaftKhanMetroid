using UnityEngine;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Rb.linearVelocity = Player.GetForward() * Player.RollSpeed * Vector2.right;

        Player.RollPressed = false;

        Player.SetInvincible(Player.RollIFrames);

        Anim.SetBool("IsRolling", true);
    }

    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        Player.ChangeState(Player.IdleState);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if(ShouldFall)
        {
            Player.ChangeState(Player.FallState);
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        //TODO: heal.

        Anim.SetBool("IsRolling", false);
    }
}
