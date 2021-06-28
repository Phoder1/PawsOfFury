using Assets.StateMachine;
using Assets.Stats;
using CustomAttributes;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    [LocalComponent]
    protected AnimationHandler animationHandler;
    protected Camera MainCam => GameManager.MainCam;
    protected EntityUI ui;
    protected StateMachine<EntityState> stateMachine;
    [HideInInspector]
    public float lastAttackTime;

    public EntityType Type => type;
    [SerializeField] bool ISInitOnStart = false;

    protected abstract EntityState DefaultState();
    public UnityEvent OnDestroyEvent;

    public UnityEvent OnAttackAnimation;
    public void AttackAnimRecall()
    {
        OnAttackAnimation?.Invoke();
        stateMachine.State.AnimationRecall();
    }


    [HideInInspector]
    public bool selected;
    private bool gameIsShuttingDown;

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

    void Awake()
    {
        if (animationHandler == null)
            animationHandler = GetComponent<AnimationHandler>();
    }
    protected virtual void Start()
    {
        lastAttackTime = -Mathf.Infinity;
        stats = new EntityStats();
        UiObject = Instantiate(UiObject, MainCam.WorldToScreenPoint(transform.position), transform.rotation, levelManager.EntityUIsObj);
        levelManager.AddToList(this);
        ui = UiObject.GetComponent<EntityUI>();
        ui.HealthBarHeight = healthbarHeight;
        FillDictionary();
        stateMachine = new StateMachine<EntityState>(null);
        StartCoroutine(CastAura());
        if (ISInitOnStart)
        {
            Init();
        }

    }
    public void Init() => stateMachine.State = DefaultState();
    public void OnPointerUp(PointerEventData eventData) => Selected = !Selected;
    public void OnPointerDown(PointerEventData eventData) { }
    protected virtual void Update()
    {
        stateMachine.Update();
        UiObject.transform.position = MainCam.WorldToScreenPoint(transform.position);

    }
    void OnApplicationQuit()
    {
        this.gameIsShuttingDown = true;
    }

    protected virtual void OnDestroy()
    {
        if (!gameIsShuttingDown) 
        { 
        transform.DOComplete();
        levelManager.RemoveFromList(this);
        OnDestroyEvent?.Invoke();
        stateMachine.State = null;
        }

    }

    private IEnumerator CastAura()
    {
        if (aura != null && aura.gameobject != null)
        {
            float castDelay = 1 / aura.rate;
            yield return new WaitForSeconds(castDelay);
            GameObject auraObj = Instantiate(aura.gameobject, transform.position, Quaternion.identity);
            auraObj.GetComponent<Projectile>().Init(this, this, aura.callback);
            StartCoroutine(CastAura());
        }
    }
    protected virtual void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, ui, maxHp, new Reaction(Reaction.DeathCondition, (value) => { Destroy(gameObject); })));
        Stat maxDamage = new Stat(this, StatType.MaxDamageMultiplier, defualtStats.MaxDamageMultiplier);
        stats.Add(maxDamage);
        stats.Add(new Stat(this, StatType.DamageMultiplier, 100, maxDamage));
        Stat maxAttackSpeed = new Stat(this, StatType.MaxAttackSpeedMultiplier, defualtStats.MaxAttackSpeedMultiplier);
        stats.Add(maxAttackSpeed);
        stats.Add(new Stat(this, StatType.AttackSpeedMultiplier, 1, maxAttackSpeed, new Reaction(Reaction.AlwaysTrue, (value) => { if (animationHandler) animationHandler.SetSpeed(value.GetSetValue); Debug.Log(this.ToString() + " Speed: " + value.GetSetValue); })));
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
        protected float AttackDelay => 1 / (entity.projectile.attackSpeed * entity.stats.GetStatValue(StatType.AttackSpeedMultiplier));
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
                if (Time.time >= entity.lastAttackTime + AttackDelay)
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
            if (entity is Unit)
            {
                if (TargetEntity.entity.transform.position.x > entity.transform.position.x)
                    SetDirection(false);
                else if (TargetEntity.entity.transform.position.x == entity.transform.position.x)
                    SetDirection(UnityEngine.Random.Range(0, 2) == 1);
                else
                    SetDirection(true);
            }

            TargetEntity.entity.OnDestroyEvent.AddListener(CancelAttack);
            DetectedInRange(detectedEntity);
            entity.lastAttackTime = Time.time;
            entity.animationHandler.SetTrigger("AttackTrigger");
            //entity.transform.DOLocalRotate(entity.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);

            void SetDirection(bool left) => entity.transform.localScale = new Vector3(left ? 1 : -1, entity.transform.localScale.y, entity.transform.localScale.z);
        }
        public override void AnimationRecall()
        {
            if (TargetEntity != null && TargetEntity.entity != null)
            {
                TargetEntity.entity.OnDestroyEvent.RemoveListener(CancelAttack);

                Vector3 firePosition = entity.FireOrigin == null ? entity.transform.position : entity.FireOrigin.position; // HERE !!!!!!
                GameObject projectile = Instantiate(entity.projectile.gameobject, firePosition, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();

                if (entity.stats.GetStatValue(StatType.DamageMultiplier) != 100)
                {
                    foreach (var effect in projectileScript.effects)
                    {
                        if (effect.affectedStat == StatType.HP)
                            effect.amount *= (entity.stats.GetStatValue(StatType.DamageMultiplier)) / 100;
                    }
                }
                projectileScript.Init(entity, TargetEntity.entity, callback: entity.projectile.callback, firePosition);
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
            entity.animationHandler.SetTrigger("Reset");
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
