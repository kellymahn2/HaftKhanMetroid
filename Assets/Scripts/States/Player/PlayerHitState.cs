using UnityEngine;

public class PlayerHitState : PlayerState
{
    public PlayerHitState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Anim.SetBool("IsHit", true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(Player.AttackPressed && !Player.AttackState.InComboTime)
        {
            Player.ChangeState(Player.AttackState);
            return;
        }
    }

    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        if(Player.CurrentState == this)
        {
            Player.ChangeState(Player.IdleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsHit", false);
    }
}