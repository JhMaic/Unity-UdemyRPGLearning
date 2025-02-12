using CustomInspector;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStatAgent : MonoBehaviour
{
    [SerializeField] [SelfFill(true)]
    private Rigidbody2D rb;

    [SerializeField] [SelfFill(true)]
    private Character character;

    public Stat maxHealth;
    public Stat atk;
    public Stat critical;
    public Stat moveSpeed;
    public Stat gravityScale;

    public SerializableReactiveProperty<int> currentHealth;


    private void Awake()
    {
        currentHealth.Value = maxHealth.FinalValueInt;
        rb.gravityScale = gravityScale.FinalValue;

        gravityScale.FinalValueChangedEvent += OnGravityScaleChanged;
    }

    private void OnDestroy()
    {
        gravityScale.FinalValueChangedEvent -= OnGravityScaleChanged;
    }


    protected virtual void OnGravityScaleChanged(float value)
    {  
        rb.gravityScale = value;
    }

    public virtual void ApplyDamage(int damage)
    {
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);

        if (currentHealth.Value.Equals(0))
            character.Die();
    }

    public virtual void Heal(int value)
    {
        currentHealth.Value = Mathf.Min(maxHealth.FinalValueInt, currentHealth.Value + value);
    }

    public virtual float CalcAttackDamage()
    {
        return ApplyCriticalDamage(atk.FinalValue);
    }

    public virtual float CalcAttackDamage(ModifierConfig atkModifierOnce)
    {
        float sumFix = 0;
        float multFix = 0;

        if (atkModifierOnce.useSumModifier)
            sumFix = atkModifierOnce.sumModifierValue;

        if (atkModifierOnce.useMultModifier)
            multFix = atkModifierOnce.multModifierValue;

        return ApplyCriticalDamage((atk.BaseValue + atk.SumModifiersFinalValue + sumFix) *
                                   (atk.MultModifierFinalValue * multFix));
    }

    private float ApplyCriticalDamage(float damage)
    {
        var critChance = Mathf.Clamp01(critical.FinalValue / 100f);
        var isCritical = Random.value < critChance;

        if (isCritical)
            return damage * 2;

        return damage;
    }

    public struct ModifierConfig
    {
        public bool useSumModifier;
        public float sumModifierValue;
        public bool useMultModifier;
        public float multModifierValue;
    }
}