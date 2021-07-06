using Refrences;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataSaving
{
    [Serializable]
    public class InventoryData : DirtyData, ISaveable
    {
        [SerializeField]
        private DirtyDataList<UnitSlotData> _units = new DirtyDataList<UnitSlotData>(false)
        {
            new UnitSlotData(0,1,1, false),
            new UnitSlotData(1,1,1, false),
            new UnitSlotData(2,1,1, false),
            new UnitSlotData(3,1,1, false),
        };
        public DirtyDataList<UnitSlotData> Units { 
            get => _units; 
            set => Setter(ref _units, value); 
        }
        public override bool IsDirty => base.IsDirty || _units.IsDirty;

        protected override void OnClean()
        {
            base.OnClean();
            _units.Clean();
        }
    }
    [Serializable]
    public class UnitSlotData : DirtyData
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

        [SerializeField]
        private byte _count;
        public byte Count { get => _count; set => Setter(ref _count, value); }

        public UnitSlotData(byte ID, byte level, byte count, bool dirty = true)
        {
            _ID = ID;
            _level = level;
            _count = count;
            IsDirty = dirty;
        }
        public UnitSO UnitSO => Database.UnitsDatabase.Units.Find((x) => x.ID == ID);
    }
    [Serializable]
    public class TeamData : DirtyData, ISaveable
    {
        [SerializeField]
        private DirtyStructList<byte> _team = new DirtyStructList<byte>(false)
        {
            0,
            1,
            2,
            3,
        };

        public TeamData()
        {
            _team.OnValueChange += ValueChanged;
        }

        public DirtyStructList<byte> Team
        {
            get => _team;
            set => Setter(ref _team, value);
        }
        public override bool IsDirty => base.IsDirty || _team.IsDirty;
        protected override void OnClean()
        {
            base.OnClean();
            _team.Clean();
        }

    }
}
