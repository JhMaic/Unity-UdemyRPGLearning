partial class Player
{
    protected class BlockState : PlayerState
    {
        public BlockState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = ctx.blockTime;
        }


        public override void NextState()
        {
            base.NextState();
            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Idle);
        }
    }
}