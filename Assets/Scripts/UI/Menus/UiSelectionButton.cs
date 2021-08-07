using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiSelectionButton : MonoBehaviour
{
    private UnitInformation _unitInformation;

    public UnitInformation UnitInformation { get => _unitInformation; set => _unitInformation = value; }

    [SerializeField]
    private UnityEvent<UnitInformation> OnClick;

    public void Click() => Click(UnitInformation);
    public void Click(UnitInformation unitInformation)
    {
        OnClick?.Invoke(unitInformation);
    }
}
