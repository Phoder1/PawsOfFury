using DataSaving;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public GameObject GetPrefab(int level)
    {
        switch (level)
        {
            case 1:
                return OneStarPrefab;
            case 2:
                return TwoStarPrefab;
            case 3:
                return ThreeStarPrefab;
            default:
                throw new NotImplementedException();
        }
    }
    [SerializeField]
    private List<UnitSO> higherChanceUnits;
    public int Count
    {
        get
        {
            UnitSlotData unit = DataHandler.Load<InventoryData>().Units.Find((x) => x.ID == ID);
            if (unit != null)
                return unit.Count;
            return 0;
        }
    }
    public Sprite TierCrystal => Database.UnitAssets.TierAssets[Tier - 1].CrystalSprite;
    public Color BackgroundColor => Database.UnitAssets.TierAssets[Tier - 1].BackgroundColor;
    public Color BorderColor => Database.UnitAssets.TierAssets[Tier - 1].BorderColor;
    public UnitSlotData SlotData => DataHandler.Load<InventoryData>().Units.Find((x) => x.ID == ID);
    public bool Owned
    {
        get
        {
            var data = SlotData;
            return data != null && data.Count > 0;
        }
    }
    public int? TeamNumber
    {
        get
        {
        var index = DataHandler.Load<TeamData>()?.Team?.FindIndex((x) => x == ID);
            if (index == -1)
                return null;

            return index + 1;
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
public class UnitInformation
{
    public UnitSO unitSO;
    public UnitSlotData slotData;

    public UnitInformation(UnitSO unitSO, UnitSlotData slotData)
    {
        this.unitSO = unitSO;
        this.slotData = slotData;
    }
}