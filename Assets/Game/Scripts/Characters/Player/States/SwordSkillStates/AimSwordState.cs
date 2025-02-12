using UnityEngine;

partial class Player
{
    protected class AimSwordState : PlayerState
    {
        public AimSwordState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ctx.animatorController.AnimTrigger += OnReleaseTriggered;
        }

        private void OnReleaseTriggered()
        {
            SkillManager.Instance.Sword.TryUseSkill();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.Mouse1))
            {
                ctx.Skills.Sword.DrawDotCurve();
                var cameraPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (cameraPos.x > ctx.transform.position.x)
                    ctx.TurnRight();
                else
                    ctx.TurnLeft();
            }


            if (Input.GetKeyUp(KeyCode.Mouse1))
                ctx.anim.SetTrigger("AimSword_Release");
        }

        public override void Exit()
        {
            base.Exit();
            ctx.Skills.Sword.DestroyDots();
            ctx.animatorController.AnimTrigger -= OnReleaseTriggered;
        }

        public override void NextState()
        {
            // base.NextState();
            if (ctx.isAnimationFinished)
                StateChangeInvoke(States.Idle);
        }
    }
}