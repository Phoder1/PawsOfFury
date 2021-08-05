using DataSaving;
using System;

public static class UnitSelection
{
    private static UnitInformation _selectedUnit;
    public static UnitInformation SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (_selectedUnit == value)
                return;

            if (_selectedUnit != null)
                OnDeselect?.Invoke(_selectedUnit);

            _selectedUnit = value;

            if (_selectedUnit != null)
                OnSelect?.Invoke(_selectedUnit);
        }
    }
    public static event Action<UnitInformation> OnDeselect;
    public static event Action<UnitInformation> OnSelect;
    public static event Action<UnitInformation> OnDisenchant;
    public static void DisenchantSelected()
    {
        if (SelectedUnit == null || SelectedUnit.unitSO == null || !SelectedUnit.Disenchantable)
            return;

        SelectedUnit.slotData.Count--;
        DataHandler.Load<PlayerCurrency>().MonsterGoo += SelectedUnit.GooValue;
        OnDisenchant?.Invoke(SelectedUnit);
        DataHandler.Save<PlayerCurrency>();
    }
}
