using System;
using System.Collections;
using System.Collections.Generic;
using Udar.SceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionManager : MonoBehaviour
{
    public CanvasGroup Fader;

    public bool InTransition { get; private set; } = false;

    public string CurrentScene;

    private Player Player => ServiceLocator.GetFirst<Player>();

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    void Start()
    {
        Fader.alpha = 0.0f;
        Fader.interactable = true;
        Fader.blocksRaycasts = false;

        CurrentScene = BootstrapLoader.GetCurrentRoom();
    }

    public void Transition(string scene, DoorID doorID)
    {
        if(InTransition)
        {
            return;
        }

        Func<Vector3> moveToPos = () =>
        {
            IReadOnlyCollection<Door> doors = ServiceLocator.Get<Door>();

            foreach(var door in doors)
            {
                if(door.DoorID == doorID)
                {
                    return door.Spawn;
                }
            }

            Debug.LogError($"Door {doorID.name} not found.");

            return Vector3.zero;
        };

        StartCoroutine(StartTransition(scene, moveToPos)); 
    }

    public void ReloadCurrent()
    {
        if(InTransition)
        {
            return;
        }

        StartCoroutine(StartTransition(CurrentScene, null));
    }

    public void TransitionToPraySpot(PrayingSpotData spotData)
    {
        if(InTransition)
        {
            return;
        }

        Func<Vector3> moveToPos = () =>
        {
            IReadOnlyCollection<PrayingSpot> spots = ServiceLocator.Get<PrayingSpot>();


            foreach(var spot in spots)
            {
                if(spot.PrayingSpotData == spotData)
                {
                    return spot.transform.position;
                }
            }

            Debug.LogError($"Praying spot {spotData} not found.");

            return Vector3.zero;
        };

        StartCoroutine(StartTransition(spotData.Scene.Name, moveToPos));
    }

    private IEnumerator StartTransition(string scene, Func<Vector3> moveToPos)
    {
        InTransition = true;

        Player.LockInput();

        yield return StartCoroutine(Fade(0.0f, 1.0f, 0.5f));

        Fader.blocksRaycasts = true;
        Fader.interactable = false;

        AsyncOperation unload = null;

        if(!string.IsNullOrEmpty(CurrentScene))
        {
            unload = SceneManager.UnloadSceneAsync(CurrentScene);
        }

        CurrentScene = scene;

        AsyncOperation load = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        load.allowSceneActivation = false;

        if(unload != null)
        {
            yield return unload;            
        }

        load.allowSceneActivation = true;

        yield return load;

        if(moveToPos != null)
        {
            Player.GetComponent<Rigidbody2D>().MovePosition(moveToPos());
        }

        Fader.interactable = true;
        Fader.blocksRaycasts = false;

        if(Player.CurrentState == Player.DeathState)
        {
            Player.ChangeState(Player.IdleState);
            Player.Rest();
        }

        yield return StartCoroutine(Fade(1.0f, 0.0f, 1.5f));

        Player.UnlockInput();

        

        InTransition = false;
    }

    public IEnumerator Fade(float start, float end, float duration)
    {
        Fader.alpha = start;

        yield return null;

        float t = 0.0f;

        while(t <= duration)
        {
            t += Time.unscaledDeltaTime;

            Fader.alpha = Mathf.Lerp(start, end, t / duration);

            yield return null;
        }

        Fader.alpha = end;
    }

}
