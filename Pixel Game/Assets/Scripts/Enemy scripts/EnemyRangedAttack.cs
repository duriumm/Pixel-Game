using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    [SerializeField]
    private float particleSpeed = 2f;
    private ObjectPool attackParticlePool;
    [SerializeField]
    private GameObject attackParticleTemplate;
    [SerializeField]
    private float attackParticleTimeToLive = 2f;
    
    private List<Particle> attackParticles = new List<Particle>();

    protected override void Start()
    {
        base.Start();
        attackParticlePool = new ObjectPool(attackParticleTemplate, 20);
    }

    void FixedUpdate()
    {
        foreach (var particle in attackParticles)
            particle.Update(Time.fixedDeltaTime);
        attackParticles.RemoveAll((particle) => !particle.IsActive);
    }

    private void SpawnAttackParticle()
    {
        var pos = gameObject.transform.position;
        pos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 

        var velocity = playerGameObject.transform.position - gameObject.transform.position;
        velocity = velocity.normalized * particleSpeed;

        attackParticles.Add(
            new Particle(
                attackParticleTimeToLive,
                attackParticlePool,
                pos,
                velocity
                )
            );
    }

    protected override void Attack()
    {
        SpawnAttackParticle();
    }

    public void DestroyAttackParticles()
    {
        foreach (var particle in attackParticles)
            particle.Destroy();
        attackParticles.Clear();        
    }

    private void OnDestroy()
    {
        DestroyAttackParticles();
    }
}

class Particle
{
    private float timeToLive;
    private GameObject gameObject;
    private Vector3 velocity;
    private ObjectPool pool;
    private Vector3 pos;

    public bool IsActive => timeToLive > 0;

    public Particle(float timeToLive, ObjectPool pool, Vector3 pos, Vector3 velocity)
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
