using UnityEngine;

public partial class Skeleton
{
    protected class MoveState : SkeletonState
    {
        public MoveState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.stateTimer = Random.Range(2, ctx.maxMoveTime);
        }

        public override void Update()
        {
            ctx.Move();

            if (ctx.IsWallDetected || ctx.IsGonnaFall)
                ctx.FlipX();
        }

        public override void Exit()
        {
            base.Exit();
            ctx.rb.linearVelocityX = 0;
        }

        public override void NextState()
        {
            if (ctx.IsPlayerDetected)
                StateChangeInvoke(States.Battle);
            if (ctx.stateTimer <= 0)
                StateChangeInvoke(States.Idle);
        }
    }
}