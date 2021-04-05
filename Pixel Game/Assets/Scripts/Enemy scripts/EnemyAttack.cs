using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    private EnemyHealth enemyHealthObject;
    private float EnemyHealth => enemyHealthObject.enemyHealth;
    protected GameObject playerGameObject;
    private bool readyToAttack = true;
    [SerializeField]
    private AudioClip preAttackSound;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private float attackRange = 1;
    public float AttackRange => attackRange;
    private GameObject mainCamera;
    private bool InRange => (playerGameObject.transform.position - gameObject.transform.position).sqrMagnitude < SqrAttackRange;
	private float SqrAttackRange => attackRange * attackRange; //Avoid square root calculation in exchange for an extra multiplication
    	
	void Start()
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
				yield return StartAttack();
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator StartAttack()
	{
        //Play pre-attack sound
        if (preAttackSound != null)
            AudioSource.PlayClipAtPoint(preAttackSound, this.transform.position);
        
        //Play pre-attack animation (blinking)
        var material = gameObject.GetComponent<SpriteRenderer>().material;
		var originalColor = material.color;
		for (int i = 0; i < 4; i++)
		{
			material.color = Color.white;
			yield return new WaitForSeconds(0.1f);
			material.color = Color.red;
			yield return new WaitForSeconds(0.1f);
		}
		material.color = originalColor;

        // Play the attack sound
        AudioSource.PlayClipAtPoint(attackSound, this.transform.position);

        Attack();
        readyToAttack = false;
	}

    protected abstract void Attack();

	protected IEnumerator WaitForNextAttack(Action onReady)
    {
        yield return new WaitForSeconds(2);
        readyToAttack = true;
        onReady?.Invoke();
	}
}
