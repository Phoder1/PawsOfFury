using System.Collections;
using UnityEngine;
using Assets.Stats;


public class EffectController
{
    private readonly EffectData effectData;
    private readonly EntityStats stats;
    Coroutine effectCoro;
    private readonly CoroutineHandler effectHandler;


    private Stat GetStat
    {
        get
        {
            if (stats != null)
                return stats.GetStat(effectData.affectedStat);
            return null;

        }
    }
    public EffectController(EntityStats stats, EffectData effectData)
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
            switch (effectData.effectType)
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
        GetStat.GetSetValue += (effectData.inPercentage) ? AmountFromPercentage(GetStat.GetSetValue, effectData.amount, effectData.isRelativeToMax) : effectData.amount;

    }
    //  Add/remove fixed amount -> wait -> return to previous State
    private void ToggleAmountOverTime()
    {
        float toggleAmount = effectData.inPercentage ? AmountFromPercentage(GetStat.GetSetValue, effectData.amount, effectData.isRelativeToMax) : effectData.amount;
        GetStat.GetSetValue += toggleAmount;
        //Disable amount
        effectCoro = effectHandler.StartCoroutine(ResetToggleAmount(toggleAmount));
    }

    private IEnumerator ResetToggleAmount(float amount)
    {
        Debug.Log("Toggle Coro");
        yield return new WaitForSeconds(effectData.duration);
        if (GetStat != null)
            GetStat.GetSetValue -= amount;
    }
    // Add/remove small amount -> wait -> return to previous State
    private IEnumerator AddEffectOverTime()
    {
        Debug.Log("DOT Coro");
        float startingTime = Time.time;
        if (effectData.inPercentage)
        {
            float amountRatio = effectData.amount / 100;
            for (int i = 0; i < Mathf.FloorToInt(effectData.duration / effectData.tickTime); i++)
            {
                if (GetStat == null)
                    yield break;
                float tickAmount = amountRatio * ((effectData.isRelativeToMax && GetStat.GetIsCapped) ? GetStat.maxStat.GetSetValue : GetStat.GetSetValue);
                GetStat.GetSetValue += tickAmount;
                yield return new WaitForSeconds(effectData.tickTime);
            }
        }
        else
        {
            while (startingTime + effectData.duration > Time.time)
            {
                if (GetStat == null)
                    yield break;
                GetStat.GetSetValue += effectData.amount * effectData.tickTime;
                yield return new WaitForSeconds(effectData.tickTime);
            }

        }
    }
}
