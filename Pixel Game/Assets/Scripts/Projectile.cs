using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Projectile : MonoBehaviour
{
    private float timeToLive;
    private Vector3 velocity;
    private bool destroyOnCollision;
    private ObjectPool pool;

    public bool IsActive => timeToLive > 0;

    public void Shoot(float timeToLive, ObjectPool pool, Vector2 velocity, bool destroyOnCollision)
    {
        this.timeToLive = timeToLive;
        this.pool = pool;
        this.velocity = velocity;
        this.destroyOnCollision = destroyOnCollision;
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

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (destroyOnCollision)
            Destroy();
    }
}
