using UnityEngine;

public partial class Skeleton
{
    protected class IdleState : SkeletonState
    {
        public IdleState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = Random.Range(1, ctx.maxStandbyTime);
        }

        public override void Update()
        {
        }

        public override void NextState()
        {
            if (ctx.IsPlayerDetected)
                StateChangeInvoke(States.Battle);

            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Move);
        }
    }
}