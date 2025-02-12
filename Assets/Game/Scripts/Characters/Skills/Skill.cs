using CustomInspector;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] [ReadOnly] private float timer;

    public float cooldown;

    private bool IsInCooldown => timer > 0;
    public bool Available => !IsInCooldown && SkillAvailability(out _);
    public abstract string SkillName { get; }

    protected Player player => PlayerManager.Instance.player;


    protected virtual void Update()
    {
        if (IsInCooldown)
            timer = Mathf.Max(timer - Time.deltaTime, 0);
    }

    public void TryUseSkill()
    {
        if (Available)
        {
            timer = cooldown;
            UseSkill();
        }
    }

    public bool TestAvailability(out string reason)
    {
        if (!IsInCooldown)
            return SkillAvailability(out reason);

        reason = "cooldown";
        return false;
    }

    protected abstract void UseSkill();

    protected virtual bool SkillAvailability(out string reason)
    {
        reason = null;
        return true;
    }
}