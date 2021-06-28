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
        private DirtyDataList<UnitSlotData> _units = new DirtyDataList<UnitSlotData>(false)
        {
            new UnitSlotData(0,1,1, false),
            new UnitSlotData(1,1,1, false),
            new UnitSlotData(2,1,1, false),
            new UnitSlotData(4,1,1, false),
        };

        public InventoryData()
        {
            Saved();
        }

        public DirtyDataList<UnitSlotData> Units { 
            get => _units; 
            set => Setter(ref _units, value); 
        }
        public override bool IsDirty => base.IsDirty || _units.IsDirty;

        protected override void OnSaved()
        {
            base.OnSaved();
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
        public byte Tier => Data.Level;
        
        [SerializeField]
        private byte _count;
        public byte Count { get => _count; set => Setter(ref _count, value); }

        public override bool IsDirty => base.IsDirty || Data.IsDirty;

        public UnitSlotData(byte ID, byte tier, byte count, bool dirty = true)
        {
            _data = new UnitData(ID, tier, dirty);
            _count = count;
            IsDirty = dirty;
        }
        public static explicit operator UnitData(UnitSlotData slot) => slot.Data;

        protected override void OnSaved()
        {
            base.OnSaved();
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
        private byte _level;
        public byte Level
        {
            get => _level;
            set => Setter(ref _level, value);
        }
        public UnitData(byte iD, byte tier, bool dirty = true)
        {
            ID = iD;
            Level = tier;
            IsDirty = dirty;
        }
        public UnitSO UnitSO => Database.UnitsDatabase.Units.Find((x) => x.ID == ID);
        public static UnitData GetDataFromSO(UnitSO unitSO)
        {
            var inventory = DataHandler.GetData<InventoryData>();
            var unitSlot = inventory?.Units?.Find((x) => x.ID == unitSO.ID);
            return unitSlot?.Data;
        }

        public bool Equals(UnitData obj)
        {
            if (obj.ID == ID && obj.Level == Level)
                return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    [Serializable]
    public class TeamData : DirtyData
    {
        [SerializeField]
        private DirtyDataList<UnitData> _team = new DirtyDataList<UnitData>(false)
        {
            new UnitData(0,1, false),
            new UnitData(1,1, false),
            new UnitData(2,1, false),
            new UnitData(3,1, false),
        };

        public TeamData()
        {
            Saved();
        }

        public DirtyDataList<UnitData> Team
        {
            get => _team;
            set => Setter(ref _team, value);
        }

        public override bool IsDirty => base.IsDirty || _team.IsDirty;

        protected override void OnSaved()
        {
            base.OnSaved();
            _team.Saved();
        }

    }
}
