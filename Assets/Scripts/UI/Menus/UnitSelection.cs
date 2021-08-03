using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitSelection
{
    private static UnitInformation _selectedUnit;
    public static UnitInformation SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (_selectedUnit == value)
                return;

            if (_selectedUnit != null)
                OnDeselectEvent?.Invoke(_selectedUnit);

            _selectedUnit = value;

            if (_selectedUnit != null)
                OnSelect?.Invoke(_selectedUnit);
        }
    }
    public static event Action<UnitInformation> OnDeselectEvent;
    public static event Action<UnitInformation> OnSelect;
}
