using UnityEngine;

public abstract class EnemyState : ControllerState
{
    protected Enemy Enemy;
    protected Animator Anim;
    protected Rigidbody2D Rb;

    public EnemyState(Enemy enemy)
    {
        Enemy = enemy;

        Anim = Enemy.GetComponent<Animator>();

        Rb = Enemy.GetComponent<Rigidbody2D>();
    }
}