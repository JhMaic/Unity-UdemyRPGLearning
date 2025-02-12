partial class Player
{
    private class DashState : PlayerState
    {
        private float _dashDir;

        public DashState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.Skills.Clone.TryUseSkill();

            ctx.stateTimer = ctx.Skills.Dash.duration;

            if (ctx._xInput != 0)
                _dashDir = ctx._xInput;
            else
                _dashDir = ctx.facingDir;
        }

        public override void Update()
        {
            base.Update();

            ctx.rb.linearVelocityY = 0;
            ctx.rb.linearVelocityX = _dashDir * ctx.Skills.Dash.speed;
        }

        public override void NextState()
        {
            base.NextState();
            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Idle);
        }
    }
}