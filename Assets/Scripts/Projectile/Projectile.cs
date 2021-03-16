using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Controls the speed of the projectile.")]
    [SerializeField] float speed;
    [SerializeField] EffectDataSO[] effectsData;
    [Tooltip("AOE = Area Of Effect, the projectile will hit multiple targets when enabled.")]
    [SerializeField] bool AOE;
    [Tooltip("The radius of the damage when AOE is enabled.")]
    [SerializeField] float aoeRadius;
    [Tooltip("WIP")]
    [SerializeField] float maxHeight;

    protected Entity target;
    protected Entity attackingEntity;
    protected Action<Entity[], Entity> callback;
    public EffectData[] effects;
    protected virtual void OnEnable()
    {
        effects = new EffectData[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++)
            effects[i] = effectsData[i].EffectData;
    }
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
            for (int i = 0; i < effects.Length; i++)
                new EffectController(target.stats, effects[i]);
            hitEntities.Add(target);
        }
        else if (AOE)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);
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
                                    if (!hitEntities.Contains(enemy))
                                        hitEntities.Add(enemy);
                                    new EffectController(enemy.stats, effects[i]);
                                }
                                break;
                            case Unit unit:
                                if ((effects[i].targets.HasFlag(TargetTypes.Unit) && unit != this.attackingEntity)
                                    || (effects[i].targets.HasFlag(TargetTypes.Self) && unit == this.attackingEntity))
                                {
                                    if (!hitEntities.Contains(unit))
                                        hitEntities.Add(unit);
                                    new EffectController(unit.stats, effects[i]);
                                }
                                break;
                        }
                    }
                }
            }
        }
        End(hitEntities.ToArray());
    }
    protected virtual void End(Entity[] hitEntities)
    {
        transform.DOComplete();
        callback?.Invoke(hitEntities, target);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
