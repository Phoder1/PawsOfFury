using DataSaving;
using System;
using System.Collections.Generic;

public static class UnitSelection
{
    private static List<UnitSelectionToken> _selectionTokens = new List<UnitSelectionToken>();

    private static UnitInformation _lastSelectedUnit;
    public static UnitInformation LastSelectedUnit
    {
        get => _lastSelectedUnit;
        set
        {
            if (_lastSelectedUnit == value)
                return;

            _lastSelectedUnit = value;

            for (int i = 0; i < _selectionTokens.Count; i++)
                _selectionTokens[i].SelectedUnit = _lastSelectedUnit;
        }
    }
    public static event Action<UnitSelectionToken> OnNewToken;
    public static void DisenchantSelected()
    {
        if (LastSelectedUnit == null || LastSelectedUnit.unitSO == null || !LastSelectedUnit.Disenchantable)
            return;

        LastSelectedUnit.slotData.Count--;
        DataHandler.Load<PlayerCurrency>().MonsterGoo += LastSelectedUnit.GooValue;
        Disenchanted(LastSelectedUnit);
        DataHandler.Save<PlayerCurrency>();
    }
    public static void Subscribe(ref UnitSelectionToken token, bool exclusive = false)
    {
        token?.Dispose();
        token = CreateNewToken(exclusive);
    }
    public static void Unsubscribe(this UnitSelectionToken token)
    {
        _selectionTokens.Remove(token);
    }
    private static UnitSelectionToken CreateNewToken(bool exclusive = false)
    {
        var token = new UnitSelectionToken(exclusive);
        _selectionTokens.Add(token);
        OnNewToken?.Invoke(token);
        token.Init();
        return token;
    }
    private static void Disenchanted(UnitInformation unit) => _selectionTokens.ForEach((x) => x.Disenchanted(unit));
}
public class UnitSelectionToken : IDisposable
{
    private readonly bool _exclusive;
    private UnitInformation _selectedUnit = null;

    public UnitSelectionToken(bool exclusive = false)
    {
        _exclusive = exclusive;
    }

    public UnitInformation SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (_selectedUnit == value)
                return;

            if (_selectedUnit != null)
                Deselected(_selectedUnit);

            _selectedUnit = value;

            if (_selectedUnit != null)
                Selected(_selectedUnit);
        }
    }
    public event Action<UnitInformation> OnDeselect;
    public event Action<UnitInformation> OnSelect;
    public event Action<UnitInformation> OnDisenchant;
    public event Action OnTokenDisposed;

    public void Init()
    {
        if (_exclusive)
            UnitSelection.OnNewToken += (x) => Dispose();
    }
    public void Dispose()
    {
        OnTokenDisposed?.Invoke();
        this.Unsubscribe();

        SelectedUnit = null;
        OnDeselect = null;
        OnSelect = null;
        OnDisenchant = null;
        OnTokenDisposed = null;
    }
    public void Deselected(UnitInformation unitInformation) => OnDeselect?.Invoke(unitInformation);
    public void Selected(UnitInformation unitInformation) => OnSelect?.Invoke(unitInformation);
    public void Disenchanted(UnitInformation unitInformation) => OnDisenchant?.Invoke(unitInformation);
}
