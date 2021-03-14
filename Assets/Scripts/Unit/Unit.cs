using Assets.StateMachine;
using System;
using UnityEngine;
using Assets.Stats;
using System.Collections.Generic;

public enum StatType
{
    HP,
    MaxHP,
    WalkSpeed,
    Damage,
    AttackSpeed,
    Range
}
[RequireComponent(typeof(NavScript))]
public class Unit : Entity
{
    protected NavScript navScript;

    protected override void Start()
    {
        navScript = GetComponent<NavScript>();
        base.Start();
    }
    protected override void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, healthBar, maxHp,
            new List<Reaction> {
            new Reaction(Reaction.DeathCondition,
            (value) => {Destroy(gameObject); })
            }
            ));
        stats.Add(new Stat(this, StatType.Damage, defualtStats.Damage));
        stats.Add(new Stat(this, StatType.AttackSpeed, defualtStats.AttackSpeed));
        stats.Add(new Stat(this, StatType.WalkSpeed, defualtStats.WalkSpeed));
        stats.Add(new Stat(this, StatType.Range, defualtStats.Range));
    }

    protected override void OnTargetLoss()
    {
        stateMachine.State = new WalkState(this);
    }
    protected override void DetectedOutOfRange()
    {

    }
    protected override EntityState DefaultState() => new WalkState(this);

    class UnitState : EntityState
    {
        protected Unit Unit => (Unit)entity;
        public UnitState(Unit unit) : base(unit)
        {
        }
    }
    class WalkState : UnitState
    {
        public WalkState(Unit unit) : base(unit)
        {
        }
        protected override void OnEnable()
        {
            Unit.navScript.StartMove();
        }
        protected override void OnUpdate()
        {
            //Todo: detection range
            detectedEntity = entity.GetEntityHit(entity.stats.GetStatValue(StatType.Range), entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
                Unit.stateMachine.State = new AttackState(entity);
        }
        protected override void OnDisable()
        {
            Unit.navScript.StopMove();
        }
    }
}

