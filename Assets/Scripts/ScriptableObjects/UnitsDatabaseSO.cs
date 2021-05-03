using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Database")]
public class UnitsDatabaseSO : ScriptableObject
{
    [SerializeField]
    private List<UnitSO> units;
    public List<UnitSO> Units => units;

    [ContextMenu("Sort Database")]
    public void SortDatabase() => units.Sort((a, b) => a.ID.CompareTo(b.ID));
}