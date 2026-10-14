using System;
using UnityEngine;


[Serializable]
public class GroundChecker
{   
    public Transform GroundCheckTransform;
    public Vector2 BoxSize;
    public float CastDistance;
    public RaycastHit2D Hit { get; private set; }
    private Collider2D Collider;

    public Color GroundedColor = Color.green, Color = Color.red;
    public void SetCollider(Collider2D collider)
    {
        Collider = collider;
    }

    public bool CheckGrounded(LayerMask groundLayer)
    {
        Hit = Physics2D.BoxCast(GroundCheckTransform.position, BoxSize, 0.0f, -GroundCheckTransform.up, CastDistance, groundLayer);

        if(Hit.collider != null)
        {
            if(Hit.collider.IsTouching(Collider))
            {
                return true;
            }
        }

        return false;
    }

    
}