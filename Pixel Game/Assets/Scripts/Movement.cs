using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private const float KnockbackAcceleration = 0.5f;
    [SerializeField]
    bool HasIdleAnim;
    [SerializeField]
    protected float maxSpeed = 5;
    [SerializeField]
    private float acceleration = 5;
    [SerializeField]
    protected Animator animator;

    private Health health;
    protected Vector2 movementDir;
    protected Vector2 faceDir = Vector2.zero;
    public Vector2 FaceDir => faceDir;
    private Rigidbody2D body;
    
    private float ActualAcceleration =>
        health != null && health.KnockedBack ?
        KnockbackAcceleration : acceleration;

    private int paramId_LastMoveX;
    private int paramId_LastMoveY;
    private int paramId_Horizontal;
    private int paramId_Vertical;
    private int paramId_Speed;

    protected virtual void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<Health>();
        
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
            if (faceDir != Vector2.zero)
            {
                // Save the last move position of the character so we can load the Idle animation correctly based on last move position
                animator.TrySetFloat(paramId_LastMoveX, faceDir.x);
                animator.TrySetFloat(paramId_LastMoveY, faceDir.y);
            }
            Vector2 movementAnim = movementDir == Vector2.zero && HasIdleAnim ? Vector2.zero : faceDir;
            animator.TrySetFloat(paramId_Horizontal, movementAnim.x);
            animator.TrySetFloat(paramId_Vertical, movementAnim.y);
            animator.TrySetFloat(paramId_Speed, maxSpeed);
        }
    }

    protected virtual void FixedUpdate()
    {
        Vector2 velocityTarget = movementDir;
        if (velocityTarget != Vector2.zero)
            velocityTarget = velocityTarget.normalized * maxSpeed;
        var difference = velocityTarget - body.velocity;
        if (difference == Vector2.zero)
            return;
        var velocityStep = difference.normalized * ActualAcceleration;
        velocityStep = Vector2.ClampMagnitude(velocityStep, difference.magnitude);
        body.velocity += velocityStep;
    }
}
