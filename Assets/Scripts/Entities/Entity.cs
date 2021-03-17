using Assets.StateMachine;
using Assets.Stats;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EntityType
{
    Healer = 1,
    DPS = 2,
    Tank = 4,
    Buffer = 8,
    Ghost = 16
}
public abstract class Entity : MonoBehaviour
{
    public string entityName;
    [SerializeField] EntityType type;
    [SerializeField] protected GameObject UiObject;
    [SerializeField] protected float healthbarHeight;
    [SerializeField] protected DefualtStats defualtStats;

    public ProjectileData projectile;
    [SerializeField] protected AuraData aura;
    [Tooltip("Height 0 is the center of the Unit.")]


    public EntityStats stats;
    protected Camera mainCam;
    protected HealthBar healthBar;
    protected StateMachine<EntityState> stateMachine;
    [HideInInspector]
    public float lastAttackTime;

    protected LevelManager levelManager;

    public EntityType Type => type;


    protected abstract EntityState DefaultState();
    public event Action OnDestroyEvent;
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
                if (selected)
                {
                    InputManager._instance.ChangeSelection();
                    InputManager._instance.OnChangedSelection += DisableSelection;
                }
                else
                    InputManager._instance.OnChangedSelection -= DisableSelection;

            }
        }
    }
    void DisableSelection() => Selected = false;


    protected virtual void Start()
    {
        mainCam = Camera.main;
        levelManager = LevelManager._instance;
        lastAttackTime = -Mathf.Infinity;
        stats = new EntityStats();
        UiObject = Instantiate(UiObject, mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight, transform.rotation);
        levelManager.AddToList(this);
        healthBar = UiObject.GetComponent<HealthBar>();
        healthBar.Init();
        FillDictionary();
        stateMachine = new StateMachine<EntityState>(DefaultState());
        CastAura();
    }
    protected virtual void Update()
    {
        stateMachine.Update();
        UiObject.transform.position = mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight;
    }
    protected virtual void OnDestroy()
    {
        transform.DOComplete();
        levelManager.RemoveFromList(this);
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
            Debug.Log("Casted aura");
        }
    }
    protected virtual void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, healthBar, maxHp,
            new List<Reaction> {
            new Reaction( Reaction.DeathCondition, (value) => { Destroy(gameObject); } )
            }));
        stats.Add(new Stat(this, StatType.DamageMultiplier, 1));
        Stat maxAttackSpeed = new Stat(this, StatType.MaxAttackSpeedMultiplier, defualtStats.MaxAttackSpeedMultiplier);
        stats.Add(maxAttackSpeed);
        stats.Add(new Stat(this, StatType.AttackSpeedMultiplier, 1, maxAttackSpeed));
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
            DetectedInRange(detectedEntity);
            entity.lastAttackTime = Time.time;
            entity.transform.DOLocalRotate(entity.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);
            GameObject projectile = Instantiate(entity.projectile.gameobject, entity.transform.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.Init(entity, TargetEntity.entity, callback: entity.projectile.callback);
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
