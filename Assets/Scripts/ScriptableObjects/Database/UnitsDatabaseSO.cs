using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = Database.SODatabaseFol + "Units")]
public class UnitsDatabaseSO : BaseDatabaseSO<UnitSO>
{
    [ContextMenu("Sort Database")]
    public void SortDatabase() => Content.Sort((a, b) => a.ID.CompareTo(b.ID));
}