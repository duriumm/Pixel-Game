﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    private Animator playerAnimator;
    public AudioClip attackSound;
    private GameObject mainCamera;
    // Start is called before the first frame update
    void Start() {
        playerAnimator = this.gameObject.GetComponent<Animator>();
        mainCamera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update() {

    }

    public void getAttackCoroutine(){
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine(){
        AudioSource.PlayClipAtPoint(attackSound, mainCamera.transform.position);
        // Play the attack animation
        playerAnimator.SetBool("isAttacking", true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        playerAnimator.SetBool("isAttacking", false);
    }
}
