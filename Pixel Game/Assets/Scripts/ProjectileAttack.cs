using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileAttack
{
    [SerializeField]
    private float speed = 2f;
    public float Speed => speed;
    [SerializeField]
    private GameObject projectileTemplate;
    public GameObject ProjectileTemplate => projectileTemplate;
    [SerializeField]
    private float timeToLive = 2f;
    [SerializeField]
    private bool destroyOnHit = true;
    
    private ObjectPool pool;
    Weapon firingWeapon;

    public void Init(GameObject firingWeapon)
    {
        if (projectileTemplate != null)
        {
            this.firingWeapon = firingWeapon.GetComponent<Weapon>();
            var damage = projectileTemplate.GetComponent<Projectile>();
            damage.SetOwner(firingWeapon);
            pool = new ObjectPool(projectileTemplate, 10);
        }
    }

    public void Shoot(Vector2 direction)
    {
        var pos = firingWeapon.Owner.transform.position;
        var velocity = direction.normalized * speed;
        var projectile = pool.Spawn(pos);
        projectile.GetComponent<Projectile>().Shoot(timeToLive, pool, velocity, destroyOnHit);
    }
}