using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float travelTime;
    [SerializeField] float radius;
    [SerializeField] bool AOE;
    [SerializeField] EffectDataSO effectData;

    EntityStats entity;
    public void Init(Vector3 target, EntityStats entity, Action<Collision> callback = null)
    {
        //Todo: Animation and sound
        this.entity = entity;
        transform.DOMove(target, 1).OnComplete(ReachedTarget);
    }

    void ReachedTarget()
    {
        //Todo: Move to projectile
        //Todo: Animation and sound
        new EffectController(entity, effectData);
        Destroy(gameObject);
        //Check hit
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

class ProjectileHit{


}