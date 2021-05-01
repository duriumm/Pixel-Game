using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    private Animator meleeAnimator;

    private Weapon defaultWeapon; //Used if no weapon is equipped
    private Weapon equippedWeapon;
    public Weapon CurrentWeapon
    {
        get
        {
            if (equippedWeapon != null)
                return equippedWeapon;
            else if (defaultWeapon != null)
                return defaultWeapon;
            else
                return null;
        }
    }

    private int meleeAnimParamId_isAttacking;

    protected virtual void Start()
    {
        defaultWeapon = gameObject.GetComponent<Weapon>();
        if (meleeAnimator != null)
            meleeAnimParamId_isAttacking = meleeAnimator.GetParamId("isAttacking");
    }

    public void Execute(Vector2? target = null)
    {
        if (CurrentWeapon == null)
            return;
        if (CurrentWeapon.ReadyToAttack)
        {
            if (CurrentWeapon.HasMeleeAttack)
                StartCoroutine(PlayMeleeAnimationCo());
            CurrentWeapon.Attack(target);
            StartCoroutine(CurrentWeapon.WaitForCooldown());
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    private void PlayMeleeAnimation()
    {
        StartCoroutine(PlayMeleeAnimationCo());
    }

    private IEnumerator PlayMeleeAnimationCo()
    {
        meleeAnimator.TrySetBool(meleeAnimParamId_isAttacking, true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        meleeAnimator.TrySetBool(meleeAnimParamId_isAttacking, false);
    }
}
