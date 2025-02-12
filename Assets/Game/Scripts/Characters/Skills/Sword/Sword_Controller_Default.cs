using System;
using UnityEngine;

public partial class Sword_Controller
{
    [Serializable]
    public record DefaultConfig : BaseConfig;

    private class DefaultType : SwordControlType<DefaultConfig>
    {
        /// <summary>
        ///     is it once collided with something;
        /// </summary>
        private bool _hasCollided;

        public DefaultType(Sword_Controller ctx, DefaultConfig config) : base(ctx, config)
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
            }
        }

        public override bool CanReturn()
        {
            if (!_hasCollided)
                return false;

            return base.CanReturn();
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
        }
    }
}