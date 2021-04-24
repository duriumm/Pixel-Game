using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public enum Group { Players, Mobs }

    [SerializeField]
    private int attackPower = 0;
    [SerializeField]
    private float attackCooldown = 1;
    [SerializeField]
    private Group group;
    [SerializeField]
    private GameObject damageReceiver;
    
    private DateTime lastAttackTime;
    private Health health;

    private bool CanInflictDamage => attackPower > 0;
    private bool CanTakeDamage => damageReceiver != null;

    private void Start()
    {
        if (damageReceiver != null)
            health = damageReceiver.GetComponent<Health>();
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
        if (colliderDamage.CanTakeDamage && CanInflictDamage)
        {
            lastAttackTime = DateTime.Now;
            colliderDamage.health.TakeDamage(attackPower, transform.position);
        }
    }
}
