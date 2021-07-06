using DataSaving;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum Filters { Unowned = 1, Team = 2 }
public enum SortingType { Tier, Level, Count }
public class MinionCollection : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollContent;
    [SerializeField, AssetsOnly]
    private GameObject _unitButtonPrefab;
    [SerializeField, EnumToggleButtons]
    private Filters _filters;
    [SerializeField, ListDrawerSettings(ListElementLabelName = "EditorName", Expanded = true)]
    private List<Sorter> _sortes;

    [SerializeField, ReadOnly, ListDrawerSettings(Expanded = false, ListElementLabelName = "editorName", IsReadOnly = true)]
    private List<UiUnitController> sceneUnits;

    private InventoryData inventory;
    private UnitsDatabaseSO database;

    private void Awake()
    {
        inventory = DataHandler.Load<InventoryData>();
        var teamData = DataHandler.Load<TeamData>();

        database = Database.UnitsDatabase;

        foreach (UnitSO unit in database.Units)
        {
            if (unit == null)
                continue;

            var UnitButton = Instantiate(_unitButtonPrefab, scrollContent.transform);
            var uiUnit = UnitButton.GetComponent<UiUnitController>();
            uiUnit.UnitInfo = unit.GetUnitInformation();

            sceneUnits.Add(uiUnit);

            UnitButton.SetActive(false);
        }

        SortCollection();

        inventory.OnValueChange += SortCollection;
        teamData.OnValueChange += SortCollection;
        teamData.Team.OnValueChange += SortCollection;
    }
    private void SortCollection()
    {
        Debug.Log("Sorted");

        //Filter
        foreach (var uiUnit in sceneUnits)
        {
            if (_filters.HasFlag(Filters.Unowned) && !uiUnit.UnitInfo.unitSO.Owned)
            {
                uiUnit.gameObject.SetActive(false);
                continue;
            }

            if (_filters.HasFlag(Filters.Team) && uiUnit.UnitInfo.unitSO.TeamNumber != null)
            {
                uiUnit.gameObject.SetActive(false);
                continue;
            }

            uiUnit.gameObject.SetActive(true);
        }

        //Sort
        sceneUnits.Sort(ListHelper.CombineComparers(_sortes.ConvertAll((x) => x.Comparer)));

        //Set index in hierarchy 
        for (int i = 0; i < sceneUnits.Count; i++)
            sceneUnits[i].transform.SetSiblingIndex(i);
    }



    [Serializable, InlineProperty, HideLabel]
    private class Sorter
    {
        [HideLabel, EnumToggleButtons, HorizontalGroup(MaxWidth = 1)]
        public SortingType sorting = SortingType.Tier;
        [HorizontalGroup(LabelWidth = 50), GUIColor("ToggleColor")]
        public bool reverse = false;
        public Comparison<UiUnitController> Comparer => GetComparer(sorting);
#if UNITY_EDITOR
        private string EditorName => sorting.ToString();
        private Color ToggleColor => reverse ? Color.red : new Color(0.8f,1f,0.8f);
#endif

        protected Comparison<UiUnitController> GetComparer(SortingType sortingType)
        {
            switch (sortingType)
            {
                case SortingType.Tier:
                    return (reverse) ? ReverseComparer<UiUnitController>(CompareTiers) : CompareTiers;
                case SortingType.Level:
                    return (reverse) ? ReverseComparer<UiUnitController>(CompareLevels) : CompareLevels;
                case SortingType.Count:
                    return (reverse) ? ReverseComparer<UiUnitController>(CompareCount) : CompareCount;
                default:
                    return (a, b) => 0;
            }
        }

        protected virtual int CompareTiers(UiUnitController a, UiUnitController b)
        {
            if (a?.UnitInfo?.unitSO == null)
            {
                if (b?.UnitInfo?.unitSO == null)
                    return 0;

                return -1;
            }
            else if (b?.UnitInfo?.unitSO == null)
                return 1;

            return a.UnitInfo.unitSO.Tier.CompareTo(b.UnitInfo.unitSO.Tier);
        }
        protected virtual int CompareLevels(UiUnitController a, UiUnitController b)
        {
            if (a?.UnitInfo?.slotData == null)
            {
                if (b?.UnitInfo?.slotData == null)
                    return 0;

                return -1;
            }
            else if (b?.UnitInfo?.slotData == null)
                return 1;

            return a.UnitInfo.slotData.Level.CompareTo(b.UnitInfo.slotData.Level);
        }
        protected virtual int CompareCount(UiUnitController a, UiUnitController b)
        {
            if (a?.UnitInfo?.slotData == null)
            {
                if (b?.UnitInfo?.slotData == null)
                    return 0;

                return -1;
            }
            else if (b?.UnitInfo?.slotData == null)
                return 1;

            return a.UnitInfo.slotData.Count.CompareTo(b.UnitInfo.slotData.Count);
        }

        private int CompareInfoNulls(UiUnitController a, UiUnitController b)
        {
            if (a?.UnitInfo?.slotData == null)
            {
                if (b?.UnitInfo?.slotData == null)
                    return 0;

                return -1;
            }
            else if (b?.UnitInfo?.slotData == null)
                return 1;

            return 0;
        }
        private int CompareSONulls(UiUnitController a, UiUnitController b)
        {
            if (a?.UnitInfo?.unitSO == null)
            {
                if (b?.UnitInfo?.unitSO == null)
                    return 0;

                return -1;
            }
            else if (b?.UnitInfo?.unitSO == null)
                return 1;

            return 0;
        }
        private Comparison<T> ReverseComparer<T>(Comparison<T> comparer)
        {
            return Reverse;
            int Reverse(T a, T b) => -1 * comparer(a, b);
        }
    }
}
