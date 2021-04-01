using System;
using UnityEngine;
public class Lich : Unit
{
    [SerializeField] ProjectileData lichAttackProjectile;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        projectile.callback = Reaction;
    }
    void Reaction(Entity[] entities, Entity target)
    {
        Debug.Log(entities.Length * 2);
        EntityHit hit = Targets.GetEntityHit(this, lichAttackProjectile.targeting);
        if (hit != null && hit.entity != null)
        {
            target = hit.entity;
            GameObject projectileObj = Instantiate(lichAttackProjectile.gameobject, transform.position, Quaternion.identity);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            Array.ForEach(projectileScript.effects, (x) => { x.amount -= entities.Length * 2; });
            projectileScript.Init(this, target);
        }
    }
}
