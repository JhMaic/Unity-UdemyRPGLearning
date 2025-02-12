using Cysharp.Threading.Tasks;
using Game.Scripts.Structs;
using R3;
using UnityEngine;

public partial class Skeleton
{
    protected class HurtState : SkeletonState
    {
        public HurtState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter<TParam>(TParam data)
        {
            base.Enter(data);
            if (data is Damage damage)
            {
                var dir = Mathf.Sign(ctx.transform.position.x - damage.position.x);

                Observable.Return(Unit.Default)
                    .Subscribe(async _ =>
                    {
                        ctx.rb.linearVelocity = new Vector2(dir * damage.force.x, damage.force.y) * ctx.TimeScale;
                        await UniTask.WaitForSeconds(0.10f);
                        ctx.rb.linearVelocityX = 0;
                    });
            }
        }

        public override void Update()
        {
        }

        public override void NextState()
        {
            if (ctx.isAnimationFinished)
                StateChangeInvoke(States.Battle);
        }
    }
}