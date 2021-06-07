using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private const float AccelerationDuringKnockback = 0.5f;
    private const float KnockbackDuration = 0.5f;
    private const float KnockbackSpeed = 7;
    [SerializeField] bool HasIdleAnim;
    [SerializeField] protected float maxSpeed = 5;
    [SerializeField] private float acceleration = 5;
    [SerializeField] protected Animator animator;
    protected Rigidbody2D rbody;
    private float currentAcceleration;
    private int paramId_LastMoveX;
    private int paramId_LastMoveY;
    private int paramId_Horizontal;
    private int paramId_Vertical;
    private int paramId_Speed;

    public Vector2 MovementDir { get; set; }
    public Vector2 FaceDir { get; set; }
    public Vector2 Position
    {
        get => rbody.position;
        set => rbody.position = value;
    }

    public Vector2 Velocity
    {
        get => rbody.velocity;
        set => rbody.velocity = value;
    }

    protected virtual void Start()
    {
        //Make sure z is 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        
        rbody = gameObject.GetComponent<Rigidbody2D>();
        currentAcceleration = acceleration;

        if (animator != null)
        {
            paramId_LastMoveX = animator.GetParamId("LastMoveX");
            paramId_LastMoveY = animator.GetParamId("LastMoveY");
            paramId_Horizontal = animator.GetParamId("Horizontal");
            paramId_Vertical = animator.GetParamId("Vertical");
            paramId_Speed = animator.GetParamId("Speed");
        }
}

    protected virtual void Update()
    {
        if (animator != null)
        {
            if (FaceDir != Vector2.zero)
            {
                // Save the last move position of the character so we can load the Idle animation correctly based on last move position
                animator.TrySetFloat(paramId_LastMoveX, FaceDir.x);
                animator.TrySetFloat(paramId_LastMoveY, FaceDir.y);
            }
            Vector2 movementAnim = MovementDir == Vector2.zero && HasIdleAnim ? Vector2.zero : FaceDir;
            animator.TrySetFloat(paramId_Horizontal, movementAnim.x);
            animator.TrySetFloat(paramId_Vertical, movementAnim.y);
            animator.TrySetFloat(paramId_Speed, maxSpeed);
        }
    }

    protected virtual void FixedUpdate()
    {
        Vector2 velocityTarget = MovementDir;
        if (velocityTarget != Vector2.zero)
            velocityTarget = velocityTarget.normalized * maxSpeed;
        var difference = velocityTarget - Velocity;
        if (difference == Vector2.zero)
            return;
        var velocityStep = difference.normalized * currentAcceleration;
        velocityStep = Vector2.ClampMagnitude(velocityStep, difference.magnitude);
        Velocity += velocityStep;
    }

    public virtual IEnumerator KnockBack(Vector3 sourcePoint)
    {
        Velocity = (transform.position - sourcePoint).normalized * KnockbackSpeed;
        currentAcceleration = AccelerationDuringKnockback;
        yield return new WaitForSeconds(KnockbackDuration);
        currentAcceleration = acceleration;
    }
}
