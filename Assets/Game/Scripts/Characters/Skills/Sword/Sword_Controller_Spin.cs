using System;
using Game.Scripts.Structs;
using R3;
using UnityEngine;

public partial class Sword_Controller
{
    [Serializable]
    public record SpinConfig : BaseConfig
    {
        /// <summary>
        ///     max distance from player
        /// </summary>
        public float maxDistance;
        /// <summary>
        ///     spin duration (seconds)
        /// </summary>
        public float spinDuration;
        /// <summary>
        ///     Action when spinning is finished
        /// </summary>
        public Action onCompleted;
    }

    private class SpinType : SwordControlType<SpinConfig>
    {
        /// <summary>
        ///     is it once collided with something;
        /// </summary>
        private bool _hasCollided;

        private bool _isStopped;

        public SpinType(Sword_Controller ctx, SpinConfig config) : base(ctx, config)
        {
        }


        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!_hasCollided && !_isStopped)
            {
                _hasCollided = true;
                FixPosition();
            }
        }

        public override void OnTriggerStay2D(Collider2D other)
        {
            base.OnTriggerStay2D(other);
            other.GetComponent<Enemy>()?.HurtEvent.Invoke(new Damage
            {
                force = new Vector2(8, 0),
                position = ctx.transform.position,
                value = ctx._player.statAgent.CalcAttackDamage()
            });
        }

        private void FixPosition()
        {
            _isStopped = true;
            ctx._rb.bodyType = RigidbodyType2D.Kinematic;
            ctx._rb.linearVelocity = Vector2.zero;

            Observable.Timer(TimeSpan.FromSeconds(config.spinDuration))
                .Subscribe(_ => { }, _ => { config.onCompleted(); })
                .AddTo(ctx);
        }


        public override void Launch()
        {
            base.Launch();
            ctx._anim.SetBool("Rotation", true);
        }

        public override void Update()
        {
            if (!_hasCollided)
                ctx.transform.right = ctx._rb.linearVelocity;


            if (!_isStopped && Vector2.Distance(ctx.transform.position, config.launchStartPos) > config.maxDistance)
                FixPosition();
        }
    }
}