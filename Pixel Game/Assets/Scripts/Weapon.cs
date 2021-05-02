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
        // If attached to a character with an Attack script, make the character the owner of the weapon
        // so that spawned shots appear at the character's positiion
        // If instead attached to a lootable item, the owner will be set when equipping the item
        if (GetComponent<Attack>() != null)
            SetOwner(gameObject);
        ShotAttack.Init(gameObject);
    }

    public void SetOwner(GameObject owner)
    {
        shotAttack.SetOwnerOfFiringWeapon(owner);
    }

    private void FixedUpdate()
    {
        ShotAttack?.Update(Time.fixedDeltaTime);
    }

    public void Attack(Vector2? direction = null)
    {
        if (!readyToAttack)
            return;
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, this.transform.position);
        if (HasShotAttack)
            shotAttack.SpawnShot((Vector2)direction);
    }

    public IEnumerator WaitForCooldown()
    {
        readyToAttack = false;
        yield return new WaitForSeconds(cooldawn);
        readyToAttack = true;
    }
}
