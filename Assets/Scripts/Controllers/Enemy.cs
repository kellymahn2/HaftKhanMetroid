using NUnit.Framework.Constraints;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public string CurrentStateName = "";
    public EnemyState CurrentState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyFollowState FollowState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyAlertState AlertState { get; private set; }


    [Header("General")]
    public float Health = 10;
    private float MaxHealth;
    public float TargetHealth = 10;
    public float DamageSpeed = 2.0f;

    public LayerMask GroundLayer;

    [Header("Movement parameters")]
    public float IdleTime = 1.0f;
    public float MoveSpeed = 5.0f;
    public float RunSpeed = 10.0f;

    [Header("Cliff checker")]
    public Transform CliffCheckerTransform;
    public float CliffCheckerDistance;
    public bool IsCliff;

    [Header("Wall checker")]
    public Transform WallCheckerTransform;
    public float WallCheckerDistance;
    public bool IsWall;

    [Header("Player detection")]
    public Player Player => ServiceLocator.GetFirst<Player>();
    
    public bool IsFollowingPlayer = false;

    public float DetectionRadius;
    public float AttackRadius;

    public GameObject Healthbar;

    public void OnAnimFinished(string name)
    {
        CurrentState.OnAnimFinished(name);
    }

    void Awake()
    {
        IdleState = new EnemyIdleState(this);

        PatrolState = new EnemyPatrolState(this);

        FollowState = new EnemyFollowState(this);

        AttackState = new EnemyAttackState(this);

        DeathState = new EnemyDeathState(this);

        AlertState = new EnemyAlertState(this);

        CurrentState = PatrolState;

        CurrentState.OnEnter();

        CurrentStateName = CurrentState.GetType().Name;

        MaxHealth = Health;

        Healthbar.transform.parent.gameObject.SetActive(false);
    }

    void Update() 
    {
        if(PauseManager.IsPaused)
        {
            return;
        }

        CurrentState.OnUpdate();

        float t = Health / MaxHealth;

        Vector3 scale = Healthbar.transform.localScale;
        scale.x = t;
        Healthbar.transform.localScale = scale;
    }

    void FixedUpdate()
    {
        if(PauseManager.IsPaused)
        {
            return;
        }
        
        CurrentState.OnFixedUpdate();
    }

    public void ChangeState(EnemyState state)
    {
        Debug.Log($"Exiting state {CurrentState.GetType().Name}, Entering state {state.GetType().Name}");
        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
        CurrentStateName = CurrentState.GetType().Name;
    }

    public float GetForward()
    {
        return transform.right.x;
    }

    public void SetDirection(bool right)
    {
        float yaw = right ? 0.0f : 180.0f;

        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);

    }

    public void FlipDirection()
    {
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 180.0f, 0.0f);
    }

    public void TakeDamage(in DamageData damageData)
    {
        Health -= damageData.Damage;

        Healthbar.transform.parent.gameObject.SetActive(true);

        if(Health <= 0)
        {
            ChangeState(DeathState);
            Healthbar.transform.parent.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsCliff ? Color.red : Color.green;

        Gizmos.DrawLine(CliffCheckerTransform.position, CliffCheckerTransform.position + -CliffCheckerTransform.up * CliffCheckerDistance);

        Gizmos.color = IsWall ? Color.red : Color.green;

        Gizmos.DrawLine(WallCheckerTransform.position, WallCheckerTransform.position + WallCheckerTransform.right * WallCheckerDistance);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, DetectionRadius);    

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, AttackRadius);    
    }

    public bool CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(WallCheckerTransform.position, WallCheckerTransform.right, WallCheckerDistance, GroundLayer);

        return hit.collider != null;
    }

    public bool CheckCliff()
    {
        RaycastHit2D hit = Physics2D.Raycast(CliffCheckerTransform.position, -CliffCheckerTransform.up, CliffCheckerDistance, GroundLayer);

        return hit.collider == null;
    }
}