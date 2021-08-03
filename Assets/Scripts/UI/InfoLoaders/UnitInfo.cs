using Assets.Stats;
using DataSaving;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    [FoldoutGroup("Refrences")]

    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private TextMeshProUGUI _name;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private TextMeshProUGUI _description;
    [SerializeField, TabGroup("Refrences/Data", "UnitSO")]
    private Image _uiSprite;

    [SerializeField, TabGroup("Refrences/Data", "UnitSlot")]
    private TextMeshProUGUI _amount;
    [SerializeField, TabGroup("Refrences/Data", "UnitSlot")]
    private TextMeshProUGUI _level;

    [SerializeField, TabGroup("Refrences/Data", "Unit")]
    private TextMeshProUGUI _goldValue;
    [SerializeField, TabGroup("Refrences/Data", "DefualtStats")]
    private TextMeshProUGUI _maxHealth;
    [SerializeField, TabGroup("Refrences/Data", "DefualtStats")]
    private TextMeshProUGUI _walkSpeed;

    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private TextMeshProUGUI _range;
    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private TextMeshProUGUI _attackSpeed;
    [SerializeField, TabGroup("Refrences/Data", "ProjectileData")]
    private TextMeshProUGUI _damage;
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
            else if(unitSO.OneStarPrefab != null)
                Load(unitSO.OneStarPrefab.GetComponent<Unit>());
        }
    }
    public void Load(UnitSO unitSO)
    {
        if (unitSO == null)
            return;

        if (_name != null)
            _name.text = unitSO.UnitName;

        if (_description != null)
            _description.text = unitSO.Description;

        if (_uiSprite != null)
            _uiSprite.sprite = unitSO.UiSprite;
    }
    public void Load(UnitSlotData unitSlotData)
    {
        if (unitSlotData == null)
            return;

        if (_amount != null)
            _amount.text = unitSlotData.Count.ToString();
        if (_level != null)
            _level.text = unitSlotData.Level.ToString();
    }
    public void Load(Unit unit)
    {
        if (unit == null)
            return;

        Load(unit.DefualtStats);
        Load(unit.projectile);

        if (_goldValue != null)
            _goldValue.text = unit.goldValue.ToString();
    }
    public void Load(DefualtStats defualtStats)
    {
        if (_maxHealth != null)
            _maxHealth.text = defualtStats.MaxHP.ToString();

        if (_walkSpeed != null)
            _walkSpeed.text = defualtStats.WalkSpeed.ToString();
    }
    public void Load(ProjectileData projectileData)
    {
        if (projectileData == null)
            return;

        if (_range != null)
            _range.text = projectileData.targeting.RulesOrder[0].range.ToString();

        if (_attackSpeed != null)
            _attackSpeed.text = projectileData.attackSpeed.ToString();

        Projectile projectile = projectileData.gameobject.GetComponent<Projectile>();
        if (projectile == null)
            return;

        if (_damage != null)
        {
            if (projectile.EffectsData != null)
            {
                EffectData damage = Array.Find(projectile.EffectsData, (x) => x.AffectedStat == StatType.HP);

                if (damage != null && damage.amount < 0)
                    _damage.text = (damage.amount * -1).ToString();
                else
                    _damage.text = string.Empty;
            }
            else
                _damage.text = string.Empty;

        }
    }
}
