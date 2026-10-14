using System;
using UnityEngine;

public abstract class ControllerState
{
    public virtual void OnEnter() {}

    public virtual void OnUpdate() {}

    public virtual void OnFixedUpdate() {}

    public virtual void OnExit() {}

    public virtual void OnAnimFinished(string name) {}
}