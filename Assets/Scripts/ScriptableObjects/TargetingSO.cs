using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Flags] public enum Target {None = 0, Tower = 1, Self = 2, Unit = 4 }
public enum Priority { DPS, SmallestMaxHP, LargestMaxHP, LowestCurrentHP, ShortestDistance, Healer }
[CreateAssetMenu(fileName = "new Targeting Settings",menuName = "ScriptableObjects/" + "Targeting")]
public class TargetingSO : ScriptableObject
{
    [SerializeField] Target[] targets;
    [SerializeField] Priority[] priorities;
    [SerializeField] bool locking;
}
