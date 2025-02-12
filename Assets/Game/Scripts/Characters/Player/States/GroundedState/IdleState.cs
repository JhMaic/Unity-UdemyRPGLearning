partial class Player
{
    private class IdleState : GroundedState
    {
        public IdleState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void NextState()
        {
            base.NextState();
            if (!ctx._xInput.Equals(0f))
                StateChangeInvoke(States.Move);
        }
    }
}