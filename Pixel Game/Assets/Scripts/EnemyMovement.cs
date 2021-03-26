using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject playerObject;
    public float speed = 0.5f;
    public Animator enemyAnimator;
    public GameObject mainCamera;
    public AudioClip ghastAmbientSound;

    private Transform playerTransform;
    private Transform ghastTransform;
    private bool isPlayingAmbientGhastSound = false;
	
	private const float AmbientSoundLowerDistance = 1.5f;
	private const float AmbientSoundUpperDistance = 4;
	private const float ChaseUpperDistance = 4;
	private float retreatUpperDistance;

	// Start is called before the first frame update
	void Start()
    {
        // TO-DO
        // Fix a nicer way of getting the player transform position
        // TO-DO
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        ghastTransform = gameObject.transform;
		float attackRange = gameObject.GetComponent<EnemyAttack>().AttackRange;
		retreatUpperDistance = attackRange * 0.7f;
	}

    void FixedUpdate()
    {
        // TO-DO
        // Make a state machine for the ghast for 1 roaming around,
        // 2 chasing player, 3 dying etc etc
        // TO-DO

        Vector2 playerDistanceVector = playerTransform.position - ghastTransform.position;
        float playerDistance = playerDistanceVector.magnitude;
		
        if (!isPlayingAmbientGhastSound && playerDistance > AmbientSoundLowerDistance && playerDistance < AmbientSoundUpperDistance)
        {
            // Plays ambient sound of ghast movement and waits 9 seconds to play it again
            StartCoroutine(waitForAmbientSoundToPlay());
        }

        // If ghast is close to player he starts moving towards the player
        // If well within attack range it backs away to avoid close-range attacks from player
        Vector3 movementVector = Vector3.zero;
        if (playerDistance < retreatUpperDistance) //Back away
            movementVector = ghastTransform.position - playerTransform.position;
        else if (playerDistance < ChaseUpperDistance) //Chase
            movementVector = playerTransform.position - ghastTransform.position;

        if (movementVector != Vector3.zero)
        {
            ghastTransform.position += movementVector.normalized * speed * Time.fixedDeltaTime;
            enemyAnimator.SetFloat("Horizontal", playerDistanceVector.x);
            enemyAnimator.SetFloat("Vertical", playerDistanceVector.y);
            enemyAnimator.SetFloat("Speed", speed);
        }
    }
    private IEnumerator waitForAmbientSoundToPlay()
    {
        isPlayingAmbientGhastSound = true;
        AudioSource.PlayClipAtPoint(ghastAmbientSound, mainCamera.transform.position);        
        yield return new WaitForSeconds(9);
        isPlayingAmbientGhastSound = false;
    }
}
