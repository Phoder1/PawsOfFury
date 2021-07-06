using DataSaving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Database
{
    public const string SOFolder = "ScriptableObjects/";
    public const string SOBehaveFol = SOFolder + "Behaviour/";
    public const string SODataFol = SOFolder + "Data/";
    public const string SODatabaseFol = SOFolder + "Database/";

    private static UnitsDatabaseSO _unitsDatabase;
    public static UnitsDatabaseSO UnitsDatabase
    {
        get
        {
            if(_unitsDatabase == null)
                _unitsDatabase = Resources.Load<UnitsDatabaseSO>("UnitsDatabase");

            return _unitsDatabase;
        }
    }
    private static UnitAssetsSO _unitAssets;
    public static UnitAssetsSO UnitAssets
    {
        get
        {
            if (_unitAssets == null)
                _unitAssets = Resources.Load<UnitAssetsSO>("UnitAssets");

            return _unitAssets;
        }
    }
    public static UnitSO GetUnitSO(this byte ID) => UnitsDatabase.Content.Find((x) => x.ID == ID);
    public static UnitInformation GetUnitInformation(this UnitSO SO) => new UnitInformation(SO, SO.SlotData);
    public static UnitInformation GetUnitInformation(this byte ID)
    {
        UnitSO SO = ID.GetUnitSO();
        UnitSlotData slotData = SO.SlotData;
        return new UnitInformation(SO, slotData);
    }
    public static byte GetTeamUnitID(this int teamNumber)
    {

        Debug.Log(teamNumber);
        return DataHandler.Load<TeamData>().Team[teamNumber - 1];
    }
}
