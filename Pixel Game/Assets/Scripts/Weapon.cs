using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int power;
    public int Power => power;
    [SerializeField]
    protected float cooldawn = 1;
    [SerializeField]
    private AudioClip sound;
    [SerializeField]
    private ShotAttack shotAttack;
    public ShotAttack ShotAttack => shotAttack;
    [SerializeField]
    private bool hasMeleeAttack;
    public bool HasMeleeAttack => hasMeleeAttack;
    [SerializeField]
    private bool hasShotAttack;
    public bool HasShotAttack => hasShotAttack;

    private bool readyToAttack = true;
    public bool ReadyToAttack => readyToAttack;

    protected virtual void Start()
    {
        ShotAttack.Init(gameObject);
    }

    private void FixedUpdate()
    {
        ShotAttack?.Update(Time.fixedDeltaTime);
    }

    public void Attack(Vector2? target = null)
    {
        if (!readyToAttack)
            return;
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, this.transform.position);
        if (HasShotAttack)
            shotAttack.SpawnShot(gameObject, target);
        readyToAttack = false;  //This will be set to true after cooldown
    }

    public IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(cooldawn);
        readyToAttack = true;
    }

    //internal void equip(Weapon itemAttack)
    //{
    //    itemAttack.animator.runtimeAnimatorController = gameObject.GetComponent<Animator>().runtimeAnimatorController;
    //}
}
