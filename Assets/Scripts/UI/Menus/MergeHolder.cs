using CustomAttributes;
using DataSaving;
using Sirenix.OdinInspector;
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
    [SerializeField]
    private float _animationDuration;

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
    [SerializeField, FoldoutGroup("Messages")]
    private string NotEnoughCurrencyMessage;

    [SerializeField, FoldoutGroup("Merge info")]
    private UnityEvent<int> _gooCost;
    [SerializeField, FoldoutGroup("Merge info")]
    private UnityEvent<int> _crystalsCost;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnValidUnitsSelected;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnValidToMerge;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnMerge;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnInvalidSelection = default;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnInvalidToMerge = default;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<string> OnInvalidMessage = default;



    private bool _locked = false;
    private InventoryData _inventory;
    public InventoryData Inventory => DataHandler.Getter(ref _inventory);

    private PlayerCurrency _currency;
    public PlayerCurrency Currency => DataHandler.Getter(ref _currency);

    public bool AllSelected => _selectionButtons.TrueForAll((x) => x.SelectedUnit != null);

    private void OnEnable()
    {
        OnInvalidSelection?.Invoke();
        OnInvalidMessage?.Invoke(string.Empty);

        for (int i = 0; i < _selectionButtons.Count; i++)
            _selectionButtons[i].OnSelect.AddListener(Selected);

        Currency.OnValueChange += CheckState;
    }
    private void OnDisable()
    {
        OnValidUnitsSelected?.Invoke();

        for (int i = 0; i < _selectionButtons.Count; i++)
            _selectionButtons[i].OnSelect.RemoveListener(Selected);

        Currency.OnValueChange -= CheckState;
    }

    public void TryMerge()
    {
        if (!isActiveAndEnabled)
            return;

        if (CheckIfSelectionValid() && CheckIfMergeValid())
            Merge();
    }
    private void Selected(UnitInformation unit)
    {
        if (!isActiveAndEnabled)
            return;

        CheckState();
    }
    private void CheckState()
    {
        if (_locked)
            return;

        if (CheckIfSelectionValid(out var message))
        {
            var unit = _selectionButtons[0].SelectedUnit;

            _gooCost?.Invoke(unit.GooValue);
            _crystalsCost?.Invoke(unit.unitSO.MergeCrystalsCost);
            OnValidUnitsSelected?.Invoke();

            if (CheckIfMergeValid(out message))
            {
                OnValidToMerge?.Invoke();
            }
            else
            {
                OnInvalidToMerge?.Invoke();
                OnInvalidMessage?.Invoke(message);
            }
        }
        else
        {
            OnInvalidSelection?.Invoke();
            OnInvalidMessage?.Invoke(message);

        }
    }
    private bool CheckIfSelectionValid() => CheckIfSelectionValid(out _);
    private bool CheckIfSelectionValid(out string message)
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

        bool AllNoneEliteUnits() => _selectionButtons.TrueForAll((x) => x.SelectedUnit.unitSO.Tier < UnitSO.MaxTier);

    }

    private bool CheckIfMergeValid() => CheckIfMergeValid(out _);
    private bool CheckIfMergeValid(out string message)
    {
        message = string.Empty;

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

        if (!HasEnoughCurrency())
        {
            message = NotEnoughCurrencyMessage;
            return false;
        }

        return true;


        bool HasEnoughCurrency() => Currency.Crystals >= _selectionButtons[0].SelectedUnit.unitSO.MergeCrystalsCost && Currency.MonsterGoo >= _selectionButtons[0].SelectedUnit.GooValue;
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
    }

    private void Merge()
    {
        _locked = true;

        if (_animator != null && !string.IsNullOrWhiteSpace(_animationTriggerName))
            _animator.SetTrigger(_animationTriggerName);

        foreach (var button in _selectionButtons)
        {
            button.StartedMerge();
            Inventory.Units.Find((x) => x == button.SelectedUnit.slotData).Count--;

        }

        var unitReward = UnitRandomizer.GetMergeReward(_selectionButtons.ConvertAll((x) => x.SelectedUnit).ToArray());
        Inventory.AddUnits(unitReward, 1);

        Currency.Crystals -= _selectionButtons[0].SelectedUnit.unitSO.MergeCrystalsCost;
        Currency.MonsterGoo -= _selectionButtons[0].SelectedUnit.GooValue;

        OnMerge?.Invoke();

        //Temp! should be moved to after the player opens the reward
        Invoke(nameof(FinishedMerging), _animationDuration);
    }

    private void FinishedMerging()
    {
        _selectionButtons.ForEach((x) => x.FinishedMerging());
        _locked = false;
    }
}
