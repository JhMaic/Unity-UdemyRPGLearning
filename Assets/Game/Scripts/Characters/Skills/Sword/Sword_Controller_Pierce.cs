using System;
using UnityEngine;

public partial class Sword_Controller
{
    [Serializable]
    public record PierceConfig : BaseConfig
    {
        public int pierceTimes;
    }

    private class PierceType : SwordControlType<PierceConfig>
    {
        private int _currentCount;
        private bool _isStopped;

        public PierceType(Sword_Controller ctx, PierceConfig config) : base(ctx, config)
        {
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")) && _currentCount < config.pierceTimes)
            {
                _currentCount++;
                return;
            }

            _isStopped = true;
            ctx.transform.parent = other.transform;
            ctx._rb.bodyType = RigidbodyType2D.Kinematic;
            ctx._rb.linearVelocity = Vector2.zero;
        }

        public override bool CanReturn()
        {
            if (!_isStopped)
                return false;

            return base.CanReturn();
        }

        public override void Update()
        {
            if (!_isStopped)
                ctx.transform.right = ctx._rb.linearVelocity;
        }
    }
}