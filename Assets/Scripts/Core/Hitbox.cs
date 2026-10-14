using UnityEngine;

public struct HitData
{
    public float Damage;
}

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private float Damage;
    private Collider2D Collider;

    void Start()
    {
        Collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if(hurtbox == null)
        {
            return;
        }

        HitData hitData = new HitData();
        hitData.Damage = Damage;

        hurtbox.TakeHit(hitData);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

}