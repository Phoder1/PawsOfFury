using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float travelTime;
    [SerializeField] bool AOE;
    [SerializeField] EffectDataSO effectData;
    [SerializeField] float radius;
    [SerializeField] TargetTypes targets;

    Entity target;
    Entity attackingEntity;
    Action<Collider[]> callback;
    public void Init(Entity attackingEntity, Entity target, Action<Collider[]> callback = null)
    {
        this.target = target;
        this.attackingEntity = attackingEntity;
        this.callback = callback;
        //Todo: Animation and sound
        transform.DOMove(target.transform.position, 1).OnComplete(ReachedTarget);

    }

    void ReachedTarget()
    {
        //Todo: Move to projectile
        //Todo: Animation and sound
        if (target != null && !AOE)
            new EffectController(target.stats, effectData);
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            List<Collider> relevantColliders = new List<Collider>();
            foreach (Collider collider in colliders)
            {
                Entity entity = collider.gameObject.GetComponent<Entity>();
                if (entity != null)
                {
                    switch (entity)
                    {
                        case Enemy enemy:
                            if (targets.HasFlag(TargetTypes.Enemy))
                            {
                                new EffectController(enemy.stats, effectData);
                                relevantColliders.Add(collider);
                            }
                            break;
                        case Unit unit:
                            if ((targets.HasFlag(TargetTypes.Unit) && unit != this.attackingEntity)
                                || (targets.HasFlag(TargetTypes.Self) && unit == this.attackingEntity))
                            {
                                new EffectController(unit.stats, effectData);
                                relevantColliders.Add(collider);
                            }
                            break;
                    }

                }
            }
            
        }
        Destroy(gameObject);
        //Check hit
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

class ProjectileHit
{


}