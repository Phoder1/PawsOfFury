using DataSaving;
using Refrences;
using Sirenix.OdinInspector;
using UnityEngine;
using static DataSaving.InventoryData;

public class MinionCollection : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollContent;
    [SerializeField, AssetsOnly]
    private GameObject _unitButtonPrefab;
    [SerializeField]
    private bool _ownedOnly;
    private UnitsDatabaseSO database;

    private void Awake()
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

        foreach (UnitSO unit in database.Units)
        {
            if (!_ownedOnly || unit.Owned)
            {
                var UnitButton = Instantiate(_unitButtonPrefab, scrollContent.transform);
                UnitButton.GetComponent<EventPassthrough>().DefaultData = unit;
                var importer = UnitButton.GetComponent<DataImporter>();
                importer.DefaultData = unit;
                importer.ImportDefault();
            }
        }
    }
}