using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitSelectionTerminal : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<UnitInformation> OnSelect;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<UnitInformation> OnDeselect;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<UnitInformation> OnDisenchant;

    private void OnEnable()
    {
        UnitSelection.OnSelect += (x) => OnSelect?.Invoke(x);
        UnitSelection.OnDeselect += (x) => OnDeselect?.Invoke(x);
        UnitSelection.OnDisenchant += (x) => OnDisenchant?.Invoke(x);

        if (UnitSelection.SelectedUnit != null)
            OnSelect?.Invoke(UnitSelection.SelectedUnit);
    }
    private void OnDisable()
    {
        UnitSelection.OnSelect -= (x) => OnSelect?.Invoke(x);
        UnitSelection.OnDeselect -= (x) => OnDeselect?.Invoke(x);
        UnitSelection.OnDisenchant -= (x) => OnDisenchant?.Invoke(x);
    }
    public void Select(UnitInformation unit) => UnitSelection.SelectedUnit = unit;
    public void DisenchantSelected() => UnitSelection.DisenchantSelected();
}
