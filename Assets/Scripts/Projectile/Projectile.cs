using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool AOE;
    [SerializeField] EffectDataSO[] effectsData;
    [SerializeField] float radius;
    [SerializeField] int chainCount;
    [SerializeField] float maxHeight;

    Entity target;
    Entity attackingEntity;
    Action<Entity[], Entity> callback;
    public EffectData[] effects;
    private void OnEnable()
    {
        effects = new EffectData[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++)
            effects[i] = effectsData[i].EffectData;
    }
    public void Init(Entity attackingEntity, Entity target, Action<Entity[], Entity> callback = null)
    {
        this.target = target;
        this.attackingEntity = attackingEntity;
        this.callback = callback;
        //Todo: Animation and sound
        if (attackingEntity != target)
        {
            float moveTime = Vector3.Distance(transform.position, target.transform.position) / speed;
            if (maxHeight != 0)
            {
                Vector3 peakHeight = transform.position + (target.transform.position - transform.position) / 2;
                peakHeight.y += maxHeight;

            }
            else
            {
                transform.DOMove(target.transform.position, moveTime).OnComplete(ReachedTarget);
            }
        }
        else
        {
            ReachedTarget();
        }

    }

    void ReachedTarget()
    {
        //Todo: Move to projectile
        //Todo: Animation and sound
        List<Entity> hitEntities = new List<Entity>();
        if (target != null && !AOE)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                new EffectController(target.stats, effects[i]);
            }
            hitEntities.Add(target);
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider collider in colliders)
            {
                Entity entity = collider.gameObject.GetComponent<Entity>();
                if (entity != null)
                {
                    for (int i = 0; i < effects.Length; i++)
                    {
                        switch (entity)
                        {
                            case Enemy enemy:
                                if (effects[i].targets.HasFlag(TargetTypes.Enemy))
                                {
                                    new EffectController(enemy.stats, effects[i]);
                                    Debug.Log(attackingEntity.name +":" + effects[i].amount);
                                    if (!hitEntities.Contains(enemy))
                                        hitEntities.Add(enemy);
                                }
                                break;
                            case Unit unit:
                                if ((effects[i].targets.HasFlag(TargetTypes.Unit) && unit != this.attackingEntity)
                                    || (effects[i].targets.HasFlag(TargetTypes.Self) && unit == this.attackingEntity))
                                {
                                    new EffectController(unit.stats, effects[i]);
                                    if (!hitEntities.Contains(unit))
                                        hitEntities.Add(unit);
                                }
                                break;
                        }
                    }

                }
            }

        }
        Destroy(gameObject);
        callback?.Invoke(hitEntities.ToArray(), target);
        //Check hit
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
