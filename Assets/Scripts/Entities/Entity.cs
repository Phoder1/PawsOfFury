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
    [SerializeField] protected DefualtStats defualtStats;

    public ProjectileData projectile;
    [SerializeField] protected AuraData aura;
    [Tooltip("Height 0 is the center of the Unit.")]
    [SerializeField] protected float healthbarHeight;


    public EntityStats stats;
    protected Camera mainCam;
    protected HealthBar healthBar;
    protected StateMachine stateMachine;
    protected float lastAttackTime;

    protected LevelManager levelManager;

    public EntityType Type => type;


    protected abstract EntityState DefaultState();
    public event Action OnDestroyEvent;
    bool selected;
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
        stateMachine = new StateMachine(DefaultState());
    }
    protected virtual void Update()
    {
        stateMachine.Update();
        UiObject.transform.position = mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight;
        CastAura();
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
    }
    protected abstract void FillDictionary();
    protected virtual void OnTargetLoss() { }
    protected virtual void DetectedOutOfRange() { }
    public EntityHit GetEntityHit(float range, TargetingSO targeting)
    {
        EntityHit hit = null;

        foreach (TargetingRule rule in targeting.RulesOrder)
        {

            if (rule.targets.HasFlag(TargetTypes.Self))
            {
                hit = new EntityHit(this, 0);
                break;
            }
            else if (rule.targets.HasFlag(TargetTypes.Enemy))
            {
                List<EntityHit> enemies = new List<Entity>(levelManager.Enemies).ConvertAll(x => new EntityHit(x, Vector3.Distance(x.transform.position, transform.position)));
                return RuleOutEntities(enemies, rule.priority);
            }
            else if (rule.targets.HasFlag(TargetTypes.Unit))
            {
                List<EntityHit> units = new List<Entity>(levelManager.Units).ConvertAll(x => new EntityHit(x, Vector3.Distance(x.transform.position, transform.position)));
                return RuleOutEntities(units, rule.priority);
            }
        }
        return hit;

        EntityHit RuleOutEntities(List<EntityHit> entities, Priority priority)
        {
            if (entities.Count == 0)
                return null;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] == null || entities[i].entity == null || entities[i].distance > range || entities[i].entity.Type.HasFlag(EntityType.Ghost))
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
            if (entities.Count == 0)
                return null;
            if (entities.Count == 1)
                return entities[0];
            switch (priority)
            {
                case Priority.DPS:
                    return PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.Damage).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.Damage).GetSetValue;
                        return Compare(a, b,
                        aValue > bValue,
                        aValue < bValue);
                    });
                case Priority.SmallestMaxHP:
                    return PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        return Compare(a, b,
                        aValue < bValue,
                        aValue > bValue);
                    });
                case Priority.LargestMaxHP:
                    return PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        return Compare(a, b,
                        aValue > bValue,
                        aValue < bValue);
                    });
                case Priority.LowestCurrentHP:
                    return PickEntity((a, b) =>
                        {
                            float aValue = a.entity.stats.GetStat(StatType.HP).GetSetValue;
                            float bValue = b.entity.stats.GetStat(StatType.HP).GetSetValue;
                            return Compare(a, b,
                            aValue < bValue,
                            aValue > bValue);
                        });
                case Priority.ShortestDistance:
                    return PickEntity((a, b) =>
                        Compare(a, b,
                        a.distance < b.distance,
                        a.distance > b.distance));
                case Priority.Healer:
                    return PickEntity((a, b) =>
                        Compare(a, b,
                        a.entity.Type.HasFlag(EntityType.Healer) && !b.entity.Type.HasFlag(EntityType.Healer),
                        !a.entity.Type.HasFlag(EntityType.Healer) && b.entity.Type.HasFlag(EntityType.Healer)));
            }
            return null;
            
            EntityHit PickEntity(Comparison<EntityHit> compare)
            {
                while(entities.Count > 1)
                {
                    int compareValue = compare(entities[0], entities[1]);
                    if (compareValue == 1)
                    {
                        entities.RemoveAt(0);
                    }
                    else if (compareValue == -1)
                    {
                        entities.RemoveAt(1);
                    }
                }
                return entities[0];
            }
            int Compare(Entity a, Entity b, bool aFirstCondition, bool bFirstCondition)
            {
                if (a.Type.HasFlag(EntityType.Tank) && !b.Type.HasFlag(EntityType.Tank))
                    return -1;
                if (!a.Type.HasFlag(EntityType.Tank) && b.Type.HasFlag(EntityType.Tank))
                    return 1;
                if (a.selected && !b.selected)
                    return -1;
                if (!a.selected && b.selected)
                    return 1;
                if (aFirstCondition)
                    return -1;
                if (bFirstCondition)
                    return 1;
                return 0;
            }
        }
    }
    
    protected abstract class EntityState : State
    {
        protected EntityHit detectedEntity;
        protected EntityHit targetEntity;
        protected Entity entity;
        protected readonly LevelManager levelManager;

        protected EntityState(Entity entity)
        {
            this.entity = entity != null ? entity : throw new ArgumentNullException(nameof(entity));
            levelManager = LevelManager._instance;
        }
    }
    protected class AttackState : EntityState
    {
        Coroutine attackCoro;

        public AttackState(Entity entity) : base(entity)
        {
        }
        protected override void OnEnable()
        {
            attackCoro = entity.StartCoroutine(StartAttacking());
            if (detectedEntity == null)
                detectedEntity = entity.GetEntityHit(entity.stats.GetStatValue(StatType.Range), entity.projectile.detection);
        }
        protected override void OnUpdate()
        {
            if (detectedEntity == null || detectedEntity.entity == null)
            {
                detectedEntity = entity.GetEntityHit(entity.stats.GetStatValue(StatType.Range), entity.projectile.detection);
            }
            if (detectedEntity != null && detectedEntity.entity != null)
            {
                float attackDelay = 1 / entity.stats.GetStatValue(StatType.AttackSpeed);
                if (Time.time >= entity.lastAttackTime + attackDelay)
                {
                    if (targetEntity == null || targetEntity.entity == null || !entity.projectile.locking)
                        targetEntity = entity.GetEntityHit(entity.stats.GetStatValue(StatType.Range), entity.projectile.targeting);
                    if (targetEntity != null && targetEntity.entity != null)
                    {
                        entity.lastAttackTime = Time.time;
                        entity.transform.DOLocalRotate(entity.transform.rotation.eulerAngles + Vector3.up * 360, attackDelay / 2, RotateMode.FastBeyond360);
                        GameObject projectile = Instantiate(entity.projectile.gameobject, entity.transform.position, Quaternion.identity);
                        projectile.GetComponent<Projectile>().Init(entity, targetEntity.entity, callback: entity.projectile.callback);
                    }
                    else
                    {
                        entity.DetectedOutOfRange();
                    }
                }
            }
            else
                entity.OnTargetLoss();
        }
        protected override void OnDisable()
        {
            Stop();
        }
        IEnumerator StartAttacking()
        {

            do
            {
                yield return null;

            } while (attackCoro != null && detectedEntity.entity != null);
        }
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
    public GameObject projectile;
}
