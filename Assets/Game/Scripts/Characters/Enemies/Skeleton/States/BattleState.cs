public partial class Skeleton
{
    protected class BattleState : SkeletonState
    {
        public BattleState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = ctx.battleTime;
        }

        public override void Update()
        {
            var playerPos = ctx._player.transform.position;


            if (playerPos.x < ctx.transform.position.x)
                ctx.TurnLeft();
            else
                ctx.TurnRight();

            ctx.Move(ctx.battleSpeedRate);
        }

        public override void Exit()
        {
            base.Exit();
            ctx.rb.linearVelocityX = 0;
        }

        public override void NextState()
        {
            if (ctx.IsPlayerInsideAttackRange)
                StateChangeInvoke(States.Attack);

            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Idle);

            if (!ctx.IsPlayerInsideBattleRange)
                StateChangeInvoke(States.Idle);
        }
    }
}