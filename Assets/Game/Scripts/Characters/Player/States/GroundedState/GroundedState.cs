using UnityEngine;

partial class Player
{
    protected abstract class GroundedState : PlayerState
    {
        protected GroundedState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Update()
        {
            base.Update();
            ctx.MoveInputHandle();
            ctx.FlipByVelocityX();

            if (ctx.IsGrounded)
                ctx.JumpHandle();
        }


        public override void NextState()
        {
            base.NextState();

            if (!ctx.IsGrounded)
                StateChangeInvoke(States.Air);

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.X) || Input.GetKey(KeyCode.X) ||
                Input.GetKey(KeyCode.Mouse0))
                StateChangeInvoke(States.PrimaryAttack);

            if (Input.GetKeyDown(KeyCode.C))
                StateChangeInvoke(States.Block);

            if (Input.GetKeyDown(KeyCode.Mouse1) && ctx.Skills.Sword.Available)
                StateChangeInvoke(States.AimSwordState);
        }
    }
}