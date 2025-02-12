partial class Player
{
    private class DeadState : PlayerState
    {
        public DeadState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void NextState()
        {
            base.NextState();
        }
    }
}