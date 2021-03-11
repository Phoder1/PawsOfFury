using System.Collections.Generic;
using UnityEngine;
using static EntityStats;
using Assets.StateMachine;

public enum StatType
{
    HP,
    MaxHP,
    WalkSpeed,
    Damage,
    AttackSpeed,
    Range
}
public partial class Unit : Entity
{ 
    public GameObject projectile;

    [Tooltip("Height 0 is the center of the Unit.")]
    [SerializeField] float healthbarHeight;

    
    Camera mainCam;
    NavScript navScript;

    protected HealthBar healthBar;
    
    StateMachine state;
    float lastAttackTime;
    private void Start()
    {
        lastAttackTime = -Mathf.Infinity;
        LevelManager._instance.AddToList(this);
        mainCam = Camera.main;
        UiObject = Instantiate(UiObject, mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight, transform.rotation);
        healthBar = UiObject.GetComponent<HealthBar>();
        healthBar.Init();
        stats = new EntityStats();
        FillDictionary();
        navScript = GetComponent<NavScript>();
        state = new StateMachine(new WalkState(this));
    }
    private void Update()
    {
        state.State.Update();
        UiObject.transform.position = mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight;
    }
    private void OnDisable()
    {
        LevelManager._instance.AddToList(this);
        state.State = null;
    }
    private void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP,healthBar, maxHp, new
            Reaction[1] {
            new Reaction(new Death(),
            (value) => {Destroy(gameObject); })
            }
            ));
        stats.Add( new Stat(this, StatType.Damage, defualtStats.Damage));
        stats.Add( new Stat(this, StatType.AttackSpeed, defualtStats.AttackSpeed));
        stats.Add( new Stat(this, StatType.WalkSpeed, defualtStats.WalkSpeed));
        stats.Add( new Stat(this, StatType.Range, defualtStats.Range));
    }
}
