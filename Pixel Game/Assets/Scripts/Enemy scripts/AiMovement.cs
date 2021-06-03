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
    private float idleUpperDistance;

    
    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        var enemyAttack = gameObject.GetComponent<AiAttack>();
        float attackRange = enemyAttack == null ? 0 : enemyAttack.AttackRange;
		idleUpperDistance = attackRange * 0.7f;
        //Stand still within 3% of retreatUpperDistance to prevent rapid switching between chase and backaway
        retreatUpperDistance = idleUpperDistance * 0.97f;
        aiPath = new AiPath(this);
	}

    protected override void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(playerTransform.position, transform.position);

        // If player is far away, roam around
        // If close, chase
        // If too close, back away but stay well within attack range
        bool backAway = false;
        if (playerDistance < retreatUpperDistance) //Back away
        {
            aiPath.Destination = transform.position + (transform.position - playerTransform.position).normalized * retreatUpperDistance;
            backAway = true;
        }
        else if (playerDistance < ChaseUpperDistance) //Chase
        {
            if (playerDistance < idleUpperDistance)
                aiPath.Destination = null;
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
            if (aiPath.Destination == null)
                aiPath.Destination = (Vector2)transform.position + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 20;
            else
                aiPath.Destination = null;
        }
        
        if (aiPath.Destination != null)
        {
            faceDir = aiPath.CurrentDir;
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

    public override IEnumerator KnockBack(Vector3 sourcePoint)
    {
        rbody.bodyType = RigidbodyType2D.Dynamic;
        yield return base.KnockBack(sourcePoint);
        rbody.bodyType = RigidbodyType2D.Kinematic;
    }
}

class AiPath
{
    private int targetWaypointIndex;
    private Path path;
    private Movement movement;
    private Seeker seeker;
    private bool calculatingPath;
    private Vector2? destination;
    private Vector2? queuedDestination;
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

    public Vector2? Destination
    {
        get => destination;
        set
        {
            if (value == null)
            {
                seeker.CancelCurrentPathRequest();
                path = null;
                destination = queuedDestination = null;
            }
            if (value != null)
                CalculatePathAsync((Vector2)value);
        }
    }

    private void CalculatePathAsync(Vector2 target)
    {
        if (!calculatingPath)
        {
            // If no path is currently being calculated, start calculating new path immediately
            calculatingPath = true;
            queuedDestination = null;
            seeker.StartPath(CurrentPosition, target);
        }
        else
        {
            // Queue destination until current calculation is finished
            // overwriting any previously queued destination
            queuedDestination = target;
        }
    }

    public Vector2 TargetWaypoint
    {
        // Returns position of currently targeted waypoint
        // If there is no waypoint, return our current position
        get
        {
            if (path == null || Waypoints.Count == 0)
                return CurrentPosition;
            return Waypoints[targetWaypointIndex];
        }
    }

    public Vector2 CurrentDir => TargetWaypoint - CurrentPosition;
    public Vector2 CurrentPosition => movement.transform.position;
    public List<Vector3> Waypoints => path.vectorPath;

    public AiPath(Movement movement)
    {
        WaypointDetectionRadius = 0.5f;
        this.movement = movement;
        seeker = movement.GetComponent<Seeker>();
        
        // Called when a path has been calculated
        seeker.pathCallback = (path) =>
        {
            if (path.error)
            {
                // There was an error, probably because we cancelled calculation
                Destination = null;
                return;
            }
            this.path = path;
            destination = Waypoints.Last();
            
            //Start moving towards second waypoint, because first one is current position
            targetWaypointIndex = 1;
            
            //Signals that we're ready to calculate a new path
            calculatingPath = false;

            //Start a new path calculation if there's a queued destination
            if (queuedDestination != null)
                CalculatePathAsync((Vector2)queuedDestination);
        };
    }

    //Call this from Update or FixedUpdate of Movement class
    public void Update()
    {
        movement.MovementDir = CurrentDir; //This is Vector2.Zero if path is null

        if (path == null)
            return;
        
        //Check if we are close to target waypoint and switch to next one
        if (Vector2.SqrMagnitude(CurrentPosition - TargetWaypoint) < sqrWaypointDetectionRadius)
        {
            if (++targetWaypointIndex >= Waypoints.Count)
                targetWaypointIndex = Waypoints.Count - 1;
        }
    }
}
