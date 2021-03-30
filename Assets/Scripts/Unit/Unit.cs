using Assets.StateMachine;
using System;
using UnityEngine;
using Assets.Stats;
using System.Collections.Generic;
using static BlackBoard;


[RequireComponent(typeof(NavScript))]
public class Unit : Entity
{
    protected override EntityState DefaultState() => new WalkState(this);
    protected virtual EntityState AttackingState => new AttackState(this);
    protected NavScript navScript;

    protected override void Start()
    {
        navScript = GetComponent<NavScript>();
        base.Start();
    }

    protected override void OnTargetLoss() => stateMachine.State = new WalkState(this);
    protected override void DetectedOutOfRange(Entity detectedEntity)
    {
        navScript.SetDestination(detectedEntity.transform.position);
        navScript.StartMove();
    }

    public override void DetectedInRange(EntityHit detectedEntity) => navScript.StopMove();

    class UnitState : EntityState
    {
        public UnitState(Unit unit) : base(unit) { }
        protected Unit Unit => (Unit)entity;
    }
    class WalkState : UnitState
    {
        public WalkState(Unit unit) : base(unit) { }
        protected override void OnEnable()
        {
            Unit.navScript.SetDestination(BlackBoard.levelManager.LevelEndPos);
            Unit.navScript.StartMove();
        }
        protected override void OnUpdate()
        {
            detectedEntity = Targets.GetEntityHit(entity, entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
                Unit.stateMachine.State = Unit.AttackingState;
            //Debug.Log(Unit.navScript.agent.velocity.x < 0 ? "Left" : "Right");
        }

        protected override void OnDisable() => Unit.navScript.StopMove();
    }
}

