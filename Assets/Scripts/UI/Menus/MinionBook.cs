using DataSaving;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MinionBook : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollContent;

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
        var database = inventory.Database;
        inventory.units[1] = new InventoryData.UnitData(4, 2, 5);
        _ = DataHandler.SaveAllAsync();

        foreach (UnitSO unit in database.Units)
        {
            var unitObj = Instantiate(unit.UiSlotPrefab, scrollContent.transform);
            var image = unitObj.GetComponent<Image>();
            var color = (Array.Exists(inventory.units, (x) => x.ID == unit.ID)) ? Color.green : Color.red;
            image.color = color;
        }
    }
}
