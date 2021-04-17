using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotAttack : EnemyAttack
{
    [SerializeField]
    private float shotSpeed = 2f;
    private ObjectPool shotPool;
    [SerializeField]
    private GameObject shotTemplate;
    [SerializeField]
    private float shotTimeToLive = 2f;
    
    private List<Shot> shots = new List<Shot>();

    protected override void Start()
    {
        base.Start();
        shotPool = new ObjectPool(shotTemplate, 20);
    }

    void FixedUpdate()
    {
        foreach (var shot in shots)
            shot.Update(Time.fixedDeltaTime);
        shots.RemoveAll((shot) => !shot.IsActive);
    }

    private void SpawnShot()
    {
        var pos = gameObject.transform.position;
        pos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 

        var velocity = playerGameObject.transform.position - gameObject.transform.position;
        velocity = velocity.normalized * shotSpeed;

        shots.Add(
            new Shot(
                shotTimeToLive,
                shotPool,
                pos,
                velocity
                )
            );
    }

    protected override void Attack()
    {
        SpawnShot();
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
    private Vector3 velocity;
    private ObjectPool pool;
    private Vector3 pos;

    public bool IsActive => timeToLive > 0;

    public Shot(float timeToLive, ObjectPool pool, Vector3 pos, Vector3 velocity)
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
