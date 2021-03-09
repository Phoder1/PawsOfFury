using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] Transform levelEnd;

    List<Unit> units;
    List<Enemy> enemies;
    public Vector3 levelEndPos => levelEnd.position;
    public void AddToList(Unit unit) => units.Add(unit);
    public void RemoveFromList(Unit unit) => units.Remove(unit);
    public void AddToList(Enemy enemy) => enemies.Add(enemy);
    public void RemoveFromList(Enemy enemy) => enemies.Remove(enemy);

    public override void Awake()
    {
        base.Awake();
        units = new List<Unit>();
        enemies = new List<Enemy>();
    }

    public EntityHit<Enemy> GetClosestEnemy(GameObject gameObject)
    {
        float minDistance = Mathf.Infinity;
        Enemy closestEnemy = null;
        if (enemies.Count != 0)
        {
            closestEnemy = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                float distance = Vector3.Distance(enemy.transform.position, gameObject.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return new EntityHit<Enemy>(closestEnemy, minDistance);
    }
    public EntityHit<Unit> GetClosestUnit(GameObject gameObject)
    {
        float minDistance = Mathf.Infinity;
        Unit closestUnit = null;
        if (units.Count != 0)
        {
            closestUnit = units[0];
            foreach (Unit unit in units)
            {
                float distance = Vector3.Distance(gameObject.transform.position, unit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestUnit = unit;
                }
            }
        }
        return new EntityHit<Unit>(closestUnit, minDistance);
    }

}
public class EntityHit<T>
{
    public T entity;
    public float distance;

    public EntityHit(T entity, float distance)
    {
        this.entity = entity;
        this.distance = distance;
    }
}
