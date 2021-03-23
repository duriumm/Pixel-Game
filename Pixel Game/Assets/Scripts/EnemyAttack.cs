using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public float AttackRange = 3;
	public GameObject AttackParticleTemplate;
    public GameObject mainCamera;
    public float particleMovementSpeed = 2f;
	// Implement this next !!
	//public AudioClip preAttackSound;
	public AudioClip attackSound;

	private GameObject playerGameObject;
    private GameObject attackParticle;
    private Vector3 movementVector = Vector3.zero;
	private static ObjectPool attackParticlePool;

	private bool InRange => (playerGameObject.transform.position - gameObject.transform.position).sqrMagnitude < SqrAttackRange;
	private bool UpdatingAttackParticle => attackParticle != null;
	private float SqrAttackRange => AttackRange * AttackRange; //Avoid square root calculation in exchange for an extra multiplication

	void Awake()
	{
		if (attackParticlePool == null)
		{
			attackParticlePool = new ObjectPool(AttackParticleTemplate, 10);
		}
	}
	// Start is called before the first frame update
	void Start()
    {
        // TO-DO
        // Fix a nicer way of getting the player gameobject?
        // TO-DO
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
		StartCoroutine(StartNewAttacks());
    }

	//Repeatedly check if a new attack should start
	//Do the check at a fixed slow interval to save cpu cycles
	IEnumerator StartNewAttacks()
	{
		while (true)
		{
			if (InRange && !UpdatingAttackParticle)
				yield return StartAttack();
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator StartAttack()
	{
		//Blinking to indicate attack will start
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
		SpawnAttackParticle();
	}

	void FixedUpdate()
    {
		if (UpdatingAttackParticle)
			UpdateAttackParticle();
	}

	private void SpawnAttackParticle()
    {
		// Spawn the particle effect gameobject from pool on top of the ghast enemy
		var particlePos = gameObject.transform.position;
		particlePos.z = -1; // we change the Z axis since otherwise the particle effect doesnt play correctly 
		attackParticle = attackParticlePool.Spawn(particlePos);
        
       	// Calculate movement vector
		movementVector = playerGameObject.transform.position - gameObject.transform.position;
		movementVector = movementVector.normalized * particleMovementSpeed;

        // Play the attack sound at the player position
        AudioSource.PlayClipAtPoint(attackSound, mainCamera.transform.position);
        
		//Wait a predetermined amount of time before destroying attack particle and allowing a new attack to begin
		StartCoroutine(WaitForNextAttack());
	}

	private void UpdateAttackParticle()
    {
        // Moves the particle attack towards the player by adding movementVector * fixedDeltatime
        // to the partiucle attack object. This makes it so particle keeps flying past the player
		attackParticle.transform.position += movementVector * Time.fixedDeltaTime;
    }

    IEnumerator WaitForNextAttack()
    {
        yield return new WaitForSeconds(2);
		DestroyAttackParticle();
	}

    public void DestroyAttackParticle()
    {
		attackParticlePool.Destroy(attackParticle);
		attackParticle = null;
	}
    // Implement this next to make a pre attack sound
    //IEnumerator GeneralWaitForSeconds(int secondsToWait)
    //{
    //    AudioSource.PlayClipAtPoint(preAttackSound, mainCamera.transform.position);
    //    yield return new WaitForSeconds(secondsToWait);

    //}
}
