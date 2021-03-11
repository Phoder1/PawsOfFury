using System.Collections;
using UnityEngine;
using static EntityStats;

public class EffectController
{
    private readonly EffectDataSO effectData;
    private readonly EntityStats stats;
    Coroutine effectCoro;
    private readonly CoroutineHandler effectHandler;

    private Stat GetStat
    {
        get
        {
            if (stats != null)
                return stats.GetStat(effectData.TargetStat);
            return null;

        }
    }
    public EffectController(EntityStats stats, EffectDataSO effectData)
    {
        this.effectData = effectData;
        this.stats = stats;
        effectHandler = CoroutineHandler._instance;
        Begin();
    }
    private float AmountFromPercentage(float amountOfStat, float precentage, bool isRelativeToMax)
        => ((isRelativeToMax && GetStat.GetIsCapped) ? GetStat.maxStat.GetSetValue : amountOfStat) * precentage / 100f;
    private void Begin()
    {
        Stop();
        if (GetStat != null)
        {
            switch (effectData.EffectType)
            {
                case EffectType.Instant:
                    AddFixedAmount();
                    return;
                case EffectType.Toggle:
                    ToggleAmountOverTime();
                    break;
                case EffectType.OverTime:
                    effectCoro = CoroutineHandler._instance.StartCoroutine(AddEffectOverTime());
                    break;
                default:
                    return;
            }
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
    {
        GetStat.GetSetValue += (effectData.InPercentage) ? AmountFromPercentage(GetStat.GetSetValue, effectData.Amount, effectData.IsRelativeToMax) : effectData.Amount;

    }
    //  Add/remove fixed amount -> wait -> return to previous State
    private void ToggleAmountOverTime()
    {
        float toggleAmount = effectData.InPercentage ? AmountFromPercentage(GetStat.GetSetValue, effectData.Amount, effectData.IsRelativeToMax) : effectData.Amount;
        GetStat.GetSetValue += toggleAmount;
        //Disable amount
        effectCoro = effectHandler.StartCoroutine(ResetToggleAmount(toggleAmount));
    }

    private IEnumerator ResetToggleAmount(float amount)
    {
        Debug.Log("Toggle Coro");
        yield return new WaitForSeconds(effectData.Duration);
        if (GetStat != null)
            GetStat.GetSetValue -= amount;
    }
    // Add/remove small amount -> wait -> return to previous State
    private IEnumerator AddEffectOverTime()
    {
        Debug.Log("DOT Coro");
        float startingTime = Time.time;
        if (effectData.InPercentage)
        {
            float amountRatio = effectData.Amount / 100;
            for (int i = 0; i < Mathf.FloorToInt(effectData.Duration / effectData.TickTime); i++)
            {
                if (GetStat == null)
                    yield break;
                float tickAmount = amountRatio * ((effectData.IsRelativeToMax && GetStat.GetIsCapped) ? GetStat.maxStat.GetSetValue : GetStat.GetSetValue);
                GetStat.GetSetValue += tickAmount;
                yield return new WaitForSeconds(effectData.TickTime);
            }
        }
        else
        {
            while (startingTime + effectData.Duration > Time.time)
            {
                if (GetStat == null)
                    yield break;
                GetStat.GetSetValue += effectData.Amount * effectData.TickTime;
                yield return new WaitForSeconds(effectData.TickTime);
            }

        }
    }
}
