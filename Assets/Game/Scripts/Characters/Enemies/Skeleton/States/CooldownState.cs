public partial class Skeleton
{
    protected class CooldownState : SkeletonState
    {
        public CooldownState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = ctx.attackCooldownTime;
        }

        public override void Enter<T>(T data)
        {
            base.Enter();
            if (data is float timer)
                ctx.stateTimer = timer;
        }

        public override void Update()
        {
        }

        public override void NextState()
        {
            if (ctx.stateTimer <= 0f)
                StateChangeInvoke(States.Battle);
        }
    }
}