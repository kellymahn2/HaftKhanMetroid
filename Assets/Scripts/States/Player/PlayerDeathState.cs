using UnityEngine;

//TODO: handle reloading.
public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsDying", true);
    }


    public override void OnAnimFinished(string name)
    {
        base.OnAnimFinished(name);

        if(Player.LastPrayingSpot)
        {
            ServiceLocator.GetFirst<RoomTransitionManager>().TransitionToPraySpot(Player.LastPrayingSpot);
        }
    }


    public override void OnExit()
    {
        base.OnExit();

        Anim.SetBool("IsDying", false);
    }
}