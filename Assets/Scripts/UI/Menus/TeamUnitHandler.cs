using DataSaving;
using Sirenix.OdinInspector;
using UnityEngine;

public class TeamUnitHandler : MonoBehaviour
{
    [SerializeField, MinValue(1), MaxValue(4)]
    private int _unitTeamNumber;
#if UNITY_EDITOR
    private Color Yellow => Color.yellow;
#endif
    [SerializeField, GUIColor("Yellow")]
    private bool _allowAnyUnit;

    private TeamData selectionSaver;
    private UnitInformation unitInfo;
    public UnitInformation UnitInfo
    {
        get => unitInfo;
        set
        {
            if (unitInfo == value)
                return;

            unitInfo = value;

            selectionSaver.Team[_unitTeamNumber - 1] = (byte)unitInfo.unitSO.ID;
            DataHandler.Save(typeof(TeamData));
        }
    }
    private void Awake()
    {
        selectionSaver = DataHandler.Load<TeamData>();
    }
}
