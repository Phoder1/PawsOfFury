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
        RangeMultiplier
    }
    public class EntityStats
    {
        Dictionary<StatType, Stat> statDict;

        public EntityStats()
        {
            statDict = new Dictionary<StatType, Stat>();
        }

        public Stat GetStat(StatType statType) => statDict[statType];
        public float GetStatValue(StatType statType) => GetStat(statType).GetSetValue;
        public void SetStatValue(StatType statType, float value) => GetStat(statType).GetSetValue = value;
        public void AddToStat(StatType statType, float value) => GetStat(statType).GetSetValue += value;
        public void Add(Stat stat) => statDict.Add(stat.statType, stat);
    }
    public class Stat
    {
        public StatType statType;
        private float value;
        public Stat maxStat;
        protected MonoBehaviour entity;
        protected CoroutineHandler coroutineHandler;
        public bool GetIsCapped => maxStat != null;
        private List<Reaction> reactions;

        public Stat(MonoBehaviour entity, StatType statType, float getSetValue, Stat maxStat = null, List<Reaction> reactions = null)
        {
            coroutineHandler = CoroutineHandler._instance;
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
        HealthBar healthBar;
        public HpStat(MonoBehaviour unit, StatType statType, float getSetValue, HealthBar healthBar, Stat maxStat = null, List<Reaction> reactions = null) : base(unit, statType, getSetValue, maxStat, reactions)
        {
            this.healthBar = healthBar;
            UpdateHealthBar();
        }
        public override float GetSetValue
        {
            get => base.GetSetValue;
            set
            {
                float currValue = GetSetValue;
                base.GetSetValue = value;
                if (GetSetValue != currValue && healthBar != null)
                    UpdateHealthBar();
            }
        }
        void UpdateHealthBar() => healthBar.SetValue(GetSetValue / maxStat.GetSetValue);
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
    }

}
