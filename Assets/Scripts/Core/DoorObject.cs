using UnityEngine;
using Udar.SceneManager;

[CreateAssetMenu(menuName = "Doors/Door Object")]
public class DoorObject : ScriptableObject
{
    public SceneField SceneA, SceneB;

    public DoorID SpawnA, SpawnB;
}