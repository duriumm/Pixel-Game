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
    private ProjectileAttack projectileAttack;
    public ProjectileAttack ProjectileAttack => projectileAttack;
    [SerializeField]
    private bool hasMeleeAttack;
    public bool HasMeleeAttack => hasMeleeAttack;
    [SerializeField]
    private bool hasProjectileAttack;
    public bool HasProjectileAttack => hasProjectileAttack;

    private bool readyToAttack = true;
    public bool ReadyToAttack => readyToAttack;

    protected virtual void Start()
    {
        // If attached to a character with an Attack script, make the character the owner of the weapon
        // so that spawned shots appear at the character's positiion
        // If instead attached to a lootable item, the owner will be set when equipping the item
        if (GetComponent<Attack>() != null)
            SetOwner(gameObject);
        ProjectileAttack.Init(gameObject);
    }

    public void SetOwner(GameObject owner)
    {
        projectileAttack.SetOwnerOfFiringWeapon(owner);
    }

    public void Attack(Vector2? direction = null)
    {
        if (!readyToAttack)
            return;
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, this.transform.position);
        if (HasProjectileAttack)
            projectileAttack.Shoot((Vector2)direction);
    }

    public IEnumerator WaitForCooldown()
    {
        readyToAttack = false;
        yield return new WaitForSeconds(cooldawn);
        readyToAttack = true;
    }
}
