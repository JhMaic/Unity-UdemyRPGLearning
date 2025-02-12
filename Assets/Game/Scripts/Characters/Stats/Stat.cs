using System;
using System.Linq;
using CustomInspector;
using UnityEngine;

[Serializable]
public class Stat
{
    [field: SerializeField]
    public float BaseValue { get; private set; }
    [SerializeField] private SerializableDictionary<string, float> sumModifiers;
    [SerializeField] private SerializableDictionary<string, float> multModifiers;

    public Stat(float value)
    {
        BaseValue = value;
    }

    public float SumModifiersFinalValue => sumModifiers.Count.Equals(0) ? 0 : sumModifiers.Values.Sum();
    public float MultModifierFinalValue =>
        multModifiers.Count.Equals(0) ? 1 : multModifiers.Values.Aggregate((x, y) => x * y);
    public float FinalValue => (BaseValue + SumModifiersFinalValue) * MultModifierFinalValue;

    public int FinalValueInt => Mathf.FloorToInt(FinalValue);

    public event Action<float> FinalValueChangedEvent;

    public void AddSumModifier(string key, float value)
    {
        sumModifiers[key] = value;
        FinalValueChangedEvent?.Invoke(FinalValue);
    }

    public void RemoveSumModifier(string key)
    {
        sumModifiers.Remove(key);
        FinalValueChangedEvent?.Invoke(FinalValue);
    }

    public void AddMultModifier(string key, float value)
    {
        multModifiers[key] = value;
        FinalValueChangedEvent?.Invoke(FinalValue);
    }

    public void RemoveMultiModifier(string key)
    {
        sumModifiers.Remove(key);
        FinalValueChangedEvent?.Invoke(FinalValue);
    }
}