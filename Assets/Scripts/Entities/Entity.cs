using Assets.StateMachine;
using Assets.Stats;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static IngameBlackBoard;

[Flags]
public enum EntityType
{
    Healer = 1,
    DPS = 2,
    Tank = 4,
    Buffer = 8,
    Ghost = 16
}
[RequireComponent(typeof(Collider))]
public abstract class Entity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string entityName;
    public int goldValue;
    [SerializeField] EntityType type;
    [SerializeField] protected GameObject UiObject;
    [SerializeField] protected float healthbarHeight;
    [SerializeField] protected DefualtStats defualtStats;
    [SerializeField] protected Transform FireOrigin;

    public ProjectileData projectile;
    [SerializeField] protected AuraData aura;
    [Tooltip("Height 0 is the center of the Unit.")]

    [SerializeField]
    public EntityStats stats;
    protected Animator animator;
    protected Camera mainCam;
    protected EntityUI ui;
    protected StateMachine<EntityState> stateMachine;
    [HideInInspector]
    public float lastAttackTime;

    public EntityType Type => type;

    protected abstract EntityState DefaultState();
    public event Action OnDestroyEvent;

    public event Action OnAttackAnimation;
    public void AttackAnimRecall()
    {
        OnAttackAnimation?.Invoke();
        stateMachine.State.AnimationRecall();
    }

    [HideInInspector]
    public bool selected;
    public bool Selected
    {
        get => selected;
        set
        {
            if (selected != value)
            {
                selected = value;
                ui.Selected = value;
                if (selected)
                {
                    inputManager.ResetSelection();
                    inputManager.OnResetSelection += DisableSelection;
                }
                else
                    inputManager.OnResetSelection -= DisableSelection;

            }
        }
    }


    void DisableSelection() => Selected = false;


    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCam = Camera.main;
        lastAttackTime = -Mathf.Infinity;
        stats = new EntityStats();
        UiObject = Instantiate(UiObject, mainCam.WorldToScreenPoint(transform.position), transform.rotation, levelManager.EntityUIsObj);
        levelManager.AddToList(this);
        ui = UiObject.GetComponent<EntityUI>();
        ui.HealthBarHeight = healthbarHeight;
        FillDictionary();
        stateMachine = new StateMachine<EntityState>(null);
        CastAura();
    }
    public void Init() => stateMachine.State = DefaultState();
    public void OnPointerUp(PointerEventData eventData) => Selected = !Selected;
    public void OnPointerDown(PointerEventData eventData) { }
    protected virtual void Update()
    {
        stateMachine.Update();
        UiObject.transform.position = mainCam.WorldToScreenPoint(transform.position);
        
    }
    protected virtual void OnDestroy()
    {
        transform.DOComplete();
        levelManager.RemoveFromList(this);
        if (OnDestroyEvent != null)
            Debug.Log(OnDestroyEvent.GetInvocationList().Length);
        OnDestroyEvent?.Invoke();
        stateMachine.State = null;
    }
    private void CastAura()
    {
        if (aura != null && aura.gameobject != null)
        {
            GameObject auraObj = Instantiate(aura.gameobject, transform.position, Quaternion.identity);
            auraObj.GetComponent<Projectile>().Init(this, this, aura.callback);
            float castDelay = 1 / aura.rate;
            Invoke(nameof(CastAura), castDelay);
        }
    }
    protected virtual void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, ui, maxHp, new Reaction(Reaction.DeathCondition, (value) => { Destroy(gameObject); })));
        stats.Add(new Stat(this, StatType.DamageMultiplier, 100));
        Stat maxAttackSpeed = new Stat(this, StatType.MaxAttackSpeedMultiplier, defualtStats.MaxAttackSpeedMultiplier);
        stats.Add(maxAttackSpeed);
        stats.Add(new Stat(this, StatType.AttackSpeedMultiplier, 1, maxAttackSpeed, new Reaction(Reaction.AlwaysTrue, (value) => { if(animator) animator.speed = value.GetSetValue; Debug.Log(this.ToString() + " Speed: " + value.GetSetValue); })));
        stats.Add(new Stat(this, StatType.WalkSpeed, defualtStats.WalkSpeed));
        stats.Add(new Stat(this, StatType.RangeMultiplier, 1));
    }
    protected virtual void OnTargetLoss() { }
    protected virtual void DetectedOutOfRange(Entity detectedEntity) { }
    public virtual void DetectedInRange(EntityHit detectedEntity) { }
    protected abstract class EntityState : State
    {
        protected EntityHit detectedEntity;
        protected Entity entity;
        protected EntityState(Entity entity)
        {
            this.entity = entity != null ? entity : throw new ArgumentNullException(nameof(entity));
        }
        public virtual void AnimationRecall() { }
    }
    protected class AttackState : EntityState
    {
        protected List<EntityHit> possibleTargets;
        protected EntityHit TargetEntity => possibleTargets.Count == 0 ? null : possibleTargets[0];
        protected void DetectedInRange(EntityHit detectedEntity) => entity.DetectedInRange(detectedEntity);

        Coroutine attackCoro;
        protected float attackDelay => 1 / (entity.projectile.attackSpeed * entity.stats.GetStatValue(StatType.AttackSpeedMultiplier));
        public AttackState(Entity entity) : base(entity) { }
        protected override void OnEnable()
        {
            possibleTargets = new List<EntityHit>();
            if (detectedEntity == null)
                detectedEntity = Targets.GetEntityHit(entity, entity.projectile.detection);
        }
        protected override void OnUpdate()
        {
            detectedEntity = Targets.GetEntityHit(entity, entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
            {
                if (Time.time >= entity.lastAttackTime + attackDelay)
                {
                    if (TargetEntity == null || TargetEntity.entity == null || !entity.projectile.locking)
                        possibleTargets = Targets.GetEntityHits(entity, entity.projectile.targeting);
                    if (TargetEntity != null && TargetEntity.entity != null)
                        Attack();
                    else
                        entity.DetectedOutOfRange(detectedEntity);
                }
            }
            else
                entity.OnTargetLoss();
        }
        protected virtual void Attack()
        {
            TargetEntity.entity.OnDestroyEvent += CancelAttack;
            DetectedInRange(detectedEntity);
            entity.lastAttackTime = Time.time;
            entity.animator.SetTrigger("AttackTrigger");
            //entity.transform.DOLocalRotate(entity.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);

        }
        public override void AnimationRecall()
        {
            if (TargetEntity != null && TargetEntity.entity != null)
            {
                TargetEntity.entity.OnDestroyEvent -= CancelAttack;
                
                Vector3 firePosition = entity.FireOrigin == null? entity.transform.position : entity.FireOrigin.position; // HERE !!!!!!
                GameObject projectile = Instantiate(entity.projectile.gameobject, firePosition, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                
                foreach(var effect in projectileScript.effects)
                {
                    if (effect.affectedStat == StatType.HP)
                        effect.amount *= (entity.stats.GetStatValue(StatType.DamageMultiplier)) / 100;
                }
                projectileScript.Init(entity, TargetEntity.entity, callback: entity.projectile.callback,firePosition);
            }
        }
        protected override void OnDisable() => Stop();
        void Stop()
        {
            if (attackCoro != null)
            {
                entity.StopCoroutine(attackCoro);
                attackCoro = null;
            }
        }
        void CancelAttack()
        {
            if (entity == null)
                return;
            entity.animator.SetTrigger("Reset");
            entity.lastAttackTime = 0;
        }

    }

}
[Serializable]
public class ProjectileData
{
    public GameObject gameobject;
    public TargetingSO detection;
    public TargetingSO targeting;
    public float attackSpeed;
    public Action<Entity[], Entity> callback;
    public bool locking;
}
[Serializable]
public class AuraData
{
    public GameObject gameobject;
    public Action<Entity[], Entity> callback;
    [Tooltip("Aura cast rate in castings per second.")]
    public float rate;
}
