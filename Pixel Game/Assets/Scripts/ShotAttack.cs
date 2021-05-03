﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShotAttack
{
    [SerializeField]
    private float shotSpeed = 2f;
    private ObjectPool shotPool;
    [SerializeField]
    private GameObject shotTemplate;
    public GameObject ShotTemplate => shotTemplate;
    [SerializeField]
    private float shotTimeToLive = 2f;
    
    private List<Shot> shots = new List<Shot>();

    private GameObject ownerOfFiringWeapon; //Needed for correct spawning pos

    public void Init(GameObject firingWeapon)
    {
        if (shotTemplate != null)
        {
            var damage = shotTemplate.GetComponent<Damage>();
            damage.owner = firingWeapon;
            shotPool = new ObjectPool(shotTemplate, 20);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var shot in shots)
            shot.Update(deltaTime);
        shots.RemoveAll((shot) => !shot.IsActive);
    }

    public void SpawnShot(Vector2 direction)
    {
        var pos = ownerOfFiringWeapon.transform.position;
        pos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 
        var velocity = direction.normalized * shotSpeed;
        shots.Add(new Shot(shotTimeToLive, shotPool, pos, velocity));
    }

    public void SetOwnerOfFiringWeapon(GameObject ownerOfFiringWeapon)
    {
        this.ownerOfFiringWeapon = ownerOfFiringWeapon;
    }

    public void DestroyShots()
    {
        foreach (var shot in shots)
            shot.Destroy();
        shots.Clear();        
    }

    private void OnDestroy()
    {
        DestroyShots();
    }
}

class Shot
{
    private float timeToLive;
    private GameObject gameObject;
    public GameObject GameObject => gameObject;
    private Vector3 velocity;
    private ObjectPool pool;

    public bool IsActive => timeToLive > 0;

    public Shot(float timeToLive, ObjectPool pool, Vector2 pos, Vector2 velocity)
    {
        this.timeToLive = timeToLive;
        this.pool = pool;
        this.gameObject = pool.Spawn(pos);
        this.velocity = velocity;
    }

    public void Update(float deltaTime)
    {
        timeToLive -= deltaTime;
        this.gameObject.transform.position += velocity * deltaTime;
        if (timeToLive < 0)
            pool.Destroy(gameObject);
    }

    internal void Destroy()
    {
        this.pool.Destroy(this.gameObject);
    }
}