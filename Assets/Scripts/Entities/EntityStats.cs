using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Stats
{
    public enum StatType
    {
        HP,
        MaxHP,
        WalkSpeed,
        DamageMultiplier,
        AttackSpeedMultiplier,
        MaxAttackSpeedMultiplier,
        MaxDamageMultiplier,
        RangeMultiplier
    }
    [Serializable]
    public class EntityStats
    {
        public Stat this[StatType index]
        {
            set => this[index] = value;
            get => stats.Find((x) => x.statType == index);
        }

        [SerializeField, ReadOnly]
        private List<Stat> stats =  new List<Stat>();

        public Stat GetStat(StatType statType) => this[statType];
        public float GetStatValue(StatType statType) => GetStat(statType).GetSetValue;
        public void SetStatValue(StatType statType, float value) => GetStat(statType).GetSetValue = value;
        public void AddToStat(StatType statType, float value) => GetStat(statType).GetSetValue += value;
        public void Add(Stat stat) => stats.Add(stat);
    }
    [Serializable]
    public class Stat
    {
        public StatType statType;
        [SerializeField]
        private float value;
        public Stat maxStat;
        protected MonoBehaviour entity;
        protected CoroutineHandler coroutineHandler;
        public bool GetIsCapped => maxStat != null;
        public bool IsFull => GetIsCapped && value == maxStat.value;
        private List<Reaction> reactions;

        public Stat(MonoBehaviour entity, StatType statType, float getSetValue, Stat maxStat = null, List<Reaction> reactions = null)
        {
            Init();
            this.entity = entity;
            this.statType = statType;
            this.maxStat = maxStat;
            this.reactions = reactions;
            GetSetValue = getSetValue;
        }
        protected virtual void Init()
        {
        }
        public virtual float GetSetValue
        {
            get => value;
            set
            {
                this.value = Mathf.Max(value, 0);
                if (maxStat != null)
                    this.value = Mathf.Min(this.value, maxStat.GetSetValue);
                if (reactions != null)
                    foreach (Reaction reaction in reactions)
                        reaction.TryInvokingReaction(this);
            }
        }
    }
    public class HpStat : Stat
    {
        EntityUI entityUI;
        public HpStat(MonoBehaviour unit, StatType statType, float getSetValue, EntityUI entityUI, Stat maxStat = null, List<Reaction> reactions = null) : base(unit, statType, getSetValue, maxStat, reactions)
        {
            this.entityUI = entityUI;
            UpdateHealthBar();
        }
        public override float GetSetValue
        {
            get => base.GetSetValue;
            set
            {
                float currValue = GetSetValue;
                base.GetSetValue = value;
                if (GetSetValue != currValue && entityUI != null)
                    UpdateHealthBar();
            }
        }
        void UpdateHealthBar() => entityUI.SetHealthBarValue(GetSetValue / maxStat.GetSetValue);
    }
    public delegate bool ReactionCondition(Stat stat);
    public class Reaction
    {
        private readonly ReactionCondition reactionCondition;
        public readonly Action<Stat> reactionAction;
        public Reaction(ReactionCondition reactionCondition, Action<Stat> reactionAction)
        {
            this.reactionCondition = reactionCondition;
            this.reactionAction = reactionAction;
        }
        public bool TryInvokingReaction(Stat stat)
        {
            bool conditionMet = CheckCondition(stat);
            if ( conditionMet)
                reactionAction?.Invoke(stat);
            return conditionMet;

        }
        public bool CheckCondition(Stat stat) => reactionCondition(stat);
        public static ReactionCondition DeathCondition => (x) => x.GetSetValue <= 0;
        public static ReactionCondition AlwaysTrue => (x) => true;
        public static implicit operator List<Reaction>(Reaction x) => new List<Reaction>() { x };
    }
    [Serializable]
    public struct DefualtStats
    {
        public float HP;
        public float MaxHP;
        public float WalkSpeed;
        [Tooltip("The maximum attack speed multiplier: 1 means the character can't be buffed, 2 means it can go only up to 200% and so on.")]
        [Min(1)]
        public float MaxAttackSpeedMultiplier;
        [Tooltip("The maximum damage multiplier: 100 means the character can't be buffed, 200 means it can go only up to 200% and so on.")]
        [Min(1)]
        public float MaxDamageMultiplier;
    }

}
