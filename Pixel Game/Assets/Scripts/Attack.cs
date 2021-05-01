using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Animator attackAnimator;
    [SerializeField]
    protected float cooldawn = 1;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private ShotAttack shotAttack;
    public ShotAttack ShotAttack => shotAttack;
    public bool HasShotAttack => shotAttack.ShotTemplate != null;
    
    protected bool readyToAttack = true;
    
    protected virtual void Start()
    {
        ShotAttack.Init();
        attackAnimator = this.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        ShotAttack?.Update(Time.fixedDeltaTime);
    }

    public void PerformAttack(Vector2? target = null)
    {
        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
        readyToAttack = false;  //This will be set to true after `timeBetweenAttack` seconds have passed
        if (attackAnimator != null)
            StartCoroutine(PlayAttackAnimation());
        if (HasShotAttack)
            shotAttack.SpawnShot(gameObject, target);
        StartCoroutine(waitForCooldown());
    }

    protected IEnumerator waitForCooldown()
    {
        yield return new WaitForSeconds(cooldawn);
        readyToAttack = true;
    }

    private IEnumerator PlayAttackAnimation()
    {
        // Play the attack animation
        attackAnimator.SetBool("isAttacking", true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        attackAnimator.SetBool("isAttacking", false);
    }
}
