using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PlayerAttackState : PlayerState
{
    public bool InComboTime { get; private set; } = false;    
    private bool Comboed = false;
    private int CurrentCombo = 0;

    private int ComboCount = 4;

    private Coroutine CurrentComboCoroutine = null;

    public PlayerAttackState(Player player) : base(player)
    {}

    public override void OnEnter()
    {
        base.OnEnter();
        
        // Player.AttackPressed = false;

        InComboTime = false;

        Comboed = false;

        CurrentCombo = 0;
        
        Rb.linearVelocity = Vector2.zero;

        Anim.SetBool("IsAttacking", true);
        Anim.SetInteger("CurrentCombo", CurrentCombo);

        Anim.speed = (1.0f + Player.AttackSpeedIncrease);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(Player.AttackPressed)
        {
            // if(InComboTime)
            // {
            //     Comboed = true;   
            //     InComboTime = false;
            // }
            // Player.AttackPressed = false;
        }

        if(Player.MoveInput.x > 0.0f)
        {
            Player.SetDirection(true);
        }
        else if(Player.MoveInput.x < 0.0f)
        {
            Player.SetDirection(false);
        }
    }
    
    private IEnumerator ComboCoroutine()
    {
        float waitTime = Player.MaxComboTime;

        while(waitTime > 0.0f && !Player.AttackPressed)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

        if(Player.AttackPressed)
        {
            int combo = CurrentCombo + 1;
            Player.ChangeState(Player.AttackState);
            Anim.SetInteger("CurrentCombo", combo);
            CurrentCombo = combo;
            // Player.AttackPressed = false;
        }

        InComboTime = false;
        CurrentComboCoroutine = null;
    }

    public override void OnAnimFinished(string name)
    {
        // if(name == "AttackFinished")
        {
            InComboTime = false;
            if(Player.AttackPressed)
            {
                CurrentCombo = (CurrentCombo + 1) % (ComboCount);
                Anim.SetInteger("CurrentCombo", CurrentCombo);
            }
            else
            {
                Player.ChangeState(Player.IdleState);
                return;
            }
            // else
            // {
            //     Player.ChangeState(Player.IdleState);
            //     if(CurrentCombo != ComboCount - 1 && !Mathf.Approximately(Player.MaxComboTime, 0.0f))
            //     {
            //         if(CurrentComboCoroutine != null)
            //         {
            //             Player.StopCoroutine(CurrentComboCoroutine);
            //         }
            //         CurrentComboCoroutine = Player.StartCoroutine(ComboCoroutine());
            //     }
            //     else
            //     {
            //         InComboTime = false;
            //     }
            // }   
        }
        // else if(name == "ComboStart")
        // {
        //     InComboTime = true;
        // }
    }

    private void SetCurrentHitboxState(bool state)
    {
        Player.AttackHitboxes[CurrentCombo].enabled = state;
    }

    public override void OnExit()
    {
        base.OnExit();

        Anim.speed = 1.0f;

        Anim.SetBool("IsAttacking", false);
    }
}