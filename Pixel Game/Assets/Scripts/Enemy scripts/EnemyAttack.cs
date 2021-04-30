﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : ShotAttack
{
    [SerializeField]
    private AudioClip preAttackSound;
    [SerializeField]
    private float attackRange = 1;
    public float AttackRange => attackRange;
    [SerializeField]
    private bool enablePreAttack;

    private EnemyHealth enemyHealth;
    private float EnemyHp => enemyHealth.Hp;
    protected GameObject playerGameObject;
    
    private bool InRange => (playerGameObject.transform.position - gameObject.transform.position).sqrMagnitude < SqrAttackRange;
	private float SqrAttackRange => attackRange * attackRange; //Avoid square root calculation in exchange for an extra multiplication
  
    protected override void Start()
    {
        base.Start();
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
        enemyHealth = this.gameObject.GetComponent<EnemyHealth>();
        StartCoroutine(StartNewAttacks());
    }

	//Repeatedly check if a new attack should start
	//Do the check at a fixed slow interval to save cpu cycles
	IEnumerator StartNewAttacks()
	{
		while (true)
		{
            if (InRange && readyToAttack && EnemyHp > 0)
            {
                if (enablePreAttack)
                    yield return PreAttack();
                if (IsShotAttack)
                    AttackAt(playerGameObject.transform.position);
                
            }
			yield return new WaitForSeconds(0.2f);
		}
	}

	protected virtual IEnumerator PreAttack()
	{
        //Play pre-attack sound
        if (preAttackSound != null)
            AudioSource.PlayClipAtPoint(preAttackSound, this.transform.position);
        
        //Play pre-attack animation (blinking)
        var material = gameObject.GetComponent<SpriteRenderer>().material;
		var originalColor = material.color;
        var darkColor = originalColor * 0.9f;
        for (int i = 0; i < 4; i++)
		{
            material.color = originalColor;
			yield return new WaitForSeconds(0.05f);
            material.color = darkColor;
            yield return new WaitForSeconds(0.05f);
		}
		material.color = originalColor;
	}
}
