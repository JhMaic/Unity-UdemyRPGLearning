using System;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    public event Action AnimTrigger;
    public event Action AnimFinishedEvent;

    public void SetAnimationFinished()
    {
        AnimFinishedEvent?.Invoke();
    }

    public void FireTrigger()
    {
        AnimTrigger?.Invoke();
    }
}