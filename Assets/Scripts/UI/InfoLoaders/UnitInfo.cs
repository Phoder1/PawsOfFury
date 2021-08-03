using Assets.Stats;
using DataSaving;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class UnitInfo : MonoBehaviour
{
    [FoldoutGroup("Refrences")]

    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private UnityEvent<string> _name;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private UnityEvent<string> _description;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private UnityEvent<Sprite> _uiSprite;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    public UnityEvent<Color> _borderColor;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    public UnityEvent<Color> _backgroundColor;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    public UnityEvent<bool> _owned;

    [SerializeField, TabGroup("Refrences/Data", "UnitSlot")]
    private UnityEvent<int> _amount;
    [SerializeField, TabGroup("Refrences/Data", "UnitSlot")]
    private UnityEvent<int> _level;

    [SerializeField, TabGroup("Refrences/Data", "Unit")]
    private UnityEvent<int> _goldValue;
    [SerializeField, TabGroup("Refrences/Data", "DefualtStats")]
    private UnityEvent<float> _maxHealth;
    [SerializeField, TabGroup("Refrences/Data", "DefualtStats")]
    private UnityEvent<float> _walkSpeed;

    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private UnityEvent<float> _range;
    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private UnityEvent<float> _attackSpeed;
    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private UnityEvent<float> _damage;
    public void Load(UnitInformation unitInformation)
    {
        if (unitInformation == null)
            return;

        UnitSO unitSO = unitInformation.unitSO;
        Load(unitSO);

        var unitSlot = unitInformation.slotData;

        if (unitSlot == null)
            unitSlot = unitSO.SlotData;

        Load(unitSlot);

        if (unitSO != null)
        {
            if (unitSlot != null)
                Load(unitSO.GetPrefab(unitSlot.Level).GetComponent<Unit>());
            else if (unitSO.OneStarPrefab != null)
                Load(unitSO.OneStarPrefab.GetComponent<Unit>());
        }
    }
    public void Load(UnitSO unitSO)
    {
        if (unitSO == null)
            return;

        _name?.Invoke(unitSO.UnitName);
        _description?.Invoke(unitSO.Description);
        _uiSprite?.Invoke(unitSO.UiSprite);
        _borderColor?.Invoke(unitSO.BorderColor);
        _backgroundColor?.Invoke(unitSO.BackgroundColor);
        _owned?.Invoke(unitSO.Owned);
    }
    public void Load(UnitSlotData unitSlotData)
    {
        if (unitSlotData == null)
            return;

        _amount?.Invoke(unitSlotData.Count);
        _level?.Invoke(unitSlotData.Level);
    }
    public void Load(Unit unit)
    {
        if (unit == null)
            return;

        Load(unit.DefualtStats);
        Load(unit.projectile);

        _goldValue?.Invoke(unit.goldValue);
    }
    public void Load(DefualtStats defualtStats)
    {
        _maxHealth?.Invoke(defualtStats.MaxHP);
        _walkSpeed?.Invoke(defualtStats.WalkSpeed);
    }
    public void Load(ProjectileData projectileData)
    {
        if (projectileData == null)
            return;

        _range?.Invoke(projectileData.targeting.RulesOrder[0].range);
        _attackSpeed?.Invoke(projectileData.attackSpeed);

        Projectile projectile = projectileData.gameobject.GetComponent<Projectile>();
        if (projectile == null)
            return;

        if (_damage != null)
        {
            if (projectile.EffectsData != null)
            {
                EffectDataSO damage = Array.Find(projectile.EffectsData, (x) => x.AffectedStat == StatType.HP);

                if (damage != null && damage.Amount < 0)
                    _damage?.Invoke(damage.Amount * -1);
                else
                    _damage?.Invoke(0);
            }
            else
                _damage?.Invoke(0);
        }
    }
}
