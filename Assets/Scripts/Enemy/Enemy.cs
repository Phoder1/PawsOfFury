using Assets.StateMachine;
using UnityEngine;
using Assets.Stats;
using System.Collections.Generic;

public class Enemy : Entity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnUpdate()
    {
    }
    protected override void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, healthBar, maxHp, 
            new List<Reaction> {
            new Reaction(new Death(),
            (value) => {DestroyEntity(); })
            }
            ));
        stats.Add(new Stat(this, StatType.Damage, defualtStats.Damage));
        stats.Add(new Stat(this, StatType.AttackSpeed, defualtStats.AttackSpeed));
        stats.Add(new Stat(this, StatType.Range, defualtStats.Range));
    }
    public void DestroyEntity()
    {
        if (this != null)
            Destroy(gameObject);
    }

    protected override void OnTargetLoss()
    {
        stateMachine.State = new SearchState(this);
    }

    protected override EntityState DefaultState() => new SearchState(this);

    class EnemyState : EntityState
    {
        public EnemyState(Enemy enemy) : base(enemy)
        {
        }

        protected Enemy Enemy => (Enemy)entity;

    }
    class SearchState : EnemyState
    {
        public SearchState(Enemy enemy) : base(enemy)
        {
        }
        protected override void OnUpdate()
        {
            //Todo: detection range
            detectedEntity = entity.GetEntityHit(entity.stats.GetStatValue(StatType.Range), entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
                Enemy.stateMachine.State = new AttackState(entity);
        }
    }
}
