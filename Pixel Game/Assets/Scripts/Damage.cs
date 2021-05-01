using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public enum Group { Players, Mobs }

    [SerializeField]
    private bool canBeAttacked;
    [SerializeField]
    private bool canAttack;
    private int attackPower => attack.Power;
    [SerializeField]
    private float attackCooldown = 1;
    [SerializeField]
    private Group group;
    [SerializeField]
    public GameObject owner;
            
    private DateTime lastAttackTime;
    private Health health;
    private Attack attack;

    private void Start()
    {
        health = owner.GetComponent<Health>();
        if (canAttack)
            attack = owner.GetComponent<Attack>();
    }

    //Inflict damage on colliding object
    private void OnTriggerStay2D(Collider2D collider)
    {
        Debug.Log("Damage OnCollision");
        //Check cooldown
        if ((DateTime.Now - lastAttackTime).TotalSeconds < attackCooldown)
            return;

        var colliderDamage = collider.gameObject.GetComponent<Damage>();
        
        //Can't inflict damage if in same group
        if (colliderDamage == null || colliderDamage.group == group)
            return;

        //Check if we can inflict damage and if the colliding object can take damage
        if (colliderDamage.canBeAttacked && canAttack)
        {
            Debug.Log(attack);
            lastAttackTime = DateTime.Now;
            colliderDamage.health.TakeDamage(attackPower, transform.position);
        }
    }
}
