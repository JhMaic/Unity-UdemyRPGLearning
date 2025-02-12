partial class Player
{
    private class MoveState : GroundedState
    {
        public MoveState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }


        public override void Exit()
        {
            base.Exit();
            ctx.rb.linearVelocityX = 0;
        }

        public override void NextState()
        {
            base.NextState();
            if (ctx._xInput.Equals(0f))
                StateChangeInvoke(States.Idle);
        }
    }
}