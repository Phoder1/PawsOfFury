using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitPopup : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<UnitInformation> OnMinionSelected;
    [SerializeField]
    private UnityEvent<UnitInformation> OnMinionDeselected;
    private void Awake()
    {
        UnitSelection.OnSelect += MinionSelected;
        UnitSelection.OnDeselect += MinionDeselected;

        if(UnitSelection.SelectedUnit != null)
            MinionSelected(UnitSelection.SelectedUnit);
    }
    private void OnDestroy()
    {
        UnitSelection.OnSelect -= MinionSelected;
        UnitSelection.OnDeselect -= MinionDeselected;
    }

    void MinionSelected(UnitInformation unit)
    {
        OnMinionSelected?.Invoke(unit);
    }
    void MinionDeselected(UnitInformation unit)
    {
        OnMinionDeselected?.Invoke(unit);
    }
}
