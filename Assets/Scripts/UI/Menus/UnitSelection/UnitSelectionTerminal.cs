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

    private UnitSelectionToken selectionToken;
    private void OnEnable() => EnableToken();


    private void OnDisable() => Disabletoken();

    private void Disabletoken()
    {
        selectionToken?.Dispose();
    }
    private void EnableToken()
    {
        UnitSelection.Subscribe(ref selectionToken);

        selectionToken.OnSelect += (x) => OnSelect?.Invoke(x);
        selectionToken.OnDeselect += (x) => OnDeselect?.Invoke(x);
        selectionToken.OnDisenchant += (x) => OnDisenchant?.Invoke(x);
    }
    public void ResetToken()
    {
        Disabletoken();
        EnableToken();
    }
    public void Select(UnitInformation unit) => UnitSelection.LastSelectedUnit = unit;
    public void DisenchantSelected() => UnitSelection.DisenchantSelected();
}
