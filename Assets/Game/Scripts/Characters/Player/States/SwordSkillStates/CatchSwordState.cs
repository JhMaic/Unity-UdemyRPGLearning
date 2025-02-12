partial class Player
{
    protected class CatchSwordState : PlayerState
    {
        public CatchSwordState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            var swordPos = ctx.Skills.Sword.SwordInstance.transform.position;

            if (swordPos.x > ctx.transform.position.x)
                ctx.TurnRight();
            else
                ctx.TurnLeft();

            ctx.rb.linearVelocityX = -1 * ctx.facingDir * ctx.Skills.Sword.catchImpact;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void NextState()
        {
            base.NextState();
            if (ctx.isAnimationFinished)
                StateChangeInvoke(States.Idle);
        }
    }
}