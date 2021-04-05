using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject playerObject;
    [SerializeField]
    private float speed = 0.5f;
    [SerializeField]
    private Animator enemyAnimator;
    private GameObject mainCamera;
    [SerializeField]
    private AudioClip ghastAmbientSound;

    private Transform playerTransform;
    private Transform ghastTransform;
    private bool isPlayingAmbientGhastSound = false;
    private float roamStartTime;
    private float roamDuration;
    private Vector3 movementVector;

    private const float MinRoamDuration = 2;
    private const float MaxRoamDuration = 5;
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
        float playerDistance = Vector3.Distance(playerTransform.position, ghastTransform.position);

        // If player is far away, roam around
        // If close, chase to get well within attack range
        // Back away if too close
        Vector3 faceDirection = Vector3.zero;
        if (playerDistance < retreatUpperDistance * 0.99f) //Back away
        {
            movementVector = ghastTransform.position - playerTransform.position;
            faceDirection = -movementVector; //Enemy should face player when backing away
        }
        else if (playerDistance < ChaseUpperDistance) //Chase
        {
            if (playerDistance < retreatUpperDistance)
                movementVector = Vector3.zero;
            else
                movementVector = playerTransform.position - ghastTransform.position;
            //Sound acts as indication that the enemy started chasing
            //Helps the player to differentiate between roaming movement and chasing movement
            if (!isPlayingAmbientGhastSound)
                StartCoroutine(playAmbientSoundAndWait());
        }
        else if (Time.time - roamStartTime > roamDuration) //Update roaming objective
        {
            //If moving, stop moving. If not moving, start moving in random direction
            roamDuration = Random.Range(MinRoamDuration, MaxRoamDuration);
            roamStartTime = Time.time;
            if (movementVector == Vector3.zero)
                movementVector = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            else
                movementVector = Vector3.zero;
        }
        if (faceDirection == Vector3.zero)
            faceDirection = movementVector;

        if (movementVector != Vector3.zero)
        { 
            ghastTransform.position += movementVector.normalized * speed * Time.fixedDeltaTime;
            enemyAnimator.SetFloat("Horizontal", faceDirection.x);
            enemyAnimator.SetFloat("Vertical", faceDirection.y);
            enemyAnimator.SetFloat("Speed", speed);
        }
    }

    private IEnumerator playAmbientSoundAndWait()
    {
        isPlayingAmbientGhastSound = true;
        AudioSource.PlayClipAtPoint(ghastAmbientSound, mainCamera.transform.position);        
        yield return new WaitForSeconds(9);
        isPlayingAmbientGhastSound = false;
    }
}
