using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public class Stat
    {
        public StatType statType;
        private float value;
        public Stat maxStat;
        protected MonoBehaviour unit;
        public bool GetIsCapped => maxStat != null;
        private Reaction[] reactions;

        public Stat(MonoBehaviour unit, StatType statType, float getSetValue, Stat maxStat = null, Reaction[] reactions = null)
        {
            Init();
            this.unit = unit;
            this.statType = statType;
            this.maxStat = maxStat;
            this.reactions = reactions;
            GetSetValue = getSetValue;
        }
        protected virtual void Init() {
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
                        if (reaction.CheckCondition(value))
                            reaction.reactionAction?.Invoke(value);
            }
        }
    }
    public class HpStat : Stat
    {
        Coroutine hideHealthBarCoro;
        HealthBar healthBar;
        public HpStat(MonoBehaviour unit, StatType statType, float getSetValue, HealthBar healthBar, Stat maxStat = null, Reaction[] reactions = null) : base(unit, statType, getSetValue, maxStat, reactions)
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
        void UpdateHealthBar()
        {
            if (hideHealthBarCoro != null)
                unit.StopCoroutine(hideHealthBarCoro);
            Debug.Log("started Coro");
            healthBar.FadeInOut(1, 0.5f);
            hideHealthBarCoro = unit.StartCoroutine(HideHealthBar());
            healthBar.SetValue(GetSetValue / maxStat.GetSetValue);
        }
        IEnumerator HideHealthBar()
        {
            yield return new WaitForSeconds(healthBar.timeUntilDisapear);
            healthBar.FadeInOut(0, 0.5f);
            hideHealthBarCoro = null;
        }
    }
    public class Reaction
    {
        private IReactionCondition reactionCondition;
        public Action<float> reactionAction;

        public Reaction(IReactionCondition reactionCondition, Action<float> reactionAction)
        {
            this.reactionCondition = reactionCondition;
            this.reactionAction = reactionAction;
        }

        public bool CheckCondition(float value) => reactionCondition.CheckCondition(value);
    }
    public interface IReactionCondition
    {
        bool CheckCondition(float value);
    }
    [Serializable]
    public struct DefualtStats
    {
        public float HP;
        public float MaxHP;
        public float WalkSpeed;
        public float Damage;
        public float AttackSpeed;
        public float Range;
    }
    public class Death : IReactionCondition
    {
        public bool CheckCondition(float value) => value == 0;
    }
}
