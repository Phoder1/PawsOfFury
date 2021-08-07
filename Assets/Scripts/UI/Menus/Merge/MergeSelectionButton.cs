using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MergeSelectionButton : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnClick;
    [FoldoutGroup("Events")]
    public UnityEvent<UnitInformation> OnSelect;
    [FoldoutGroup("Events")]
    public UnityEvent OnDeselect;

    private UnitSelectionToken _selectionToken;
    private UnitInformation _selectedUnit;
    private bool _locked = false;
    public UnitInformation SelectedUnit { get => _selectedUnit; private set => _selectedUnit = value; }
    public void Click()
    {
        if (_locked)
            return;

        OnClick?.Invoke();
        UnitSelection.Subscribe(ref _selectionToken, true);
        _selectionToken.OnSelect += MinionSelected;
    }
    private void OnDisable()
    {
        Deselect();
    }

    private void MinionSelected(UnitInformation obj)
    {
        SelectedUnit = obj;
        _selectionToken?.Dispose();

        OnSelect?.Invoke(obj);
    }
    private void Deselect()
    {
        _selectionToken?.Dispose();

        SelectedUnit = null;

        OnDeselect?.Invoke();
    }
    public void StartedMerge()
    {
        _locked = true;
    }

    public void FinishedMerging()
    {
        Deselect();
        _locked = false;
    }
}
