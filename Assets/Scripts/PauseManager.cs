using UnityEngine;

public static class PauseManager
{
    public static bool IsPaused { get; private set; } = false;

    public static void SetPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0.0f : 1.0f;

        AudioListener.pause = pause;
    }

    public static void TogglePause()
    {
        SetPause(!IsPaused);
    }

}