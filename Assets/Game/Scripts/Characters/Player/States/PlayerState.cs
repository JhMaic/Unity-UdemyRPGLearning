using UnityEngine;

partial class Player
{
    protected abstract class PlayerState : CharacterState<Player>
    {
        protected PlayerState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Update()
        {
        }


        /// <summary>
        ///     any state entry
        /// </summary>
        public override void NextState()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && ctx.Skills.Dash.Available)
                StateChangeInvoke(States.Dash);

            if (Input.GetKeyDown(KeyCode.Mouse1) && ctx.Skills.Sword.SwordInstance)
                ctx.Skills.Sword.ReturnSwordToPlayer();

            if (Input.GetKeyDown(KeyCode.R))
                ctx.Skills.Blackhole.TryUseSkill();
        }
    }
}