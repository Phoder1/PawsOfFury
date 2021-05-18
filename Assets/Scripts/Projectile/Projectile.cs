using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool hasTravelTime = true;
    [ShowIf("@hasTravelTime"), Tooltip("Controls the speed of the projectile.")]
    [SerializeField] float speed;
    [SerializeField] private bool travelsToTarget = true;
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

        Vector3 targetPos;
        if (travelsToTarget)
            targetPos = target.transform.position;
        else
            targetPos = originPosition.Value;



        transform.position = (Vector3)originPosition;

        if (hasTravelTime && attackingEntity != target)
        {
            

            float moveTime = Vector3.Distance((Vector3)originPosition, target.transform.position) / speed;
            transform.DOMove(targetPos, moveTime).OnComplete(ReachedTarget);
        }
        else
        {
            transform.position = targetPos;
            ReachedTarget();
        }
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
