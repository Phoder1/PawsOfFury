using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataSaving
{
    #region Lists

    [Serializable]
    public abstract class BaseDirtyList<T> : List<T>, IDirtyData
    {
        protected bool _isDirty;

        public void ValueChanged() => _isDirty = true;
        public abstract bool IsDirty { get; }
        public abstract void Saved();
        protected abstract void OnValueSet(int index);
        public new T this[int index]
        {
            get => base[index];
            set
            {
                base[index] = value;
                OnValueSet(index);
            }
        }
        public new int Capacity { get; set; }
        public new void Add(T item)
        {
            base.Add(item);
            ValueChanged();
        }
        public new void Clear()
        {
            base.Clear();
            ValueChanged();
        }
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            ValueChanged();
        }
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            ValueChanged();
        }
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            ValueChanged();
        }
        public new int RemoveAll(Predicate<T> match)
        {
            var x = base.RemoveAll(match);
            ValueChanged();
            return x;
        }
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            ValueChanged();
        }
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            ValueChanged();
        }
        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            ValueChanged();
        }
        public new void Reverse()
        {
            base.Reverse();
            ValueChanged();
        }
        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            ValueChanged();
        }
        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            ValueChanged();
        }
        public new void Sort()
        {
            base.Sort();
            ValueChanged();
        }
        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            ValueChanged();
        }
        public new void TrimExcess()
        {
            base.TrimExcess();
            if (Capacity > Count)
                ValueChanged();
        }


    }
    [Serializable]
    public class DirtyDataList<T> : BaseDirtyList<T> where T : IDirtyData
    {
        public override bool IsDirty
        {
            get
            {
                if (_isDirty)
                    return true;

                return Exists((x) => IsDirty);
            }
        }

        public override void Saved()
        {
            ForEach((X) => X.Saved());
            _isDirty = false;
        }

        protected override void OnValueSet(int index) => _isDirty |= this[index].IsDirty;

    }
    [Serializable]
    public class DirtyStructList<T> : BaseDirtyList<T> where T : struct
    {
        public override bool IsDirty => _isDirty;
        public override void Saved() => _isDirty = false;
        protected override void OnValueSet(int index) => _isDirty = true;

    }
    #endregion
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
