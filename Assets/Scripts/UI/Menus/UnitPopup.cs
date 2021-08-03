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
        MinionBookUnitButton.OnSelect += MinionSelected;
        MinionBookUnitButton.OnDeselectEvent += MinionDeselected;

        if(MinionBookUnitButton.SelectedUnit != null)
            MinionSelected(MinionBookUnitButton.SelectedUnit);
    }
    private void OnDestroy()
    {
        MinionBookUnitButton.OnSelect -= MinionSelected;
        MinionBookUnitButton.OnDeselectEvent -= MinionDeselected;
    }

    void MinionSelected(MinionBookUnitButton unitButton)
    {
        OnMinionSelected?.Invoke(unitButton.Unit);
    }
    void MinionDeselected(MinionBookUnitButton unitButton)
    {
        OnMinionDeselected?.Invoke(unitButton.Unit);
    }
}
