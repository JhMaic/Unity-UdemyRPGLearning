public partial class Skeleton
{
    protected class CounterAttackedState : SkeletonState
    {
        public CounterAttackedState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
        }

        public override void NextState()
        {
            if (ctx.isAnimationFinished)
                ctx.stateMachine.ChangeState(States.Cooldown, 1f);
        }
    }
}