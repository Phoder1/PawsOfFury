using DataSaving;
using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using static DataSaving.InventoryData;

public class MinionCollection : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollContent;
    [SerializeField, AssetsOnly]
    private UnitButton _unitButtonPrefab;
    private UnitsDatabaseSO database;

    private void Start()
    {
        var inventory = DataHandler.GetData<InventoryData>();
        if (inventory == null)
        {
            inventory = new InventoryData();
            DataHandler.SetData(inventory);
        }
        else
            Debug.Log(inventory);
        database = Database.UnitsDatabase;
        inventory.units[1] = new UnitData(4, 2, 5);
        _ = DataHandler.SaveAllAsync();

        foreach (UnitSO unit in database.Units)
        {
            var UnitButton = Instantiate(_unitButtonPrefab, scrollContent.transform);
            UnitButton.unit = unit;
        }
    }
}
