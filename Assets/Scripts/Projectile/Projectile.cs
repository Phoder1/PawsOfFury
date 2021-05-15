using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Controls the speed of the projectile.")]
    [SerializeField] float speed;
    [SerializeField] protected GameObject spell;
    [SerializeField] EffectDataSO[] effectsData;
    protected Entity attackingEntity;
    protected Action<Entity[], Entity> callback;

    protected Entity target;
    public event Action OnProjectileAnimation;
    public EffectData[] effects;
    public void ProjectileAnimRecall() => OnProjectileAnimation?.Invoke();


    protected virtual void OnEnable()
    {
        effects = Array.ConvertAll(effectsData, (x) => (EffectData)x);
    }
    public void Init(Entity attackingEntity, Entity target, Action<Entity[], Entity> callback = null, Vector3? originPosition = null)
    {
        transform.DOComplete();
        this.target = target;
        this.attackingEntity = attackingEntity;
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
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
    public virtual void ReachedTarget()
    {
        ProjectileSpell.CastSpell(spell, transform.position, attackingEntity, target, effects, callback);
        DestroyProjectile();
    }
    protected virtual void DestroyProjectile()
    {
        transform.DOComplete();
        Destroy(gameObject);
    }
    

}
