using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoSingleton<LevelManager>
{
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
