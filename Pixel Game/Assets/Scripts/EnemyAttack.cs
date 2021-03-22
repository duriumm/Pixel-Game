using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public float AttackRange = 3;
	public GameObject particleAttackPrefab;
    public GameObject mainCamera;
    public float particleMovementSpeed = 2f;
	// Implement this next !!
	//public AudioClip preAttackSound;
	public AudioClip attackSound;

	private GameObject playerGameObject;
    private GameObject particleAttackObject;
    private Vector3 movementVector = Vector3.zero;

	bool InRange => (playerGameObject.transform.position - gameObject.transform.position).sqrMagnitude < SqrAttackRange;
	bool UpdatingParticleAttackObject => particleAttackObject != null;
	float SqrAttackRange => AttackRange * AttackRange; //Avoid square root calculation in exchange for an extra multiplication

	// Start is called before the first frame update
	void Start()
    {
        // TO-DO
        // Fix a nicer way of getting the player gameobject?
        // TO-DO
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
		StartCoroutine(SpawnNewAttacks());
    }

	//Repeatedly check if a new attack should start
	//Do the check at a fixed slow interval to save cpu cycles
	IEnumerator SpawnNewAttacks()
	{
		while (true)
		{
			if (InRange && !UpdatingParticleAttackObject)
				SpawnParticleAttackObject();
			yield return new WaitForSeconds(0.2f);
		}
	}

	void FixedUpdate()
    {
		if (UpdatingParticleAttackObject)
			UpdateParticleAttackObject();
	}

    private void SpawnParticleAttackObject()
    {
		// Spawn the particle effect gameobject from a prefab on top of the ghast enemy
		// TO-DO
		// Here i would like to have object pooling to use the same 1 ,2 or 3 attack objects over and over
		// TO-DO
		particleAttackObject = Instantiate(particleAttackPrefab) as GameObject;

		//Set particle attack object to enemys position
		particleAttackObject.transform.position = gameObject.transform.position;
        // we change the Z axis since otherwise the particle effect doesnt play correctly 
        particleAttackObject.transform.position = new Vector3(particleAttackObject.transform.position.x, particleAttackObject.transform.position.y, -1);

       	// Calculation movement vector
		movementVector = playerGameObject.transform.position - particleAttackObject.transform.position;
		movementVector.z = 0;
		movementVector = movementVector.normalized * particleMovementSpeed;

        // Play the attack sound at the player position
        AudioSource.PlayClipAtPoint(attackSound, mainCamera.transform.position);
        
		//Wait a predetermined amount of time before destroying attack particle and allowing a new attack to begin
		StartCoroutine(WaitForNextAttack());
	}

	private void UpdateParticleAttackObject()
    {
        // Moves the particle attack towards the player by adding movementVector * fixedDeltatime
        // to the partiucle attack object. This makes it so particle keeps flying past the player
		particleAttackObject.transform.position += movementVector * Time.fixedDeltaTime;
    }

    IEnumerator WaitForNextAttack()
    {
        yield return new WaitForSeconds(2);
		DestroyParticleAttackObject();
	}

    public void DestroyParticleAttackObject()
    {
		//Todo: use pooling, see todo in SpawnParticleAttackObject
		Destroy(particleAttackObject);
		particleAttackObject = null;
	}
    // Implement this next to make a pre attack sound
    //IEnumerator GeneralWaitForSeconds(int secondsToWait)
    //{
    //    AudioSource.PlayClipAtPoint(preAttackSound, mainCamera.transform.position);
    //    yield return new WaitForSeconds(secondsToWait);

    //}
}
