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
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnLock;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUnlock;

    private UnitSelectionToken _selectionToken;
    private UnitInformation _selectedUnit;
    private bool _locked = false;
    public bool Locked
    {
        get => _locked;
        set
        {
            if (_locked == value)
                return;

            _locked = value;

            if (_locked)
                OnLock?.Invoke();
            else
                OnUnlock?.Invoke();
        }
    }
    public UnitInformation SelectedUnit { get => _selectedUnit; private set => _selectedUnit = value; }

    public void Click()
    {
        if (Locked)
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
    public void Deselect()
    {
        _selectionToken?.Dispose();

        SelectedUnit = null;

        OnDeselect?.Invoke();
    }
    public void StartedMerge()
    {
        Locked = true;
    }
    public void FinishedMerging()
    {
        Deselect();
        Locked = false;
    }
}
