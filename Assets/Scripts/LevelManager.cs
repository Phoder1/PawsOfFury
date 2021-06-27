using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using static IngameBlackBoard;
using CustomAttributes;
using UnityEngine.Events;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] Transform levelEnd;
    [Rename("Entity UIs Obj")]
    [SerializeField] Transform entityUIsObj;
    public Transform EntityUIsObj => entityUIsObj;
    public Tilemap tilemap;

    List<Unit> units;
    List<Enemy> enemies;
    public Vector3 LevelEndPos => levelEnd.position;

    public Unit[] Units => units.ToArray();
    public Enemy[] Enemies => enemies.ToArray();
    [SerializeField] int CheapestMinion;// TODO - assaign this variable to te value of the cheapest minion on his current team
    [SerializeField] UnityEvent PlayerLose;
    [SerializeField] int startingGold;
    int gold;
    public int Gold
    {
        get => gold;
        set
        {
            if (gold != value)
            {
                gold = value;
                IngameUiManager.instance.SetGold(Gold);
                GoldAmountChanged?.Invoke(Gold);
            }
        }
    }
    void UpdateGoldValue() => Gold = startingGold;
    public event Action<float> GoldAmountChanged;

    public void AddToList<T>(T entity) where T : Entity
    {
        switch (entity)
        {
            case Unit unit:
                units.Add(unit);
                break;
            case Enemy enemy:
                enemies.Add(enemy);
                break;
        }
    }
    public void RemoveFromList<T>(T entity) where T : Entity
    {
        switch (entity)
        {
            case Unit unit:
                units.Remove(unit);
                if (Check_Player_Lose())
                    PlayerLose.Invoke();
                break;
            case Enemy enemy:
                enemies.Remove(enemy);
                break;
        }
    }

    public override void OnAwake()
    {
        IngameBlackBoard.levelManager = instance;
        units = new List<Unit>();
        enemies = new List<Enemy>();
    }
    private void Start()
    {
        UpdateGoldValue();
    }

   private bool Check_Player_Lose()
   {
        if(units.Count == 0 && gold < CheapestMinion) 
        {
            return true;
        }
        else
        {
            return false;
        }
   }



}
public static class GlobalEffects
{
    public static bool disableTaunts;
}
public class EntityHit
{
    public Entity entity;
    public float distance;

    public EntityHit(Entity entity, float distance)
    {
        this.entity = entity;
        this.distance = distance;
    }
    public static implicit operator Entity(EntityHit x) => x?.entity;
}
