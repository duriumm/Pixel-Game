using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Movement
{
    private GameObject playerObject;
    private GameObject mainCamera;
    [SerializeField]
    private AudioClip ambientSound;
    private Transform playerTransform;
    private Transform enemyTransform;
    private bool isPlayingAmbientSound = false;
    private float roamStartTime;
    private float roamDuration;
    
    private const float MinRoamDuration = 2;
    private const float MaxRoamDuration = 5;
    private const float ChaseUpperDistance = 4;
	private float retreatUpperDistance;
    
    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        enemyTransform = gameObject.transform;
         var enemyAttack = gameObject.GetComponent<EnemyAttack>();
        float attackRange = enemyAttack == null ? 0 : enemyAttack.AttackRange;
		retreatUpperDistance = attackRange * 0.7f;
	}

    protected override void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(playerTransform.position, enemyTransform.position);

        // If player is far away, roam around
        // If close, chase to get well within attack range
        // Back away if too close
        faceDir = Vector2.zero;
        if (playerDistance < retreatUpperDistance * 0.97f) //Back away
        {
            movementDir = enemyTransform.position - playerTransform.position;
            faceDir = -movementDir; //Enemy should face player when backing away
        }
        else if (playerDistance < ChaseUpperDistance) //Chase
        {
            if (playerDistance < retreatUpperDistance)
                movementDir = Vector2.zero;
            else
                movementDir = playerTransform.position - enemyTransform.position;
            //Sound acts as indication that the enemy started chasing
            //Helps the player to differentiate between roaming movement and chasing movement
            if (!isPlayingAmbientSound && ambientSound != null)
                StartCoroutine(playAmbientSoundAndWait());
        }
        else if (Time.time - roamStartTime > roamDuration) //Update roaming objective
        {
            //If moving, stop moving. If not moving, start moving in random direction
            roamDuration = Random.Range(MinRoamDuration, MaxRoamDuration);
            roamStartTime = Time.time;
            if (movementDir == Vector2.zero)
                movementDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            else
                movementDir = Vector2.zero;
        }
        if (faceDir == Vector2.zero)
            faceDir = movementDir;

        base.FixedUpdate();
    }

    private IEnumerator playAmbientSoundAndWait()
    {
        isPlayingAmbientSound = true;
        AudioSource.PlayClipAtPoint(ambientSound, this.gameObject.transform.position);        
        yield return new WaitForSeconds(9);
        isPlayingAmbientSound = false;
    }
}
