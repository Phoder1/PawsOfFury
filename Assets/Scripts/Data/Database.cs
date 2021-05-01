using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Database
{
    public readonly static UnitsDatabaseSO UnitsDatabase = Resources.Load<UnitsDatabaseSO>("UnitsDatabase");
    public readonly static UnitAssetsSO UnitAssets = Resources.Load<UnitAssetsSO>("UnitAssets");
    public readonly static Sprite[] TierCrystals = UnitAssets.TierCrystals;
}
