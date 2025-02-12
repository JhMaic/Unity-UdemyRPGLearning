using UnityEngine;

public partial class BlackholeSkill
{
    private class DefaultType : SkillVariant
    {
        public DefaultType(BlackholeSkill ctx) : base(ctx)
        {
        }


        public override void UseSkill()
        {
            base.UseSkill();
            ctx._controller.Setup(ctx.maxRadius, ctx.growSpeed, ctx.duration, 2, LayerMask.NameToLayer("PlayerHitBox"),
                null, () => { isOpening = false; });
        }
    }
}