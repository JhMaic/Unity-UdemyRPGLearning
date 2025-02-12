using UnityEngine;

partial class Player
{
    protected class WallJump : PlayerState
    {
        public WallJump(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = 0.2f;
            ctx.rb.linearVelocity = new Vector2(ctx.facingDir * -1 * 4, ctx.jumpForce + 3);
        }


        public override void NextState()
        {
            base.NextState();
            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Idle);
        }
    }
}