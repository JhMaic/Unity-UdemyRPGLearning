using R3;
using UnityEngine;
using UnityEngine.UI;

public class Character_UI : MonoBehaviour
{
    [SerializeField] private CharacterStatAgent statAgent;
    [SerializeField] private Slider healthBar;

    private void Awake()
    {
        healthBar.maxValue = statAgent.maxHealth.FinalValue;
        statAgent.maxHealth.FinalValueChangedEvent += OnMaxHealthChanged;
        statAgent.currentHealth.AsObservable().Subscribe(OnHealthChanged).AddTo(this);
    }

    private void OnDestroy()
    {
        statAgent.maxHealth.FinalValueChangedEvent -= OnMaxHealthChanged;
    }

    private void OnHealthChanged(int value)
    {
        healthBar.value = value;
    }

    private void OnMaxHealthChanged(float value)
    {
        healthBar.maxValue = value;
    }
}