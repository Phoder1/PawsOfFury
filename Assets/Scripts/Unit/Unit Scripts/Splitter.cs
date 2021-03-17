using Assets.StateMachine;
using DG.Tweening;
using UnityEngine;

public class Splitter : Unit
{
    [Min(2)]
    [SerializeField] int maxAttackCount;
    protected override EntityState AttackingState => new SplitAttackState(this);
    class SplitAttackState : AttackState
    {
        Splitter Splitter => (Splitter)entity;
        public SplitAttackState(Splitter splitter) : base(splitter) {
        }
        protected override void Attack()
        {
            DetectedInRange(detectedEntity);
            entity.lastAttackTime = Time.time;
            entity.transform.DOLocalRotate(entity.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);
            for (int i = 0; i < Mathf.Min(possibleTargets.Count, Splitter.maxAttackCount); i++)
            {
                GameObject projectile = Instantiate(entity.projectile.gameobject, entity.transform.position, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                foreach (EffectData effectData in projectileScript.effects)
                    effectData.amount /= Mathf.Min(possibleTargets.Count, Splitter.maxAttackCount);
                projectileScript.Init(entity, possibleTargets[i].entity, callback: entity.projectile.callback);
            }
        }

    }
}
