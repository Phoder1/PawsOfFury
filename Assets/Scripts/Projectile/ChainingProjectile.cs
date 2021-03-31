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
    float[] originalAmounts;
    int currentBounce = 0;
    List<Entity> hitHistory;

    void OnEnable()
    {
        originalAmounts = Array.ConvertAll(effects, (x) => x.amount);
        endEffectAmounts = Array.ConvertAll(originalAmounts, (x) => x * endEffectAmountRatio);
        if (forceUniqueTargets)
            hitHistory = new List<Entity>();
    }
    public  override void ReachedTarget()
    {
        var spellObject = Instantiate(spell, transform.position, Quaternion.identity);
        ProjectileSpell projectileSpell = spellObject.GetComponent<ProjectileSpell>();
        projectileSpell.Init(attackingEntity, target, effects ,callback);

        transform.DOComplete();
        if (forceUniqueTargets && target != null)
            hitHistory.Add(target);
        if (currentBounce >= chainCount)
            DestroyProjectile();
        else
        {
            for (int i = 0; i < effects.Length; i++)
                effects[i].amount = Mathf.Lerp(originalAmounts[i], endEffectAmounts[i], ((float)currentBounce + 1) / (float)chainCount);

            Targets.EntityHitCondition newTargetCondition;
            if (forceUniqueTargets)
                newTargetCondition = (x) => !hitHistory.Contains(x);
            else
                newTargetCondition = (x) => x != target;

            Entity newTarget = Targets.GetEntityHit(transform.position, targeting, newTargetCondition, attackingEntity);
            currentBounce++;
            if (newTarget != null)
                Init(attackingEntity, newTarget, callback, transform.position);
            else
                DestroyProjectile();
        }
    }
}
