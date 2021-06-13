using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Database
{
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
}
