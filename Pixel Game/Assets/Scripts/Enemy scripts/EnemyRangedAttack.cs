using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    [SerializeField]
    private float particleSpeed = 2f;
    private GameObject attackParticle;
    private Vector3 particleVelocity = Vector3.zero;
    private static ObjectPool attackParticlePool;
    [SerializeField]
    private GameObject AttackParticleTemplate;
    private bool UpdatingAttackParticle => attackParticle != null;

    void Awake()
    {
        if (attackParticlePool == null)
        {
            attackParticlePool = new ObjectPool(AttackParticleTemplate, 10);
        }
    }

    void FixedUpdate()
    {
        if (UpdatingAttackParticle)
            UpdateAttackParticle();
    }

    private void SpawnAttackParticle()
    {
        // Spawn the particle effect gameobject from pool on top of the enemy
        var particlePos = gameObject.transform.position;
        particlePos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 
        attackParticle = attackParticlePool.Spawn(particlePos);

        // Calculate movement vector
        particleVelocity = playerGameObject.transform.position - gameObject.transform.position;
        particleVelocity = particleVelocity.normalized * particleSpeed;

        //Wait a predetermined amount of time before destroying attack particle and allowing a new attack to begin
        StartCoroutine(WaitForNextAttack(() => DestroyAttackParticle()));
    }

    protected override void Attack()
    {
        SpawnAttackParticle();
    }

    private void UpdateAttackParticle()
    {
        // Moves the particle attack towards the player by adding movementVector * fixedDeltatime
        // to the partiucle attack object. This makes it so particle keeps flying past the player
        attackParticle.transform.position += particleVelocity * Time.fixedDeltaTime;
    }

    public void DestroyAttackParticle()
    {
        attackParticlePool.Destroy(attackParticle);
        attackParticle = null;
    }
}
