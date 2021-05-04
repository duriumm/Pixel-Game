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
    
    //private List<Projectile> shots = new List<Projectile>();

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

    //public void Update(float deltaTime)
    //{
        //foreach (var shot in shots)
          //  shot.Update(deltaTime);
        //shots.RemoveAll((shot) => !shot.IsActive);
    //}

    public void Shoot(Vector2 direction)
    {
        var pos = ownerOfFiringWeapon.transform.position;
        pos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 
        var velocity = direction.normalized * speed;
        //var projectile = GameObject.Instantiate(ProjectileTemplate);
        var projectile = pool.Spawn(pos);
        projectile.GetComponent<Projectile>().Shoot(timeToLive, pool, velocity, destroyOnCollision);
        //shots.Add(new Projectile(projectileTimeToLive, shotPool, pos, velocity));
    }

    public void SetOwnerOfFiringWeapon(GameObject ownerOfFiringWeapon)
    {
        this.ownerOfFiringWeapon = ownerOfFiringWeapon;
    }

    //public void DestroyShots()
    //{
        //foreach (var shot in shots)
          //  shot.Destroy();
        //shots.Clear();        
    //}

    //private void OnDestroy()
    //{
    //    DestroyShots();
    //}
}