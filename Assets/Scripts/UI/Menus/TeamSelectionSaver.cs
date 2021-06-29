using DataSaving;
using Refrences;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionSaver : MonoBehaviour
{

    private TeamData _teamData;
    private UnitsDatabaseSO _unitsDatabase;

    private void Awake()
    {
        _teamData = DataHandler.GetData<TeamData>();
        _unitsDatabase = Database.UnitsDatabase;
    }
}
