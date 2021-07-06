using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDatabaseSO<T> : ScriptableObject
{
    [SerializeField]
    protected List<T> content;
    public List<T> Content => content;
}
