using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Assets.StateMachine;

public partial class Unit : MonoBehaviour
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
            unit.navScript.StartMove(levelManager.levelEndPos);
        }
        protected override void OnUpdate()
        {

            EntityHit<Enemy> hit = levelManager.GetClosestEnemy(unit.gameObject);
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
        EntityHit<Enemy> hit;
        Coroutine attackCoro;
        public AttackState(Unit unit) : base(unit)
        {
        }
        protected override void OnEnable()
        {
            levelManager = LevelManager._instance;
            hit = levelManager.GetClosestEnemy(unit.gameObject);
            attackCoro = unit.StartCoroutine(Attack(0));
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
        IEnumerator Attack(float delay)
        {
            if (delay != 0)
                yield return new WaitForSeconds(delay);
            Debug.Log("ATTACKK!!");
            if (hit.entity != null)
            {
                float attackDelay = 1 / unit.stats.GetStatValue(StatType.AttackSpeed);
                //Todo: Animation and sound
                unit.transform.DOLocalRotate(unit.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);
                GameObject projectile = Instantiate(unit.projectile, unit.transform.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().Init(hit.entity.transform.position, hit.entity.stats);
                attackCoro = unit.StartCoroutine(Attack(attackDelay));
            }
        }
        void Stop()
        {
            if (attackCoro != null)
                unit.StopCoroutine(attackCoro);
        }
    }
}
