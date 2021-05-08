using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Projectile : Damage
{
    private float timeToLive;
    private Vector3 velocity;
    private bool destroyOnHit;
    public bool DestroyOnHit => destroyOnHit;
    private ObjectPool pool;
    [SerializeField]
    private float spriteRotation;

    public bool IsActive => timeToLive > 0;

    public void Shoot(float timeToLive, ObjectPool pool, Vector2 velocity, bool destroyOnHit)
    {
        this.timeToLive = timeToLive;
        this.pool = pool;
        this.velocity = velocity;
        transform.rotation = Quaternion.identity;
        float angle = Vector2.Angle(velocity, new Vector2(1, 0));
        if (velocity.y < 0)
            angle *= -1;
        transform.Rotate(0, 0, angle - spriteRotation);
        this.destroyOnHit = destroyOnHit;
    }

    void FixedUpdate()
    {
        timeToLive -= Time.fixedDeltaTime;
        this.gameObject.transform.position += velocity * Time.fixedDeltaTime;
        if (timeToLive < 0)
            Destroy();
    }

    public void Destroy()
    {
        this.pool.Destroy(gameObject);
    }
}
