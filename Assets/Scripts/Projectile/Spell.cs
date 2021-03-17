using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string entityName;
    [SerializeField] EffectDataSO[] effectsData;
    [Tooltip("The radius of the spell.")]
    [SerializeField] float aoeRadius;
    protected Entity attackingEntity;
    protected Action<Entity[], Entity> callback;
    public EffectData[] effects;
    protected virtual void OnEnable()
    {
        effects = Array.ConvertAll(effectsData, (x) => (EffectData)x);
    }
    public void CastSpell(Action<Entity[], Entity> callback = null)
    {
        if (callback != null)
            this.callback = callback;
        //Todo: Animation and sound
        End(ApplyEffect().ToArray());
    }
    protected List<Entity> ApplyEffect()
    {
        List<Entity> hitEntities = new List<Entity>();
        Entity[] collisions = Array.ConvertAll(Physics.OverlapSphere(transform.position, aoeRadius), (x) => x.gameObject.GetComponent<Entity>());
        foreach (Entity entity in collisions)
        {
            if (entity != null && !hitEntities.Contains(entity))
            {
                foreach (EffectData effect in effects)
                {
                    if (effect != null
                        && ((entity is Enemy && effect.targets.HasFlag(TargetTypes.Enemy) && entity != attackingEntity)
                        || (entity is Unit && effect.targets.HasFlag(TargetTypes.Unit) && entity != attackingEntity)
                        || (effect.targets.HasFlag(TargetTypes.Self) && entity == attackingEntity)))
                    {
                        hitEntities.Add(entity);
                        new EffectController(entity.stats, effect);
                    }
                }
            }
        }
        return hitEntities;
    }
    protected virtual void End(Entity[] hitEntities)
    {
        transform.DOComplete();
        callback?.Invoke(hitEntities, null);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}