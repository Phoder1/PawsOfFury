using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BlackBoard;

public class LevelManager : MonoSingleton<LevelManager>
{
    //[Style(AttributeStyle.FoldOut)]
    public bool disabled;
    [Hide(nameof(disabled),false, 1)]
    [SerializeField] Transform levelEnd;
    [Rename("entity UIs obj")]
    [SerializeField] Transform entityUIsObj;
    public Transform EntityUIsObj => entityUIsObj;
    public Tilemap tilemap;


    List<Unit> units;
    List<Enemy> enemies;
    public Vector3 LevelEndPos => levelEnd.position;

    public Unit[] Units => units.ToArray();
    public Enemy[] Enemies => enemies.ToArray();
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
                UiManager._instance.SetGold(Gold);
            }
        }
    }
    void UpdateGoldValue() => Gold = startingGold;

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
                break;
            case Enemy enemy:
                enemies.Remove(enemy);
                break;
        }
    }

    public override void Awake()
    {
        base.Awake();
        BlackBoard.levelManager = _instance;
        units = new List<Unit>();
        enemies = new List<Enemy>();
    }
    private void Start()
    {
        UpdateGoldValue();
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
