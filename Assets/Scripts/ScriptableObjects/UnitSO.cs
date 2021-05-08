using DataSaving;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static DataSaving.InventoryData;

[CreateAssetMenu(menuName = "SO/UnitSO")]
public class UnitSO : ScriptableObject
{
    [SerializeField]
    private Sprite _uiSprite;
    public Sprite UiSprite => _uiSprite;
    [SerializeField, TextArea]
    private string _description;

    public string Description => _description;

    [SerializeField]
    private byte id;
    public int ID => id;

    [SerializeField, MinValue(1), MaxValue(3)]
    private byte tier = 1;
    public int Tier => tier;

    public int GooValue(int starValue) => (2 * tier) - 1 + starValue;

    [SerializeField]
    private GameObject oneStarPrefab;
    public GameObject OneStarPrefab => oneStarPrefab;

    [SerializeField]
    private GameObject twoStarPrefab;
    public GameObject TwoStarPrefab => twoStarPrefab;

    [SerializeField]
    private GameObject threeStarPrefab;
    public GameObject ThreeStarPrefab => threeStarPrefab;

    [SerializeField]
    private List<UnitSO> higherChanceUnits;
    public int Count
    {
        get
        {
            UnitData unit = DataHandler.GetData<InventoryData>().units.Find((x) => x.ID == ID);
            if (unit != null)
                return unit.Count;
            return 0;

        }
    }
    public Sprite TierCrystal => Database.TierCrystals[Tier - 1];
    public bool Owned
    {
        get
        {
            var data = DataHandler.GetData<InventoryData>().units.Find((x) => x.ID == ID);
            return data != null && data.Count > 0;

        }
    }
    public GameObject GetStarLevel(byte starLevel)
    {
        switch (starLevel)
        {
            case 1:
                return OneStarPrefab;
            case 2:
                return TwoStarPrefab;
            case 3:
                return ThreeStarPrefab;
            default:
                return null;
        }
    }
}