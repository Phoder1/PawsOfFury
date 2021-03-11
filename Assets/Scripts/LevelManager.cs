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

    public EntityHit GetClosestEnemy(GameObject gameObject) => GetClosestEntity(gameObject, enemies);
    public EntityHit GetClosestUnit(GameObject gameObject) => GetClosestEntity(gameObject, units);
    public EntityHit GetClosestEntity<T>(GameObject gameObject, List<T> entityList) where T : Entity
    {
        float minDistance = Mathf.Infinity;
        Entity closestEntity = null;
        if (entityList.Count != 0)
        {
            closestEntity = entityList[0];
            foreach (T entity in entityList)
            {
                float distance = Vector3.Distance(gameObject.transform.position, entity.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEntity = entity;
                }
            }
        }
        return new EntityHit(closestEntity, minDistance);
    }

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
}
