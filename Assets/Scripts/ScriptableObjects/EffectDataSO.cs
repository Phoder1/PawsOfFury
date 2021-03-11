using System;
using UnityEngine;

public enum EffectType { Instant, Toggle, OverTime }

[CreateAssetMenu(fileName = "new Effect Data", menuName = "ScriptableObjects/" + "Effect Data")]
public class EffectDataSO : ScriptableObject
{
    [Tooltip("Which target types should be affected.")]
    [SerializeField] TargetTypes affectedTargets;

    [Tooltip("The affected stat")]
    [SerializeField] StatType affectedStat;
    
    [Tooltip("The type of effect: \n" +
        "Instant = instant effect\n" +
        "Toggle = instant effect that is disabled after the duration\n" +
        "Overtime = over time effect, affecting for the duration, each tick.")]
    [SerializeField] EffectType effectType;
    
    [Tooltip("Whether to use a fixed amount or precentage.")]

    [SerializeField] bool inPercentage;
    
    [Tooltip("Whether to use precentage relative to max amount.")]
    [SerializeField] bool isRelativeToMax;

    [Tooltip("The effect amount.\n" +
        "If set to Overtime that's the damage everytick.\n" +
        "If was set to percentage:\n" +
        "(10 = 10%, 110 = 110%)")]
    [SerializeField] float amount;

    [Tooltip("The total duration of the effect.")]
    [Min(0.5f)]
    [SerializeField] float duration;

    [Tooltip("When set to overtime, this is the time between additions of the amount.")]
    [Min(0.2f)]
    [SerializeField] float tickTime;

    public TargetTypes AffectedTargets => affectedTargets;
    public StatType TargetStat => affectedStat;
    public EffectType EffectType => effectType;
    public bool InPercentage => inPercentage;
    public bool IsRelativeToMax => isRelativeToMax;
    public float Amount => amount;
    public float Duration => duration;
    public float TickTime => tickTime;
}
