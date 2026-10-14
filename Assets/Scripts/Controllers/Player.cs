using System;
using System.Collections;
using NUnit.Framework.Interfaces;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    public string CurrentStateName = "";
    public PlayerState CurrentState {get; private set; }
    public PlayerIdleState IdleState {get; private set; }
    public PlayerWalkingState WalkingState {get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerLandingState LandingState { get; private set; }
    public PlayerPrayState PrayState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerRollState RollState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerHitState HitState { get; private set; }

    [Header("Stats")]
    public Vigor Vigor;
    public Strength Strength;

    [Header("General")]
    [Min(0), SerializeField] private float Health = 10;
    
    private float TargetHealth;

    public float GetHealth => Health;

    public float HealSpeed = 2.0f;

    [Header("Ground checking")]
    public LayerMask GroundLayer;
    [SerializeField]
    public GroundChecker GroundChecker;
    public bool IsGrounded = true;

    [Header("Movement parameters")]
    public Vector2 MoveInput;
    public float MoveSpeed = 2.0f;

    public float AirMoveSpeed = 3.0f;

    [Header("Jump parameters")]
    public float JumpForce = 8.0f;
    public float JumpCutMultiplier = 0.5f;
    public float JumpGravity = 6.0f;
    public float JumpCoyoteTime = 0.2f;
    public bool JumpPressed = false, JumpReleased = false;

    [Header("Fall parameters")]
    public float FallGravity = 9.0f;

    [Min(0.0f)] public float HardFallMinSpeed = 5.0f;

    [Min(0.0f)] public float MaxFallSpeed = 5.0f;

    [Header("Attack parameters")]
    public bool AttackPressed = false;
    public float MaxComboTime = 0.1f;
    public Hitbox[] AttackHitboxes;
    public float AttackSpeedIncrease = 0.0f;

    [Header("Roll parameters")]
    public bool RollPressed = false;
    public float RollSpeed = 10.0f;
    public event Action<float> OnHealthChanged;
    public uint RollIFrames = 20;

    public int UnspentSkillPoints = 5;

    public bool IsDead => CurrentState == DeathState;

    public PrayingSpotData LastPrayingSpot;

    [field: SerializeField] public bool IsInvincible { get; private set; } = false;

    private Coroutine InvincibleRoutine = null;

    public void OnAnimFinished(string name)
    {
        CurrentState.OnAnimFinished(name);
    }

    void Awake()
    {
        IdleState = new PlayerIdleState(this);

        WalkingState = new PlayerWalkingState(this);

        JumpState = new PlayerJumpState(this);

        FallState = new PlayerFallState(this);

        LandingState = new PlayerLandingState(this);

        AttackState = new PlayerAttackState(this);

        PrayState = new PlayerPrayState(this);

        RollState = new PlayerRollState(this);

        DeathState = new PlayerDeathState(this);

        HitState = new PlayerHitState(this);

        CurrentState = IdleState;

        CurrentState.OnEnter();

        CurrentStateName = CurrentState.GetType().Name;

        GroundChecker.SetCollider(GetComponent<Collider2D>());

        Vigor = new Vigor(1, 5, 0, 10, 20);
        Strength = new Strength(1, 1, 0, 10, 20);

        ServiceLocator.Register(this);
    }

    void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    void Start()
    {
        if(Health > Vigor.GetValue())
        {
            Health = Vigor.GetValue();
        }

        TargetHealth = Health;
        //OnHealthChanged(Health);
    }

    void Update() 
    {
        if(PauseManager.IsPaused)
        {
            return;
        }

        CurrentState.OnUpdate();

        if(!IsDead && !Mathf.Approximately(Health, TargetHealth))
        {
            Health = Mathf.MoveTowards(Health, TargetHealth, HealSpeed * Time.deltaTime);
            OnHealthChanged?.Invoke(Health);
        }
    }

    void FixedUpdate()
    {
        if(PauseManager.IsPaused)
        {
            return;
        }
        

        CurrentState.OnFixedUpdate();
    }
    
    public void ChangeState(PlayerState state)
    {
        //Debug.Log($"Exiting state {CurrentState.GetType().Name}, Entering state {state.GetType().Name}");
        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
        CurrentStateName = CurrentState.GetType().Name;
    }

    public float GetForward()
    {
        return transform.right.x;
    }

    private IEnumerator MakeInvincible(uint frames)
    {
        IsInvincible = true;

        yield return new WaitForFrames(frames);

        InvincibleRoutine = null;

        IsInvincible = false;
    }

    public void SetInvincible(uint frames)
    {
        if(InvincibleRoutine != null)
        {
            StopCoroutine(InvincibleRoutine);
        }

        InvincibleRoutine = StartCoroutine(MakeInvincible(frames));
    }

    public void SetDirection(bool right)
    {
        float yaw = right ? 0.0f : 180.0f;

        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
    }

    public void LockInput()
    {
        GetComponent<PlayerInput>().enabled = false;

        if(CurrentState != IdleState && CurrentState != PrayState && CurrentState != DeathState)
        {
            ChangeState(IdleState);
        }
    }

    public void UnlockInput()
    {
        GetComponent<PlayerInput>().enabled = true;
    }

    public void Rest()
    {
        Health = Vigor.GetValue();
        TargetHealth = Health;

        OnHealthChanged?.Invoke(Health);
    }

    void OnMove(InputValue value)
    {
        if(PauseManager.IsPaused)
        {
            return;
        }

        MoveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if(PauseManager.IsPaused)
        {
            return;
        }
        if(value.isPressed)
        {
            JumpPressed = true;
            
            JumpReleased = false;
        }
        else
        {
            JumpPressed = false;
            JumpReleased = true;
        }
    }

    void OnAttack(InputValue value)
    {
        if(PauseManager.IsPaused)
        {
            return;
        }

        if(IsGrounded && !IsDead)
        {
            AttackPressed = value.isPressed;
        }
    }

    void OnRoll(InputValue value)
    {
        if(PauseManager.IsPaused)
        {
            return;
        }

        if(value.isPressed)
        {
            if(IsGrounded && !IsDead)
            {
                RollPressed = true;
            }
        }
    }

    void OnHeal(InputValue value)
    {
        if(PauseManager.IsPaused)
        {
            return;
        }
        
        if(value.isPressed && !IsDead)
        {
            Heal(1);
        }
    }

    void OnPause(InputValue value)
    {
        if(value.isPressed && !ServiceLocator.GetFirst<RoomTransitionManager>().InTransition)
        {
            PauseManager.TogglePause();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? GroundChecker.GroundedColor : GroundChecker.Color;

        Gizmos.DrawWireCube(GroundChecker.GroundCheckTransform.position + GroundChecker.CastDistance * -GroundChecker.GroundCheckTransform.up, GroundChecker.BoxSize);
    }

    public void TakeDamage(in DamageData damageData)
    {
        if(IsDead || IsInvincible)
        {
            return;
        }

        TargetHealth -= damageData.Damage;

        Health = TargetHealth;

        OnHealthChanged?.Invoke(Health);

        if(Health <= 0)
        {
            ChangeState(DeathState);
        }
        else
        {
            //ChangeState(HitState);
        }
    }

    private void Heal(float amount)
    {
        TargetHealth += amount;

        if(TargetHealth > Vigor.GetValue())
        {
            TargetHealth = Vigor.GetValue();
        }
    }
}
