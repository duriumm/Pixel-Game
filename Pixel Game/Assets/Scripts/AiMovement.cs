using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiMovement : Movement
{
    public enum TargetingStateEnum
    {
        Idle,
        Retreat,
        Follow,
    }
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip retreatSound;
    [SerializeField] private AudioClip followSound;

    [SerializeField] private bool RoamWhenIdle;
    [SerializeField] protected Transform targetTransform;
    [SerializeField] private float followUpperDistance = 4; //Stop following above this distance
    [SerializeField] protected float followLowerDistance = 0; //Stop following belovw this distance
    [SerializeField] protected float retreatUpperDistance = 0; //Stop retreating above this distance
    protected bool retreatBackwards;
    private float roamStartTime;
    private float roamDuration;
    private const float MinRoamDuration = 2;
    private const float MaxRoamDuration = 5;
    private AiPath aiPath;
    private TargetingStateEnum targetingState;

    public TargetingStateEnum TargetingState 
    { 
        get => targetingState; 
        private set
        {
            if (targetingState == value)
                return;
            targetingState = value;
            switch (value)
            {
                case TargetingStateEnum.Retreat:
                    playSound(retreatSound);
                    break;
                case TargetingStateEnum.Follow:
                    playSound(followSound);
                    break;
                case TargetingStateEnum.Idle:
                    playSound(idleSound);
                    break;
            }
        }
    }

    protected override void Start()
    {
        aiPath = new AiPath(this);
        base.Start();
    }

    protected override void FixedUpdate()
    {
        aiPath.Update();
        UpdateTargetingState();
        base.FixedUpdate();
    }

    private void UpdateTargetingState()
    {
        float targetDistance = 0;
        if (targetTransform != null)
            targetDistance = Vector3.Distance(targetTransform.position, transform.position);
        if (targetTransform != null && targetDistance < retreatUpperDistance) //Retreat
        {
            TargetingState = TargetingStateEnum.Retreat;
            aiPath.Destination = transform.position + (transform.position - targetTransform.position).normalized * retreatUpperDistance;
        }
        else if (targetTransform != null && targetDistance < followUpperDistance)
        {
            if (targetDistance < followLowerDistance) //Between retreaiting and following, don't change state
                aiPath.Destination = null;
            else //Follow
            {
                TargetingState = TargetingStateEnum.Follow;
                aiPath.Destination = targetTransform.position;
            }
        }
        else //Idle
        {
            TargetingState = TargetingStateEnum.Idle;
            if (!RoamWhenIdle)
                aiPath.Destination = null;
            else if (Time.time - roamStartTime > roamDuration) //Roam
            {
                //If moving, stop moving. If not moving, start moving in random direction
                roamDuration = Random.Range(MinRoamDuration, MaxRoamDuration);
                roamStartTime = Time.time;
                if (aiPath.Destination == null)
                    aiPath.Destination = (Vector2)transform.position + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 20;
                else
                    aiPath.Destination = null;
            }
        }

        if (aiPath.Destination != null)
        {
            faceDir = aiPath.CurrentDir;
            if (TargetingState == TargetingStateEnum.Retreat)
                faceDir *= -1;
        }
    }

    public override IEnumerator KnockBack(Vector3 sourcePoint)
    {
        //Activate collisions during knockback
        rbody.bodyType = RigidbodyType2D.Dynamic;
        yield return base.KnockBack(sourcePoint);
        rbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetDestination(Vector2 position)
    {
        targetTransform = null;
        aiPath.Destination = position;
    }

    public void SetTarget(Transform transform)
    {
        targetTransform = transform;
    }

    private void playSound(AudioClip sound)
    {
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, transform.position);
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
            //Signals that we're ready to calculate a new path
            calculatingPath = false;

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
            
            //Start a new path calculation if there's a queued destination
            if (queuedDestination != null)
                CalculatePathAsync((Vector2)queuedDestination);
        };
    }

    //Call this from Update or FixedUpdate of Movement class
    public void Update()
    {
        movement.MovementDir = CurrentDir; //This is Vector2.Zero if we have no path

        if (path == null)
            return;
        
        //Check if we are close to currently targetetd waypoint and switch to next one
        if (Vector2.SqrMagnitude(CurrentPosition - TargetWaypoint) < sqrWaypointDetectionRadius)
        {
            if (++targetWaypointIndex >= Waypoints.Count)
                targetWaypointIndex = Waypoints.Count - 1;
        }
    }
}
