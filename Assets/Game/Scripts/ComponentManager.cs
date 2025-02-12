using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class ComponentManager : MonoBehaviour
{
    public static Subject<Animator> AnimDisableEvent = new();
    public static Subject<Animator> AnimEnableEvent = new();


    private void Awake()
    {
        AnimDisableEvent
            .Chunk(50)
            .SubscribeAwait(async (_, ct) =>
            {
                foreach (var animator in _)
                    animator.enabled = false;

                await UniTask.Yield(PlayerLoopTiming.Update);
            });

        AnimEnableEvent
            .Chunk(50)
            .SubscribeAwait(async (_, ct) =>
            {
                foreach (var animator in _)
                    animator.enabled = true;

                await UniTask.Yield(PlayerLoopTiming.Update);
            });
    }
}