using System;
using PrimeTween;
using R3;
using UnityEngine;

public class Blackhole_Controller : MonoBehaviour
{
    private bool _isSetupFinished;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isSetupFinished)
            EnterEvent?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isSetupFinished)
            ExitEvent?.Invoke(other);
    }

    public event Action<Collider2D> EnterEvent;
    public event Action<Collider2D> ExitEvent;


    public void Setup(float maxRadius, float growSpeed, float duration, float reduceSpeed, LayerMask layerMask,
        Action onGrowthCompleted, Action onReduceCompleted)
    {
        transform.gameObject.layer = layerMask;

        Observable.Return(Unit.Default)
            .SubscribeAwait(async (_, ct) =>
            {
                await Tween.Scale(transform, Vector3.one * maxRadius, 1 / growSpeed, Ease.OutQuart);
                onGrowthCompleted?.Invoke();
                await Tween.Scale(transform, Vector3.one, 1 / reduceSpeed, Ease.Linear, startDelay: duration);
                Destroy(gameObject);
                onReduceCompleted?.Invoke();
            }).AddTo(this);

        _isSetupFinished = true;
    }
}