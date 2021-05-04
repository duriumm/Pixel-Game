using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileAttack
{
    [SerializeField]
    private float speed = 2f;
    private ObjectPool pool;
    [SerializeField]
    private GameObject projectileTemplate;
    public GameObject ProjectileTemplate => projectileTemplate;
    [SerializeField]
    private float timeToLive = 2f;
    [SerializeField]
    private bool destroyOnCollision = true;
    
    private GameObject ownerOfFiringWeapon; //Needed for correct spawning pos

    public void Init(GameObject firingWeapon)
    {
        if (projectileTemplate != null)
        {
            var damage = projectileTemplate.GetComponent<Damage>();
            damage.owner = firingWeapon;
            pool = new ObjectPool(projectileTemplate, 10);
        }
    }

    public void Shoot(Vector2 direction)
    {
        var pos = ownerOfFiringWeapon.transform.position;
        pos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 
        var velocity = direction.normalized * speed;
        var projectile = pool.Spawn(pos);
        projectile.GetComponent<Projectile>().Shoot(timeToLive, pool, velocity, destroyOnCollision);
    }

    public void SetOwnerOfFiringWeapon(GameObject ownerOfFiringWeapon)
    {
        this.ownerOfFiringWeapon = ownerOfFiringWeapon;
    }
}