using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class UiUnitController : MonoBehaviour
{
    [SerializeField]
    private bool _importFromTeamOnStart;
    [SerializeField, ShowIf("@_importFromTeamOnStart")]
    private int _teamNumber;
    [SerializeField]
    private UnityEvent<UnitInformation> OnUnitAssign;

    private UnitInformation _unit;
    public UnitInformation Unit
    {
        get => _unit;
        set
        {
            if (value == null || value == _unit)
                return;

            _unit = value;

            OnUnitAssign?.Invoke(_unit);
        }
    }
    public void SetUnit(UnitInformation unit, bool withCallBacks = true)
    {
        if (withCallBacks)
            Unit = unit;
        else
            _unit = unit;
    }
    private void Start()
    {
        if (_importFromTeamOnStart)
            Unit = _teamNumber.GetTeamUnitID().GetUnitInformation();
    }
}
