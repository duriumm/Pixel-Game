using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Projectile : MonoBehaviour
{
    private float timeToLive;
    private Vector3 velocity;
    private bool destroyOnHit;
    public bool DestroyOnHit => destroyOnHit;
    private ObjectPool pool;

    public bool IsActive => timeToLive > 0;

    public void Shoot(float timeToLive, ObjectPool pool, Vector2 velocity, bool destroyOnCollision)
    {
        this.timeToLive = timeToLive;
        this.pool = pool;
        this.velocity = velocity;
        transform.rotation = Quaternion.identity;
        float angle = Vector2.Angle(velocity, new Vector2(1, 0));
        if (velocity.y < 0)
            angle *= -1;
        //-45 is to compensate for the arrow sprite being rotated 45 degrees
        transform.Rotate(0, 0, angle - 45); 
        Debug.Log(angle);
        this.destroyOnHit = destroyOnCollision;
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
