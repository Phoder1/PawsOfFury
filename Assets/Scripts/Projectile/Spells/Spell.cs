using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public string spellName;
    [Tooltip("The radius of the spell.")]
    [SerializeField] protected float aoeRadius;
    protected Action<Entity[], Entity> callback;
    public EffectData[] effects;
    [SerializeField] protected bool hasParticles;
    public void ApplyEffect()
    {
        Entity[] targets = GetTargets().ToArray();
        foreach (Entity entity in targets)
            foreach(EffectData effect in effects)
                new EffectController(entity.stats, effect);
        callback?.Invoke(targets, null);
    }
    protected abstract List<Entity> GetTargets();
    public virtual void DestroySpell()
    {
        transform.DOComplete();
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}