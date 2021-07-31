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

#if UNITY_EDITOR
    private string editorName => gameObject.name;
#endif
    private UnitInformation _unitInfo;
    public UnitInformation UnitInfo
    {
        get => _unitInfo;
        set
        {
            if (value == null || value == _unitInfo)
                return;

            _unitInfo = value;

            OnUnitAssign?.Invoke(_unitInfo);

            if (LevelManager.instance != null)
                LevelManager.instance.LoadedMinion(_unitInfo);
        }
    }
    public void SetUnit(UnitInformation unit, bool withCallBacks = true)
    {
        if (withCallBacks)
            UnitInfo = unit;
        else
            _unitInfo = unit;
    }
    private void Start()
    {
        if (_importFromTeamOnStart)
            UnitInfo = _teamNumber.GetTeamUnitID().GetUnitInformation();
    }
}
