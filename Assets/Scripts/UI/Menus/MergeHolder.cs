using CustomAttributes;
using DataSaving;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MergeHolder : MonoBehaviour
{
    [SerializeField]
    private List<MergeSelectionButton> _selectionButtons;

    [SerializeField, LocalComponent]
    private Animator _animator;

    [SerializeField]
    private string _animationTriggerName;

    [SerializeField, FoldoutGroup("Messages")]
    private string DiffrentTiersMessage;
    [SerializeField, FoldoutGroup("Messages")]
    private string NotAllSelectedMessage;
    [SerializeField, FoldoutGroup("Messages")]
    private string NotAllMergableMessage;
    [SerializeField, FoldoutGroup("Messages")]
    private string NotEnoughCopiesMessage;
    [SerializeField, FoldoutGroup("Messages")]
    private string EliteUnitMessage;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnValidUnitsSelected;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnMerge;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<string> OnInvalidSelection = default;

    private InventoryData _inventory;
    public InventoryData Inventory => DataHandler.Getter(ref _inventory);

    public bool AllSelected => _selectionButtons.TrueForAll((x) => x.SelectedUnit != null);

    private void OnEnable()
    {
        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            _selectionButtons[i].OnSelect.AddListener(Selected);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            _selectionButtons[i].OnSelect.RemoveListener(Selected);
        }
    }

    public void TryMerge()
    {
        if (!isActiveAndEnabled)
            return;

        if (CheckIfValid())
            StartMerge();
    }
    private void Selected(UnitInformation unit)
    {
        if (!isActiveAndEnabled)
            return;

        if (CheckIfValid(out string message))
            OnValidUnitsSelected?.Invoke();
        else
            OnInvalidSelection?.Invoke(message);
    }

    private bool CheckIfValid() => CheckIfValid(out _);
    private bool CheckIfValid(out string message)
    {
        message = string.Empty;
        if (!AllSelected)
        {
            message = NotAllSelectedMessage;
            return false;
        }

        if (!IsSameTier())
        {
            message = DiffrentTiersMessage;
            return false;
        }

        if (!AllMergeable())
        {
            message = NotAllMergableMessage;
            return false;
        }

        if (!EnoughCopies())
        {
            message = NotEnoughCopiesMessage;
            return false;
        }

        if (!AllNoneEliteUnits())
        {
            message = EliteUnitMessage;
            return false;
        }

        return true;

        bool IsSameTier()
        {
            int tier = _selectionButtons[0].SelectedUnit.unitSO.Tier;
            return _selectionButtons.TrueForAll((x) => x.SelectedUnit.unitSO.Tier == tier);
        }
        bool AllMergeable() => _selectionButtons.TrueForAll((x) => x.SelectedUnit.Mergeable);
        bool EnoughCopies()
        {
            List<UnitInformation> unitsChecked = new List<UnitInformation>();
            for (int i = 0; i < _selectionButtons.Count; i++)
            {
                var unit = _selectionButtons[i].SelectedUnit;
                if (unitsChecked.Contains(unit))
                    continue;

                unitsChecked.Add(unit);
                int count = 1;

                for (int j = i + 1; j < _selectionButtons.Count; j++)
                    if (_selectionButtons[j].SelectedUnit == unit)
                        count++;

                if (unit.Amount <= count)
                    return false;
            }
            return true;
        }

        bool AllNoneEliteUnits() => _selectionButtons.TrueForAll((x) => x.SelectedUnit.unitSO.Tier < UnitSO.MaxTier);
    }

    private void StartMerge()
    {
        if (_animator != null && !string.IsNullOrWhiteSpace(_animationTriggerName))
            _animator.SetTrigger(_animationTriggerName);

        foreach (var button in _selectionButtons)
        {
            button.StartedMerge();
            Inventory.Units.Find((x) => x == button.SelectedUnit.slotData).Count--;
        }

        var unitReward = UnitRandomizer.GetMergeReward(_selectionButtons.ConvertAll((x) => x.SelectedUnit).ToArray());
        Inventory.AddUnits(unitReward, 1);

        OnMerge?.Invoke();
    }
}
