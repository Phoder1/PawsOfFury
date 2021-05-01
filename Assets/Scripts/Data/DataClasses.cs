using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataSaving
{
    [Serializable]
    public class InventoryData : DirtyData
    {
        public DirtyDataList<UnitData> units = new DirtyDataList<UnitData>()
        {
            new UnitData(0,1,1),
            new UnitData(0,1,1),
            new UnitData(0,1,1),
            new UnitData(0,1,1),
        };
        public override bool IsDirty => base.IsDirty || units.IsDirty;

        public override void Saved()
        {
            base.Saved();
            units.Saved();
        }

        [Serializable]
        public class UnitData : DirtyData
        {
            [SerializeField]
            private byte _ID;
            public byte ID 
            { 
                get => _ID; 
                set
                {
                    if (_ID == value)
                        return;
                    _ID = value;
                    ValueChanged();
                }
            }
            [SerializeField]
            private byte _tier;
            public byte Tier
            {
                get => _tier;
                set
                {
                    if (_tier == value)
                        return;
                    _tier = value;
                    ValueChanged();
                }
            }
            [SerializeField]
            private byte _count;
            public byte Count
            {
                get => _count;
                set
                {
                    if (_count == value)
                        return;
                    _count = value;
                    ValueChanged();
                }
            }
            public UnitData(byte iD, byte tier, byte count)
            {
                ID = iD;
                Tier = tier;
                Count = count;
            }
        }
    }
}
