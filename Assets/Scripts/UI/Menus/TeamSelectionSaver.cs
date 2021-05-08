using DataSaving;
using Refrences;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionSaver : MonoBehaviour
{
    [SerializeField]
    private ObjectRefrence[] teamRefrences;

    private TeamData _teamData;
    private UnitsDatabaseSO _unitsDatabase;

    private void Awake()
    {
        _teamData = DataHandler.GetData<TeamData>();
        _unitsDatabase = Database.UnitsDatabase;

        for (int i = 0; i < teamRefrences.Length; i++)
        {
            teamRefrences[i].Value = _teamData.Team[i].UnitSO;
        }

    }
}
