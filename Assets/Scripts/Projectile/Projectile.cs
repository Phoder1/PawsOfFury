using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{
    [Tooltip("Controls the speed of the projectile.")]
    [SerializeField] float speed;
    [Tooltip("AOE = Area Of Effect, the projectile will hit multiple targets when enabled.")]
    [SerializeField] bool AOE;
    [Tooltip("WIP")]
    [SerializeField] float maxHeight;

    protected Entity target;
    public void Init(Entity attackingEntity, Entity target, Action<Entity[], Entity> callback = null, Vector3? originPosition = null)
    {
        transform.DOComplete();
        if (target == null)
            return;
        this.target = target;
        this.attackingEntity = attackingEntity;
        if (callback != null)
            this.callback = callback;
        if (originPosition == null)
            originPosition = attackingEntity.transform.position;
        transform.position = (Vector3)originPosition;
        if (attackingEntity != target)
        {
            float moveTime = Vector3.Distance((Vector3)originPosition, target.transform.position) / speed;
            Vector3 targetPos = target.transform.position;
            transform.DOMove(targetPos, moveTime).OnComplete(ReachedTarget);
        }
        else
            ReachedTarget();
    }
    void ReachedTarget()
    {
        //Todo: Animation and sound
        List<Entity> hitEntities = new List<Entity>();
        if (target != null && !AOE)
        {
            Array.ForEach(effects, (x) => new EffectController(target.stats, x));
            hitEntities.Add(target);
        }
        else if (AOE)
        {
            hitEntities = ApplyEffect();
        }
        End(hitEntities.ToArray());
    }
}
