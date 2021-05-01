using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    [SerializeField]
    private int power;
    public int Power => power;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    protected float cooldawn = 1;
    [SerializeField]
    private AudioClip sound;
    [SerializeField]
    private ShotAttack shotAttack;
    public ShotAttack ShotAttack => shotAttack;
    public bool HasShotAttack => shotAttack.ShotTemplate != null;
    
    protected bool readyToAttack = true;
    private int paramId_isAttacking;
    
    protected virtual void Start()
    {
        ShotAttack.Init(gameObject);
        if (animator != null)
            paramId_isAttacking = animator.GetParamId("isAttacking");
    }

    private void FixedUpdate()
    {
        ShotAttack?.Update(Time.fixedDeltaTime);
    }

    public void SpawnAttack(Vector2? target = null)
    {
        if (!readyToAttack)
            return;
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, this.transform.position);
        readyToAttack = false;  //This will be set to true after cooldown
        if (animator != null)
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
        animator.TrySetBool(paramId_isAttacking, true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        animator.TrySetBool(paramId_isAttacking, false);
    }
}
