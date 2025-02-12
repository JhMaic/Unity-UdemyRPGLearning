using System;
using Game.Scripts.Structs;
using R3;
using UnityEngine;

public partial class Skeleton
{
    protected class AttackState : SkeletonState
    {
        public AttackState(string animBoolName, Skeleton ctx) : base(animBoolName, ctx)
        {
        }

        private void OnHit(Collider2D other)
        {
            var player = other.GetComponent<Player>();

            if (player.CurrentState.Equals(Player.States.Block))
                StateChangeInvoke(States.CounterAttacked);

            player.HurtEvent.Invoke(new Damage
            {
                value = ctx.statAgent.CalcAttackDamage(),
                position = ctx.transform.position
            });
        }

        public override void Enter()
        {
            base.Enter();
            ctx.HitBoxEvents.AddListener(OnHit);

            Observable.EveryUpdate()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(0.1f)))
                .Subscribe(_ => { ctx.Move(0.2f); }, _ => { ctx.rb.linearVelocityX = 0; });
        }


        public override void Update()
        {
        }

        public override void Exit()
        {
            base.Exit();
            ctx.HitBoxEvents.RemoveListener(OnHit);
        }

        public override void NextState()
        {
            if (ctx.isAnimationFinished)
                StateChangeInvoke(States.Cooldown);
        }
    }
}