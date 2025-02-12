using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Structs;
using R3;
using UnityEngine;

public partial class Sword_Controller
{
    [Serializable]
    public record BounceConfig : BaseConfig
    {
        public int bounceMaxTimes;
        public float bounceSpeed;
        public float detectRadius;
        public Action onCompleted;
    }

    private class BounceType : SwordControlType<BounceConfig>
    {
        /// <summary>
        ///     is it once collided with something;
        /// </summary>
        private bool _hasCollided;

        /// <summary>
        ///     is it bouncing between enemies;
        /// </summary>
        private bool _isBouncing;

        public BounceType(Sword_Controller ctx, BounceConfig config) : base(ctx, config)
        {
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!_hasCollided)
            {
                _hasCollided = true;
                ctx._anim.SetBool("Rotation", false);
                ctx.transform.parent = other.transform;
                ctx._rb.bodyType = RigidbodyType2D.Kinematic;
                ctx._rb.linearVelocity = Vector2.zero;

                if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
                    TryBounce();
            }

            if (_isBouncing)
                // do something
                other.GetComponent<Enemy>()?.HurtEvent.Invoke(new Damage
                {
                    force = new Vector2(5, 0),
                    position = ctx.transform.position,
                    value = ctx._player.statAgent.CalcAttackDamage()
                });
        }

        public override void Update()
        {
            if (!_hasCollided)
                ctx.transform.right = ctx._rb.linearVelocity;
        }


        private void TryBounce()
        {
            var results = new List<Collider2D>();
            var count = Physics2D.OverlapCircle(ctx.transform.position, config.detectRadius, new ContactFilter2D
            {
                layerMask = LayerMask.GetMask("Enemy"),
                useLayerMask = true
            }, results);

            if (count > 1)
            {
                ctx.transform.parent = null;
                BounceTargets(results.Select(c => c.transform).ToList());
            }
        }

        private void BounceTargets(List<Transform> transforms)
        {
            _isBouncing = true;
            ctx._anim.SetBool("Rotation", true);

            var count = transforms.Count;
            var idx = 0;

            Observable.EveryUpdate()
                .Select(_ => transforms[idx % count].position)
                .TakeWhile(_ => idx < config.bounceMaxTimes)
                .Subscribe(targetPos =>
                {
                    if (Vector2.Distance(targetPos, ctx.transform.position) > 0.2f)
                        ctx.transform.position =
                            Vector2.MoveTowards(ctx.transform.position, targetPos,
                                config.bounceSpeed * Time.deltaTime);
                    else
                        idx++;
                }, _ =>
                {
                    _isBouncing = false;
                    ctx._anim.SetBool("Rotation", false);
                    config.onCompleted();
                }).AddTo(ctx);
        }


        public override void Launch()
        {
            base.Launch();
            ctx._anim.SetBool("Rotation", true);
        }

        public override bool CanReturn()
        {
            if (_isBouncing || !_hasCollided)
                return false;

            return base.CanReturn();
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(ctx.transform.position, config.detectRadius);
        }
    }
}