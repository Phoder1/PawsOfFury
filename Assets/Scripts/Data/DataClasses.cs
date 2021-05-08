using Refrences;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataSaving
{
    [Serializable]
    public class InventoryData : DirtyData
    {
        [SerializeField]
        private DirtyDataList<UnitSlotData> _units = new DirtyDataList<UnitSlotData>()
        {
            new UnitSlotData(1,1,1),
            new UnitSlotData(2,1,1),
            new UnitSlotData(3,1,1),
            new UnitSlotData(4,1,1),
        };
        public DirtyDataList<UnitSlotData> Units { 
            get => _units; 
            set => Setter(ref _units, value); 
        }
        public override bool IsDirty => base.IsDirty || _units.IsDirty;

        public override void Saved()
        {
            base.Saved();
            _units.Saved();
        }
    }
    [Serializable]
    public class UnitSlotData : DirtyData
    {
        [SerializeField]
        private UnitData _data = null;
        public UnitData Data { get => _data; set => Setter(ref _data, value); }
        public byte ID => Data.ID;
        public byte Tier => Data.Tier;
        
        [SerializeField]
        private byte _count;
        public byte Count { get => _count; set => Setter(ref _count, value); }

        public override bool IsDirty => base.IsDirty || Data.IsDirty;

        public UnitSlotData(byte ID, byte tier, byte count)
        {
            _data = new UnitData(ID, tier);
            _count = count;
        }
        public static explicit operator UnitData(UnitSlotData slot) => slot.Data;

        public override void Saved()
        {
            base.Saved();
            Data.Saved();
        }
    }
    [Serializable]
    public class UnitData : DirtyData
    {
        [SerializeField]
        private byte _ID;
        public byte ID
        {
            get => _ID;
            set => Setter(ref _ID, value);
        }
        [SerializeField]
        private byte _tier;
        public byte Tier
        {
            get => _tier;
            set => Setter(ref _tier, value);
        }
        public UnitData(byte iD, byte tier)
        {
            ID = iD;
            Tier = tier;
        }
        public UnitSO UnitSO => Database.UnitsDatabase.Units.Find((x) => x.ID == ID);
    }
    [Serializable]
    public class TeamData : DirtyData
    {
        [SerializeField]
        private DirtyDataList<UnitData> _team = new DirtyDataList<UnitData>()
        {
            new UnitData(0,1),
            new UnitData(1,1),
            new UnitData(2,1),
            new UnitData(3,1),
        };
        public DirtyDataList<UnitData> Team
        {
            get => _team;
            set => Setter(ref _team, value);
        }

        public override bool IsDirty => base.IsDirty || _team.IsDirty;

        public override void Saved()
        {
            _team.Saved();
            base.Saved();
        }

    }
}
