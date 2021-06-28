using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiUnitController : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<UnitSO> OnUnitAssign;

    private UnitSO _unit;
    public UnitSO Unit 
    { 
        get => _unit; 
        set
        {
            if (value == null || value == _unit)
                return;

            _unit = value;

            OnUnitAssign?.Invoke(_unit);
        }
    }
    public void SetUnit(UnitSO unit, bool withCallBacks = true)
    {
        if (withCallBacks)
            Unit = unit;
        else
            _unit = unit;
    }
}
