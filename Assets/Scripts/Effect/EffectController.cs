using System.Collections;
using UnityEngine;

public class EffectController
{
    EffectDataSO effectData;
    EntityStats stats;
    Coroutine effectCoro;
    EffectHandler effectHandler;
    private float statValue { get => GetStat.GetSetValue; set => GetStat.GetSetValue = value; }
    private EntityStats.Stat GetStat => stats.GetStat(effectData.TargetStat);
    public EffectController(EntityStats stats, EffectDataSO effectData)
    {
        this.effectData = effectData;
        this.stats = stats;
        effectHandler = EffectHandler._instance;
        Begin();
    }
    private float AmountFromPercentage(float amountOfStat, float precentage, bool isRelativeToMax)
        => ((isRelativeToMax && GetStat.GetIsCapped) ? GetStat.maxStat.GetSetValue : amountOfStat) * precentage / 100f;
    private void Begin()
    {
        Stop();
        switch (effectData.EffectType)
        {
            case EffectType.Instant:
                AddFixedAmount();
                return;
            case EffectType.Toggle:
                ToggleAmountOverTime();
                break;
            case EffectType.OverTime:
                effectCoro = EffectHandler._instance.StartCoroutine(AddEffectOverTime());
                break;
            default:
                return;
        }
    }
    public void Stop()
    {
        if (effectCoro != null)
        {
            effectHandler.StopCoroutine(effectCoro);
            effectCoro = null;
        }
    }
    // instantly add/remove fixed amount 
    private void AddFixedAmount()
        => statValue += (effectData.InPercentage) ? AmountFromPercentage(statValue, effectData.Amount, effectData.IsRelativeToMax) : effectData.Amount;
    //  Add/remove fixed amount -> wait -> return to previous State
    private void ToggleAmountOverTime()
    {
        float toggleAmount = effectData.InPercentage ? AmountFromPercentage(statValue, effectData.Amount, effectData.IsRelativeToMax) : effectData.Amount;
        statValue += toggleAmount;
        //Disable amount
        effectCoro = effectHandler.StartCoroutine(ResetToggleAmount(toggleAmount));
    }

    private IEnumerator ResetToggleAmount(float amount)
    {
        yield return new WaitForSeconds(effectData.Duration);
        statValue -= amount;
    }
    // Add/remove small amount -> wait -> return to previous State
    private IEnumerator AddEffectOverTime()
    {
        float startingTime = Time.time;
        if (effectData.InPercentage)
        {
            while (startingTime + effectData.Duration > Time.time)
            {
                statValue += (Mathf.Pow((1 + (effectData.Amount / 100)), effectData.TickTime) - 1) * ((effectData.IsRelativeToMax && GetStat.GetIsCapped) ? GetStat.maxStat.GetSetValue : statValue);
                yield return new WaitForSeconds(effectData.TickTime);
            }
        }
        else
        {
            while (startingTime + effectData.Duration > Time.time)
            {
                statValue += effectData.Amount * effectData.TickTime;
                yield return new WaitForSeconds(effectData.TickTime);
            }

        }
    }
}
