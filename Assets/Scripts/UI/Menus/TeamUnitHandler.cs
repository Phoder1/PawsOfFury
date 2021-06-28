using DataSaving;
using Sirenix.OdinInspector;
using UnityEngine;

public class TeamUnitHandler : MonoBehaviour
{
    [SerializeField, MinValue(1), MaxValue(4)]
    private int _unitTeamNumber;
    private Color Yellow => Color.yellow;
    [SerializeField, GUIColor("Yellow")]
    private bool _allowAnyUnit;

    private TeamData selectionSaver;
    private UnitSO unitSO;
    public UnitSO UnitSO
    {
        get => unitSO;
        set
        {
            if (unitSO == value)
                return;

            unitSO = value;

            selectionSaver.Team[_unitTeamNumber - 1] = new UnitData((byte)unitSO.ID, 1, false);
            DataHandler.Save(typeof(TeamData));
        }
    }
    private void Start()
    {
        selectionSaver = DataHandler.GetData<TeamData>();

        GetComponent<UiUnitController>().Unit = selectionSaver.Team[_unitTeamNumber - 1].UnitSO;
    }
}
