using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainingProjectile : Projectile
{
    [Min(2)]
    [Tooltip("The number of Bounces.")]
    [SerializeField] int chainCount = 2;
    [Tooltip("The effect amount at the last chain projectile, equally easing the amount every bounce.")]
    [SerializeField] float endEffectAmountRatio;
    [SerializeField] TargetingSO targeting;
    [SerializeField] bool forceUniqueTargets;

    float[] endEffectAmounts;
    EffectData[] originalEffects;
    int currentBounce = 0;
    List<Entity> hitHistory;

    protected override void OnEnable()
    {
        base.OnEnable();
        endEffectAmounts = Array.ConvertAll(effects, (x) => x.amount * endEffectAmountRatio);
        originalEffects = effects;
        if (forceUniqueTargets)
            hitHistory = new List<Entity>();
    }
    protected override void End(Entity[] hitEntities)
    {
        transform.DOComplete();
        if (forceUniqueTargets)
            hitHistory.Add(target);
        if (currentBounce >= chainCount)
            ChainEnd(hitEntities);
        else
        {
            for (int i = 0; i < effects.Length; i++)
                effects[i].amount = Mathf.Lerp(originalEffects[i].amount, endEffectAmounts[i], ((float)currentBounce+1) / (float)chainCount);
            Targets.EntityHitCondition newTargetCondition = (x) => x != target;
            if (forceUniqueTargets)
                newTargetCondition = (x) => !hitHistory.Contains(x) && x != attackingEntity;
            Entity newTarget = Targets.GetEntityHit(transform.position, targeting, newTargetCondition, attackingEntity);
            currentBounce++;
            if (newTarget != null)
                Init(attackingEntity, newTarget, callback, transform.position);
            else
                ChainEnd(hitEntities);
        }
    }
    void ChainEnd(Entity[] hitEntities)
    {
        //callback?.Invoke(hitEntities, target);
        transform.DOComplete();
        Destroy(gameObject);
    }
}
