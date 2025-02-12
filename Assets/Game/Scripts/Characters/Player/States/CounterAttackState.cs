using Game.Scripts.Structs;
using PrimeTween;
using UnityEngine;

partial class Player
{
    protected class CounterAttackState : PlayerState
    {
        public CounterAttackState(string animBoolName, Player ctx) : base(animBoolName, ctx)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Tween.Custom(ctx.tweenCounterAttackMovement, newVal => ctx.rb.linearVelocityX = newVal * ctx.facingDir);
            ctx.HitBoxEvents.AddListener(OnHit);
        }

        private void OnHit(Collider2D other)
        {
            other.GetComponent<Enemy>().HurtEvent.Invoke(new Damage
            {
                value = ctx.statAgent.CalcAttackDamage(),
                position = ctx.transform.position,
                force = ctx.counterAttackKnockbackForce
            });
        }

        public override void Exit()
        {
            base.Exit();
            ctx.HitBoxEvents.RemoveListener(OnHit);
        }


        public override void NextState()
        {
            base.NextState();
            if (ctx.isAnimationFinished)
                StateChangeInvoke(States.Idle);
        }
    }
}