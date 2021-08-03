using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelSelectionTerminal : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<LevelSO> OnSelect;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<LevelSO> OnDeselect;

    private void OnEnable()
    {
        LevelSelection.OnSelect += (x) => OnSelect?.Invoke(x);
        LevelSelection.OnDeselect += (x) => OnDeselect?.Invoke(x);

        if (LevelSelection.SelectedLevel != null)
            OnSelect?.Invoke(LevelSelection.SelectedLevel);
    }
    private void OnDisable()
    {
        LevelSelection.OnSelect -= (x) => OnSelect?.Invoke(x);
        LevelSelection.OnDeselect -= (x) => OnDeselect?.Invoke(x);
    }
    public void Select(LevelSO unit) => LevelSelection.SelectedLevel = unit;
}
