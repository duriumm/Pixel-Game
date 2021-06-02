using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement : Movement
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
  //  private NavMeshAgent navMeshAgent;
    private Seeker seeker;
    Path currentPath;
    private bool calculatingPath;

    private AIPath aiPath;

    private const float MinRoamDuration = 2;
    private const float MaxRoamDuration = 5;
    private const float ChaseUpperDistance = 4;
	private float retreatUpperDistance;

    private Vector2? destination;
    private Vector2? Destination
    {
        get => destination;
        set
        {
            destination = value;
            if (!calculatingPath)
            {
                seeker.StartPath(transform.position, (Vector2)value);
                calculatingPath = true;
            }
        }
    }

    public Vector2? CurrentPathDirection
    {
        get
        {
            //var path = seeker.GetCurrentPath();
            if (currentPath == null || destination == null)
                return null;
            if (currentPath.vectorPath.Count > 0)
                return currentPath.vectorPath[1] - transform.position;
            else
                return null;
        }
    }

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        enemyTransform = gameObject.transform;
         var enemyAttack = gameObject.GetComponent<AiAttack>();
        float attackRange = enemyAttack == null ? 0 : enemyAttack.AttackRange;
		retreatUpperDistance = attackRange * 0.7f;
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //navMeshAgent.updateRotation = navMeshAgent.updateUpAxis = false;
        //navMeshAgent.isStopped = true;
        seeker = GetComponent<Seeker>();
        seeker.pathCallback = (path) =>
        {
            currentPath = path;
            calculatingPath = false;
            //foreach (var point in currentPath.vectorPath)
            //    Debug.Log(point);
            //Debug.Log("*************");
        };
        aiPath = GetComponent<AIPath>();
	}

    protected override void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(playerTransform.position, enemyTransform.position);

        // If player is far away, roam around
        // If close, chase
        // If too close, back away but stay well within attack range
        bool backAway = false;
        if (playerDistance < retreatUpperDistance * 0.97f) //Back away
        {
            movementDir = enemyTransform.position - playerTransform.position;
            backAway = true;
        }
        else if (playerDistance < ChaseUpperDistance) //Chase
        {
            if (playerDistance < retreatUpperDistance)
                movementDir = Vector2.zero;
            //else
              //  Destination = playerTransform.position;
            //movementDir = playerTransform.position - enemyTransform.position;
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
        //navMeshAgent.SetDestination(playerTransform.position);
        //aiPath.target = transform;
        aiPath.destination = playerTransform.position;
        //movementDir = navMeshAgent.path.corners[0] - transform.position;
        //movementDir = navMeshAgent.desiredVelocity;

        if (Input.GetKeyDown(KeyCode.P))
            Destination = playerTransform.position;

        if (CurrentPathDirection != null)
        {
            movementDir = (Vector2)CurrentPathDirection;
        }
        if (movementDir != Vector2.zero)
        {
            faceDir = movementDir;
            if (backAway)
                faceDir *= -1;
        }
        //navMeshAgent.velocity = new Vector3(movementDir.x, movementDir.y, 0);
        Destination = playerTransform.position;

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
