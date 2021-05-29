using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int damage;
    public int Damage => damage;
    [SerializeField]
    protected float cooldawn = 1;
    public float Cooldown => cooldawn;
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
    private GameObject owner;
    public GameObject Owner
    {
        get => owner;
        set => owner = value;
    }

    public bool ReadyToAttack => readyToAttack;

    public virtual void Start()
    {
        // If attached to a character with an Attack script, make the character the owner of the weapon
        // so that spawned shots appear at the character's positiion
        // If instead attached to a lootable item, the owner will be set when equipping the item
        if (GetComponent<Attack>() != null)
            Owner = gameObject;
        ProjectileAttack.Init(gameObject);
    }
  
    public void Attack(Vector2? direction = null)
    {
        if (!readyToAttack)
            return;
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, Owner.transform.position);
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
