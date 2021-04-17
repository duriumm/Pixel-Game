using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private AudioClip preAttackSound;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private float attackRange = 1;
    public float AttackRange => attackRange;
    [SerializeField]
    private float timeBetweenAttacks = 2;
    [SerializeField]
    private bool enablePreAttack;

    private EnemyHealth enemyHealthObject;
    private float EnemyHealth => enemyHealthObject.enemyHealth;
    protected GameObject playerGameObject;
    private bool readyToAttack = true;

    private bool InRange => (playerGameObject.transform.position - gameObject.transform.position).sqrMagnitude < SqrAttackRange;
	private float SqrAttackRange => attackRange * attackRange; //Avoid square root calculation in exchange for an extra multiplication
  
    protected virtual void Start()
    {
        // TO-DO
        // Fix a nicer way of getting the player gameobject?
        // TO-DO
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
        enemyHealthObject = this.gameObject.GetComponent<EnemyHealth>();
        StartCoroutine(StartNewAttacks());
    }

	//Repeatedly check if a new attack should start
	//Do the check at a fixed slow interval to save cpu cycles
	IEnumerator StartNewAttacks()
	{
		while (true)
		{
            if (InRange && readyToAttack && EnemyHealth > 0)
            {
                if (enablePreAttack)
                    yield return PreAttack();
                // Play the attack sound
                if (attackSound != null)
                    AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
                readyToAttack = false;  //This will be set to true after `timeBetweenAttack` seconds have passed
                Attack();
                yield return WaitForNextAttack();
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

    protected virtual void Attack()
    {
    }

	protected IEnumerator WaitForNextAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        readyToAttack = true;
	}
}
