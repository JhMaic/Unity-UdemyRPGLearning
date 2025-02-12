using UnityEngine;

partial class Player
{
    protected class WallSlide : PlayerState
    {
        public WallSlide(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }


        public override void Update()
        {
            base.Update();
            ctx.MoveInputHandle();
            if (!ctx._xInput.Equals(0))
                ctx.rb.linearVelocityY *= 0.5f;
        }


        public override void NextState()
        {
            base.NextState();
            if (!ctx.IsWallDetected)
                StateChangeInvoke(States.Idle);

            if (ctx.IsGrounded && ctx.rb.linearVelocityY.Equals(0))
                StateChangeInvoke(States.Idle);

            if (Input.GetKeyDown(KeyCode.Space))
                StateChangeInvoke(States.WallJump);
        }
    }
}