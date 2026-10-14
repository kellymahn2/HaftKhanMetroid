using UnityEngine;
using UnityEngine.InputSystem;

public class PrayingSpot : MonoBehaviour
{
    [SerializeField] 
    private bool PlayerNearby = false;

    private Player Player = null;

    private InputAction ClickAction;

    public PrayingSpotData PrayingSpotData;

    void Awake()
    {
        ClickAction = new InputAction("Click", InputActionType.Button);
        ClickAction.AddBinding("<Mouse>/leftButton");
        ClickAction.AddBinding("<Touchscreen>/primaryTouch/tap");

        ClickAction.performed += OnClick;

        Debug.Assert(PrayingSpotData != null);

        ServiceLocator.Register(this);
    }

    void OnEnable()  => ClickAction.Enable();
    void OnDisable() => ClickAction.Disable();

    void OnDestroy()
    {
        ServiceLocator.Unregister(this);
        ClickAction.performed -= OnClick;   
    }

    void OnClick(InputAction.CallbackContext ctx)
    {
        if(!PlayerNearby || PauseManager.IsPaused)
        {
            return;
        }

        var pos = Pointer.current;
        if (pos == null)
        {
            return;  
        } 

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos.position.ReadValue());

        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        if (hit == null || hit.transform != transform)
        {
            return;  
        } 

        Interact();
    }

    void Interact()
    {
        Player.ChangeState(Player.PrayState);
        Player.LastPrayingSpot = PrayingSpotData;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player = collision.GetComponent<Player>();
            PlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerNearby = false;
            Player = null;
        }
    }
}