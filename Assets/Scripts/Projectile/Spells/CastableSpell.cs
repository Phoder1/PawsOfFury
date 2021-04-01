using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastableSpell : Spell
{
    public Sprite buttonSprite;
    [SerializeField] EffectDataSO[] effectsData;
    public int goldValue;
    void OnEnable()
    {
        effects = Array.ConvertAll(effectsData, (x) => (EffectData)x);
    }
    public void Init(Action<Entity[], Entity> callback = null)
    {
        if (callback != null)
            this.callback = callback;
        if (!hasParticles)
        {
            ApplyEffect();
            DestroySpell();
        }
    }
    protected override List<Entity> GetTargets()
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
                        && ((entity is Enemy && effect.targets.HasFlag(TargetTypes.Enemy))
                        || (entity is Unit && effect.targets.HasFlag(TargetTypes.Unit))
                        || (effect.targets.HasFlag(TargetTypes.Self))))
                    {
                        hitEntities.Add(entity);
                    }
                }
            }
        }
        return hitEntities;

    }
}
