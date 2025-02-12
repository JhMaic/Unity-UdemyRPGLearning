using UnityEngine;

public class DeactoveWhenInvisible : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        OnBecameInvisible();
    }

    private void OnBecameInvisible()
    {
        ComponentManager.AnimDisableEvent.OnNext(_anim);
    }

    private void OnBecameVisible()
    {
        ComponentManager.AnimEnableEvent.OnNext(_anim);
    }
}