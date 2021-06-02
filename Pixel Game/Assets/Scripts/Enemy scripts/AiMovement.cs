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
    private AiPath aiPath;

    private const float MinRoamDuration = 2;
    private const float MaxRoamDuration = 5;
    private const float ChaseUpperDistance = 4;
	private float retreatUpperDistance;

    
    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        var enemyAttack = gameObject.GetComponent<AiAttack>();
        float attackRange = enemyAttack == null ? 0 : enemyAttack.AttackRange;
		retreatUpperDistance = attackRange * 0.7f;
        aiPath = new AiPath(gameObject);
	}

    protected override void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(playerTransform.position, transform.position);

        // If player is far away, roam around
        // If close, chase
        // If too close, back away but stay well within attack range
        bool backAway = false;
        //Destination = null;
        if (playerDistance < retreatUpperDistance * 0.97f) //Back away
        {
            //movementDir = transform.position - playerTransform.position;
            aiPath.Destination = playerTransform.position;
            backAway = true;
        }
        else if (playerDistance < ChaseUpperDistance) //Chase
        {
            if (playerDistance < retreatUpperDistance)
                movementDir = Vector2.zero;
            else
                aiPath.Destination = playerTransform.position;
            
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
                aiPath.Destination = (Vector2)transform.position + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 10;
            else
            {
                movementDir = Vector2.zero;
                aiPath.Destination = null;
            }
        }
        
        //if (Input.GetKeyDown(KeyCode.P))
        //    Destination = playerTransform.position;

        if (aiPath.CurrentDirection != null)
        {
            movementDir = (Vector2)aiPath.CurrentDirection;
        }
        if (movementDir != Vector2.zero)
        {
            faceDir = movementDir;
            if (backAway)
                faceDir *= -1;
        }
        aiPath.Update();
        base.FixedUpdate();
    }

    private IEnumerator playAmbientSoundAndWait()
    {
        isPlayingAmbientSound = true;
        AudioSource.PlayClipAtPoint(ambientSound, transform.position);        
        yield return new WaitForSeconds(9);
        isPlayingAmbientSound = false;
    }
}

class AiPath
{
    private int targetWaypointIndex;
    private Path path;
    private GameObject gameObject;
    private Seeker seeker;
    private bool calculatingPath;

    private Vector2? destination;
    public Vector2? Destination
    {
        get => destination;
        set
        {
            destination = value;
            if (!calculatingPath && value != null)
            {
                seeker.StartPath(gameObject.transform.position, (Vector2)value);
                calculatingPath = true;
            }
        }
    }

    public Vector2 NextPosition
    {
        get
        {
            if (path == null || destination == null || path.vectorPath.Count < 2)
                return gameObject.transform.position;
            return path.vectorPath[targetWaypointIndex];
        }
    }

    public Vector2 CurrentDirection => NextPosition - CurrentPosition;
    public Vector2 CurrentPosition => gameObject.transform.position;

    public List<Vector3> WayPoints => path.vectorPath;
    private float waypointDetectionRadius;
    private float sqrWaypointDetectionRadius;
    public float WaypointDetectionRadius 
    { 
        get => waypointDetectionRadius;
        set
        {
            waypointDetectionRadius = value;
            sqrWaypointDetectionRadius = value * value;
        }
    }

    public AiPath(GameObject gameObject)
    {
        WaypointDetectionRadius = 0.5f;
        this.gameObject = gameObject;
        seeker = gameObject.GetComponent<Seeker>();
        seeker.pathCallback = (path) =>
        {
            this.path = path;
            targetWaypointIndex = 1;
            calculatingPath = false;
        };
    }

    public void Update()
    {
        if (path != null && Vector2.SqrMagnitude(CurrentPosition - NextPosition) < sqrWaypointDetectionRadius)
        {
            if (++targetWaypointIndex >= WayPoints.Count)
                targetWaypointIndex = WayPoints.Count - 1;

    }
}
}
