using Assets.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BlackBoard;
[Flags] public enum TargetTypes { Enemy = 1, Self = 2, Unit = 4 }
public static class Targets
{
    public static LayerMask GetLayerMask(TargetTypes targetTypes)
    {
        LayerMask layerMask = 0;
        if (targetTypes.HasFlag(TargetTypes.Enemy))
        {
            if (layerMask == 0)
                layerMask = 11;
            else
                layerMask |= 11;
        }
        if (targetTypes.HasFlag(TargetTypes.Self) || targetTypes.HasFlag(TargetTypes.Unit))
        {
            if (layerMask == 0)
                layerMask = 8;
            else
                layerMask |= 8;
        }
        return layerMask;
    }
    public delegate bool EntityHitCondition(Entity entity);
    public static EntityHit GetEntityHit(Entity entity, TargetingSO targeting, EntityHitCondition entityHitCondition = null)
        => GetEntityHit(entity.gameObject.transform.position, targeting, entityHitCondition, entity);
    public static EntityHit GetEntityHit(Vector3 originPosition, TargetingSO targeting, EntityHitCondition entityHitCondition = null, Entity attackingEntity = null)
    {
        List<EntityHit> possibleHits = GetEntityHits(originPosition, targeting, entityHitCondition, attackingEntity);
        return possibleHits.Count == 0 ? null : possibleHits[0]; 
    }
    public static List<EntityHit> GetEntityHits(Entity entity, TargetingSO targeting, EntityHitCondition entityHitCondition = null)
        => GetEntityHits(entity.gameObject.transform.position, targeting, entityHitCondition, entity);
    public static List<EntityHit> GetEntityHits(Vector3 originPosition, TargetingSO targeting, EntityHitCondition entityHitCondition = null, Entity attackingEntity = null)
    {
        foreach (TargetingRule rule in targeting.RulesOrder)
        {
            if (rule.targets.HasFlag(TargetTypes.Enemy))
            {
                List<EntityHit> enemies = new List<Entity>(levelManager.Enemies).ConvertAll(x => new EntityHit(x, Vector3.Distance(x.transform.position, originPosition)));
                if (attackingEntity != null && attackingEntity is Enemy enemy && rule.targets.HasFlag(TargetTypes.Self))
                    enemies.Add(new EntityHit(enemy, 0));
                return RuleOutEntities(enemies, rule);
            }
            else if (rule.targets.HasFlag(TargetTypes.Unit))
            {
                List<EntityHit> units = new List<Entity>(levelManager.Units).ConvertAll(x => new EntityHit(x, Vector3.Distance(x.transform.position, originPosition)));
                if (attackingEntity != null && attackingEntity is Unit unit && rule.targets.HasFlag(TargetTypes.Self))
                    units.Add(new EntityHit(unit, 0));
                return RuleOutEntities(units, rule);
            }
            else if (attackingEntity != null && rule.targets.HasFlag(TargetTypes.Self))
                return new List<EntityHit>() { new EntityHit(attackingEntity, 0) };
        }
        return null;
        List<EntityHit> RuleOutEntities(List<EntityHit> entities, TargetingRule rule)
        {
            if (entities.Count == 0)
                return entities;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] == null
                    || entities[i].entity == null
                    || entities[i].distance > rule.range
                    || (!rule.canDetectGhosts && entities[i].entity.Type.HasFlag(EntityType.Ghost))
                    || (entityHitCondition != null && !entityHitCondition(entities[i]))
                    || Physics.Raycast(originPosition, entities[i].entity.transform.position - originPosition, entities[i].distance, LayerMask.GetMask("Wall")))
                {
                    entities.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            if (entities.Count == 0 || entities.Count == 1)
                return entities;
            foreach (EntityType entityType in Enum.GetValues(typeof(EntityType)))
                if (rule.tagPriority.HasFlag(entityType))
                {
                    entities = PickEntity((a, b) =>
                        Compare(a, b,
                        a.entity.Type.HasFlag(entityType) && !b.entity.Type.HasFlag(entityType),
                        !a.entity.Type.HasFlag(entityType) && b.entity.Type.HasFlag(entityType)));
                }
            switch (rule.priority)
            {
                //Todo: Add a way to calculate DPS
                //case Priority.DPS: 
                //    return PickEntity((a, b) =>
                //    {
                //        float aValue = a.entity.stats.GetStat(StatType.Damage).GetSetValue;
                //        float bValue = b.entity.stats.GetStat(StatType.Damage).GetSetValue;
                //        return Compare(a, b,
                //        aValue > bValue,
                //        aValue < bValue);
                //    });
                case Priority.SmallestMaxHP:
                    entities = PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        return Compare(a, b,
                        aValue < bValue,
                        aValue > bValue);
                    });
                    break;
                case Priority.LargestMaxHP:
                    entities = PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.MaxHP).GetSetValue;
                        return Compare(a, b,
                        aValue > bValue,
                        aValue < bValue);
                    });
                    break;
                case Priority.LowestCurrentHP:
                    entities = PickEntity((a, b) =>
                    {
                        float aValue = a.entity.stats.GetStat(StatType.HP).GetSetValue;
                        float bValue = b.entity.stats.GetStat(StatType.HP).GetSetValue;
                        return Compare(a, b,
                        aValue < bValue,
                        aValue > bValue);
                    });
                    break;
                case Priority.ShortestDistance:
                    entities = PickEntity((a, b) =>
                        Compare(a, b,
                        a.distance < b.distance,
                        a.distance > b.distance));
                    break;
                case Priority.Random:
                    Shuffle.ListShuffle(ref entities);
                    break;
            }
            return entities;
            List<EntityHit> PickEntity(Comparison<EntityHit> compare)
            {
                entities.Sort(compare);
                //for (int i = 0; i < entities.Count - 1; i++)
                //{
                //    int compareValue = compare(entities[i], entities[i + 1]);
                //    if (compareValue == 1)
                //    {
                //        entities.RemoveAt(i);
                //        i--;
                //    }
                //    else if (compareValue == -1)
                //    {
                //        entities.RemoveAt(i + 1);
                //        i--;
                //    }
                //}
                return entities;
            }
            int Compare(Entity a, Entity b, bool aFirstCondition, bool bFirstCondition)
            {
                if (!GlobalEffects.disableTaunts && !rule.ignoresTaunt)
                {
                    if (a.Type.HasFlag(EntityType.Tank) && !b.Type.HasFlag(EntityType.Tank))
                        return -1;
                    if (!a.Type.HasFlag(EntityType.Tank) && b.Type.HasFlag(EntityType.Tank))
                        return 1;
                }
                if (!rule.ignoresSelected)
                {
                    if (a.selected && !b.selected)
                        return -1;
                    if (!a.selected && b.selected)
                        return 1;
                }
                if (aFirstCondition)
                    return -1;
                if (bFirstCondition)
                    return 1;
                return 0;
            }
        }
    }
}
public enum Priority { ShortestDistance, SmallestMaxHP, LargestMaxHP, LowestCurrentHP, Random }
[CreateAssetMenu(fileName = "new Targeting Settings", menuName = "ScriptableObjects/" + "Targeting")]
public class TargetingSO : ScriptableObject
{
    [SerializeField] TargetingRule[] rulesOrder;
    public TargetingRule[] RulesOrder => rulesOrder;
}
[Serializable]
public class TargetingRule
{
    public TargetTypes targets;
    public Priority priority;
    public EntityType tagPriority;
    public float range;
    public bool ignoresTaunt;
    public bool canDetectGhosts;
    public bool ignoresSelected;
}
