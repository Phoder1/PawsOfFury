using Assets.Stats;
using System.Collections.Generic;
using UnityEngine;
using static IngameBlackBoard;

public class Enemy : Entity
{
    protected override void OnTargetLoss() => stateMachine.State = new SearchState(this);
    protected override EntityState DefaultState() => new SearchState(this);
    protected override void Start()
    {
        base.Start();
        Init();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        levelManager.Gold += goldValue;

        if (Settings_UI.Vibrations)
        {
            Handheld.Vibrate();
        }
    }
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
