using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Flags] public enum TargetTypes { Enemy = 1, Self = 2, Unit = 4 }
public static class Targets
{
    public static LayerMask GetLayerMask(TargetTypes targetTypes) {
        LayerMask layerMask = 0;
        if(targetTypes.HasFlag(TargetTypes.Enemy))
        {
            if (layerMask == 0)
                layerMask = 11;
            else
                layerMask |= 11;
        }
        if(targetTypes.HasFlag(TargetTypes.Self) || targetTypes.HasFlag(TargetTypes.Unit))
        {
            if (layerMask == 0)
                layerMask = 8;
            else
                layerMask |= 8;
        }
        return layerMask;
    }
}
public enum Priority { DPS, SmallestMaxHP, LargestMaxHP, LowestCurrentHP, ShortestDistance, Healer }
[CreateAssetMenu(fileName = "new Targeting Settings",menuName = "ScriptableObjects/" + "Targeting")]
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
}
