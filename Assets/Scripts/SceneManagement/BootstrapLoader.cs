using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapLoader
{
    private const string BootstrapScene = "Bootstrap";
    private const string MainMenuScene = "MainMenu";

    private static bool s_Loading;
    private static bool s_CheckQueued;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        s_Loading = false;
        s_CheckQueued = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        QueueCheck();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"{scene.name} loaded.");
        QueueCheck();
    }

    static void QueueCheck()
    {
        if (s_CheckQueued) return;
        s_CheckQueued = true;

        var go = new GameObject("[BootstrapLoader]") { hideFlags = HideFlags.HideAndDontSave };
        Object.DontDestroyOnLoad(go);
        go.AddComponent<Runner>().StartCoroutine(CheckNextFrame(go));
    }

    static IEnumerator CheckNextFrame(GameObject runner)
    {
        // Let every scene that was loading at startup finish first.
        yield return null;

        s_CheckQueued = false;
        Object.Destroy(runner);

        EnsureBootstrapLoaded();
    }

    static void EnsureBootstrapLoaded()
    {
        if (s_Loading) return;

        bool mainMenuLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.name == BootstrapScene) return;          // already present
            if (s.name == MainMenuScene) mainMenuLoaded = true;
        }

        if (mainMenuLoaded) return;

        s_Loading = true;
        var op = SceneManager.LoadSceneAsync(BootstrapScene, LoadSceneMode.Additive);
        op.completed += _ => s_Loading = false;
    }

    public static string GetCurrentRoom()
    {
        for(int i = 0; i < SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);

            if(scene.name == BootstrapScene || scene.name == MainMenuScene)
            {
                continue;
            }

            return scene.name;
        }

        return null;
    }

    private class Runner : MonoBehaviour { }
}