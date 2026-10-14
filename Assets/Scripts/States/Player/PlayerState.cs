using System;
using UnityEngine;

public abstract class PlayerState : ControllerState
{
    protected Player Player;
    protected Animator Anim;
    protected Rigidbody2D Rb;

    protected SpriteRenderer Sr;

    protected bool ShouldFall => Rb.linearVelocity.y < -0.1f || !Player.GroundChecker.CheckGrounded(Player.GroundLayer);

    public PlayerState(Player player)
    {
        Player = player;    

        Anim = Player.GetComponent<Animator>();

        Rb = Player.GetComponent<Rigidbody2D>();

        Sr = Player.GetComponentInChildren<SpriteRenderer>();
    }

}