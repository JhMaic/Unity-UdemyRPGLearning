partial class Player
{
    protected class AirState : PlayerState
    {
        public AirState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Update()
        {
            base.Update();
            ctx.MoveInputHandle();
            ctx.FlipByVelocityX();
        }

        public override void NextState()
        {
            base.NextState();
            if (ctx.IsGrounded)
                StateChangeInvoke(States.Idle);

            if (ctx.IsWallDetected && !ctx._xInput.Equals(0) && !ctx.IsRising)
                StateChangeInvoke(States.WallSlide);
        }
    }
}