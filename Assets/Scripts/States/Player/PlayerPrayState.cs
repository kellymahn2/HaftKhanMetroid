using UnityEngine;

public class PlayerPrayState : PlayerState
{


    private bool IsLeaving = false;
    public PlayerPrayState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();

        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsPraying", true);

        IsLeaving = false;

        ServiceLocator.GetFirst<HUD>().gameObject.SetActive(false);
        ServiceLocator.GetFirst<RestUIManager>().gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //TODO: make better.
        if(Player.AttackPressed)
        {
            if(!IsLeaving)
            {
                IsLeaving = true;
                Anim.SetBool("IsPraying", false);
            }
            
            Player.AttackPressed = false;
        }
    }

    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        if(name == "PrayLeaveFinished")
        {
            Player.ChangeState(Player.IdleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        ServiceLocator.GetFirst<RestUIManager>().gameObject.SetActive(false);
        ServiceLocator.GetFirst<HUD>().gameObject.SetActive(true);
    }
}
