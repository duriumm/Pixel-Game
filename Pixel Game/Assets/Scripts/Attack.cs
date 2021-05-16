using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackGroup { Players, Mobs }

public class Attack : MonoBehaviour
{
    [SerializeField]
    private Animator meleeAnimator;
    [SerializeField]
    private AttackGroup group;
    public AttackGroup Group => group;
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

    public virtual void Start()
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
                StartCoroutine(PlayMeleeAnimation());
            Vector2 direction;
            if (target != null)
                direction = (Vector2)(target - transform.position);
            else
                direction = GetComponent<Movement>().FaceDir;
            CurrentWeapon.Attack(direction);
            StartCoroutine(CurrentWeapon.WaitForCooldown());
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapon != null)
            equippedWeapon.Owner = null;
        if (weapon != null)
            weapon.Owner = gameObject;
        equippedWeapon = weapon;
    }

    private IEnumerator PlayMeleeAnimation()
    {
        meleeAnimator.TrySetBool(meleeAnimParamId_isAttacking, true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        meleeAnimator.TrySetBool(meleeAnimParamId_isAttacking, false);
    }
}
