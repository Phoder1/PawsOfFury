using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelSelection
{
    private static LevelSO _selectedLevel;
    public static LevelSO SelectedLevel
    {
        get => _selectedLevel;
        set
        {
            if (_selectedLevel == value)
                return;

            if (_selectedLevel != null)
                OnDeselect?.Invoke(_selectedLevel);

            _selectedLevel = value;

            if (_selectedLevel != null)
                OnSelect?.Invoke(_selectedLevel);
        }
    }
    public static event Action<LevelSO> OnDeselect;
    public static event Action<LevelSO> OnSelect;
}
