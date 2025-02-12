using System;
using Game.Scripts.Structs;
using UnityEngine;

partial class Player
{
    [Serializable]
    protected class PrimaryAttack : PlayerState
    {
        private float _comboDuration;
        private float _lastTimeAttacked;

        public PrimaryAttack(string animBoolName, float comboDuration, Player ctx) : base(animBoolName, ctx)
        {
            ctx.ComboCounter = 0;
            _lastTimeAttacked = Time.time;
            _comboDuration = comboDuration;
        }

        private void OnHit(Collider2D other)
        {
            other.GetComponent<Enemy>().HurtEvent.Invoke(new Damage
            {
                value = ctx.statAgent.CalcAttackDamage(),
                force = ctx.primaryAttackKnockbackForce,
                position = ctx.transform.position
            });
        }

        public override void Enter()
        {
            base.Enter();
            if (ctx.ComboCounter > 2 || Time.time > _lastTimeAttacked + _comboDuration)
                ctx.ComboCounter = 0;

            ctx.anim.SetInteger("ComboCounter", ctx.ComboCounter);

            // 短暂位移时间
            ctx.stateTimer = 0.2f;
            ctx.rb.linearVelocityX = ctx.facingDir * ctx.attackMovement[ctx.ComboCounter].x;
            ctx.rb.linearVelocityY = ctx.attackMovement[ctx.ComboCounter].y;
            ctx.HitBoxEvents.AddListener(OnHit);
        }


        public override void Update()
        {
            base.Update();
            if (ctx.stateTimer <= 0)
                ctx.rb.linearVelocityX = 0;
        }

        public override void Exit()
        {
            base.Exit();
            _lastTimeAttacked = Time.time;
            ctx.ComboCounter++;
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