using CustomInspector;
using UnityEngine;

public partial class BlackholeSkill : Skill
{
    public enum SkillVariantType
    {
        CloneAttack,
        Default
    }

    [SerializeField] private GameObject blackholePrefab;

    [SerializeField] private float growSpeed;
    [SerializeField] private float maxRadius;
    [SerializeField] private float duration;
    [SerializeField] private SkillVariantType skillVariantType;

    [ShowIfIs(nameof(skillVariantType), SkillVariantType.CloneAttack)]
    [SerializeField] private CloneAttackVariantConfig cloneAttackVariantConfig;


    private Blackhole_Controller _controller;
    private ISkillController _skillController;


    public override string SkillName => "Blackhole";

    protected override void UseSkill()
    {
        _controller = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity)
            .GetComponent<Blackhole_Controller>();

        _skillController = skillVariantType switch
        {
            SkillVariantType.Default => new DefaultType(this),
            SkillVariantType.CloneAttack => new CloneAttackType(this),
            _ => new DefaultType(this)
        };

        _skillController.UseSkill();
    }


    protected override bool SkillAvailability(out string reason)
    {
        if (_skillController is not null && !_skillController.SkillAvailability(out reason))
            return false;

        return base.SkillAvailability(out reason);
    }

    private interface ISkillController
    {
        public void UseSkill();
        public bool SkillAvailability(out string reason);
    }

    private abstract class SkillVariant : ISkillController
    {
        protected readonly BlackholeSkill ctx;
        protected bool isOpening;

        protected SkillVariant(BlackholeSkill ctx)
        {
            this.ctx = ctx;
            isOpening = false;
        }

        public virtual void UseSkill()
        {
            ctx._controller.EnterEvent += OnEnemyEntered;
            ctx._controller.ExitEvent += OnEnemyExit;
            isOpening = true;
        }

        public virtual bool SkillAvailability(out string reason)
        {
            reason = null;

            if (isOpening)
            {
                reason = "blackhole is opening";
                return false;
            }

            return true;
        }

        public virtual void OnEnemyEntered(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            enemy.TimeScale = 0;
        }

        public virtual void OnEnemyExit(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            enemy.TimeScale = 1;
        }
    }
}