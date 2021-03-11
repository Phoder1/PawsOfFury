using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityStats;

public class Entity : MonoBehaviour
{
    [SerializeField] protected GameObject UiObject;
    [SerializeField] protected DefualtStats defualtStats;

    public EntityStats stats;
}
