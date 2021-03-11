using Assets.StateMachine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public partial class Unit : Entity
{
    abstract class UnitState : State
    {
        protected Unit unit;

        protected UnitState(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }
    }
    class WalkState : UnitState
    {
        LevelManager levelManager;
        public WalkState(Unit unit) : base(unit)
        {
        }
        protected override void OnEnable()
        {
            levelManager = LevelManager._instance;
            unit.navScript.StartMove();
        }
        protected override void OnUpdate()
        {

            EntityHit hit = levelManager.GetClosestEnemy(unit.gameObject);
            if (hit.distance <= unit.stats.GetStatValue(StatType.Range))
                unit.state.State = new AttackState(unit);
        }
        protected override void OnDisable()
        {
            unit.navScript.StopMove();
        }
    }
    class AttackState : UnitState
    {
        LevelManager levelManager;
        EntityHit hit;
        Coroutine attackCoro;
        public AttackState(Unit unit) : base(unit)
        {
        }
        protected override void OnEnable()
        {
            levelManager = LevelManager._instance;
            hit = levelManager.GetClosestEnemy(unit.gameObject);
            attackCoro = unit.StartCoroutine(StartAttacking());
        }
        protected override void OnUpdate()
        {
            if (hit.entity == null)
            {
                hit = levelManager.GetClosestEnemy(unit.gameObject);
                if (hit.distance > unit.stats.GetStatValue(StatType.Range) || hit.entity == null)
                    unit.state.State = new WalkState(unit);
            }
        }
        protected override void OnDisable()
        {
            Stop();
        }
        IEnumerator StartAttacking()
        {
            float attackDelay = 1 / unit.stats.GetStatValue(StatType.AttackSpeed);
            do
            {
                if (Time.time >= unit.lastAttackTime + attackDelay)
                {
                    unit.lastAttackTime = Time.time;
                    unit.transform.DOLocalRotate(unit.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);
                    GameObject projectile = Instantiate(unit.projectile, unit.transform.position, Quaternion.identity);
                    projectile.GetComponent<Projectile>().Init(unit, hit.entity);
                }
                yield return null;
            } while (attackCoro != null && hit.entity != null);
        }
        void Stop()
        {
            if (attackCoro != null)
            {
                unit.StopCoroutine(attackCoro);
                attackCoro = null;
            }
        }
    }
}
