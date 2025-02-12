using Game.Scripts.FSM;

public partial class Skeleton
{
    protected class BlankState : State<Skeleton>
    {
        public BlankState(Skeleton ctx) : base(ctx)
        {
        }

        public override void Enter<TParam>(TParam data)
        {
            base.Enter(data);
            if (data is string animBoolName)
                ctx.anim.SetBool(animBoolName, true);
        }

        public override void Update()
        {
        }

        public override void NextState()
        {
        }
    }
}