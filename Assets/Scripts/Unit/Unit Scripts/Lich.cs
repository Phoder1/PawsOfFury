using UnityEngine;
[RequireComponent(typeof(Unit))]
public class Lich : MonoBehaviour
{
    [SerializeField] ProjectileData projectile;
    Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        unit.projectile.callback = Reaction;
    }
    void Reaction(Entity[] entities, Entity target)
    {
        Debug.Log(entities.Length * 2);
        EntityHit hit = unit.GetEntityHit(unit.stats.GetStatValue(StatType.Range), projectile.targeting);
        if (hit != null && hit.entity != null)
        {
            target = hit.entity;
            GameObject projectileObj = Instantiate(projectile.gameobject, unit.transform.position, Quaternion.identity);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.effects[0].amount -= entities.Length * 2;
            projectileScript.Init(unit, target);
        }

    }
}
