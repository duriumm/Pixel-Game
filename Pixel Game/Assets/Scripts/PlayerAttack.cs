using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Animator meleeAnimator;
    
    private AttackSpawner fist;
    private AttackSpawner weapon;

    void Start()
    {
        fist = gameObject.GetComponent<AttackSpawner>();
        if (fist != null && fist.animator == null)
            fist.animator = meleeAnimator;
    }

    public void Attack()
    {
        if (weapon != null )
            weapon.SpawnAttack();
        else if (fist != null)
            fist.SpawnAttack();
    }

    public void EquipWeapon(AttackSpawner weapon)
    {
        weapon.animator = weapon.HasShotAttack ?
            null : meleeAnimator;
        this.weapon = weapon;
    }
}
