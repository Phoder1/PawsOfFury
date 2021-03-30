using Assets.Stats;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] int goldReward;
    protected override void OnTargetLoss() => stateMachine.State = new SearchState(this);
    protected override EntityState DefaultState() => new SearchState(this);
    class EnemyState : EntityState
    {
        protected Enemy Enemy => (Enemy)entity;
        public EnemyState(Enemy enemy) : base(enemy) { }
    }
    class SearchState : EnemyState
    {
        public SearchState(Enemy enemy) : base(enemy) { }
        protected override void OnUpdate()
        {
            detectedEntity = Targets.GetEntityHit(entity, entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
                Enemy.stateMachine.State = new AttackState(entity);
        }
    }
}
