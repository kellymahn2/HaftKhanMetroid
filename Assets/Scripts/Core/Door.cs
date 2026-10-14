using UnityEngine;

public class Door : MonoBehaviour 
{
    public DoorObject DoorObject;

    public DoorID DoorID;

    public Transform SpawnPoint = null;

    public Vector3 Spawn => SpawnPoint == null ? transform.position : SpawnPoint.position;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //Go to B
            if(DoorObject.SpawnA == DoorID)
            {
                ServiceLocator.GetFirst<RoomTransitionManager>().Transition(DoorObject.SceneB.Name, DoorObject.SpawnB);
            }
            //Go to A
            else
            {
                ServiceLocator.GetFirst<RoomTransitionManager>().Transition(DoorObject.SceneA.Name, DoorObject.SpawnA);
            }
        }
    }
}