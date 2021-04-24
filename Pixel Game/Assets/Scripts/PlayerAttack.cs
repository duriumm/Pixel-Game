using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {
    private Animator playerAnimator;

    void Start() {
        playerAnimator = this.gameObject.GetComponent<Animator>();
    }

    protected override void AttackImpl(){
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine(){
        // Play the attack animation
        playerAnimator.SetBool("isAttacking", true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        playerAnimator.SetBool("isAttacking", false);
    }
}
