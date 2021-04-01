using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : Spell
{
    [Tooltip("AOE = Area Of Effect, the projectile will hit multiple targets when enabled.")]
    [SerializeField] bool AOE;
    Entity attackingEntity;
    Entity target;
    
    public void Init(Entity attackingEntity, Entity target, EffectData[] effects, Action<Entity[], Entity> callback = null)
    {
        if (effects != null)
            this.effects = effects;
        this.target = target;
        this.attackingEntity = attackingEntity;
        if (callback != null)
            this.callback = callback;
        if (!hasParticles)
        {
            ApplyEffect();
            DestroySpell();
        }

    }
    public static void CastSpell(GameObject spell, Vector3 position, Entity attackingEntity, Entity target, EffectData[] effects, Action<Entity[], Entity> callback = null)
    {
        var spellObject = Instantiate(spell, position, Quaternion.identity);
        ProjectileSpell projectileSpell = spellObject.GetComponent<ProjectileSpell>();
        projectileSpell.Init(attackingEntity, target, effects, callback);
    }
    protected override List<Entity> GetTargets()
    {
        List<Entity> hitEntities = new List<Entity>();
        if (target == null)
            return hitEntities;
        if (!AOE)
        {
            hitEntities.Add(target);
            return hitEntities;
        }
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
                    }
                }
            }
        }
        return hitEntities;
    }
}
