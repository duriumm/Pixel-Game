using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [SerializeField]
    protected float cooldawn = 1;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private GameObject weapon;

    protected bool readyToAttack = true;
    
    protected virtual void Attack()
    {
        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
        readyToAttack = false;  //This will be set to true after `timeBetweenAttack` seconds have passed
        AttackImpl();
        StartCoroutine(waitForCooldown());
    }

    protected IEnumerator waitForCooldown()
    {
        yield return new WaitForSeconds(cooldawn);
        readyToAttack = true;
    }

    // Implementation of the attack, different for different child classes
    protected abstract void AttackImpl(); 
}
