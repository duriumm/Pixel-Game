using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField]
    private bool canBeAttacked;
    [SerializeField]
    private bool canAttack;
    [SerializeField]
    private float attackCooldown = 1;
    [SerializeField]
    private GameObject owner;
    private DateTime lastAttackTime;
    private Health health;
    private Attack Attack
    {
        get
        {
            var attack = owner.GetComponent<Attack>();
            if (attack == null)
                attack = owner.GetComponent<Weapon>().Owner.GetComponent<Attack>();
            return attack;
        }
    }
    
    private void Start()
    {
        SetOwner(owner);
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
        health = owner.GetComponent<Health>();
     }

    //Inflict damage on colliding object
    private void OnTriggerStay2D(Collider2D collider)
    {
        //Check cooldown
        if ((DateTime.Now - lastAttackTime).TotalSeconds < attackCooldown)
            return;

        var colliderDamage = collider.gameObject.GetComponent<Damage>();
        
        //Can't inflict damage if in same group
        if (colliderDamage == null || colliderDamage.Attack.Group == Attack.Group)
            return;

        //Check if we can inflict damage and if the colliding object can take damage
        if (colliderDamage.canBeAttacked && canAttack)
        {
            var projectile = GetComponent<Projectile>();
            if (projectile != null)
            {
                if (projectile.DestroyOnHit)
                    projectile.Destroy();
            }
            
            lastAttackTime = DateTime.Now;
            colliderDamage.health.TakeDamage(Attack.CurrentWeapon.Power, transform.position);
        }
    }
}
