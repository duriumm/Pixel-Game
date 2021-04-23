using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private const float KnockbackAcceleration = 0.5f;
    [SerializeField]
    protected float maxSpeed = 5;
    [SerializeField]
    private float acceleration = 5;
    [SerializeField]
    protected Animator animator;

    private Health health;
    protected Vector2 movementDir;
    protected Vector2 faceDir = Vector3.zero;
    private Rigidbody2D body;
    
    //Don't you just love properties and ternary operators? I do.
    private float EffectiveAcceleration =>
        health != null && health.KnockedBack ?
        KnockbackAcceleration : acceleration;

    //public float bootsSpeedGang;          // Use when equipment is being added

    //public GameObject feetSocketTest;     // Use when equipment is being added


    protected virtual void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<Health>();
    }

    protected virtual void Update()
    {
        if (animator != null)
        {
            if (faceDir != Vector2.zero)
            {
                // Save the last move position of the character so we can load the Idle animation correctly based on last move position
                animator.SetFloat("LastMoveX", faceDir.x);
                animator.SetFloat("LastMoveY", faceDir.y);
            }
            animator.SetFloat("Horizontal", faceDir.x);
            animator.SetFloat("Vertical", faceDir.y);
            animator.SetFloat("Speed", maxSpeed);
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
        var velocityStep = difference.normalized * EffectiveAcceleration;
        velocityStep = Vector2.ClampMagnitude(velocityStep, difference.magnitude);
        body.velocity += velocityStep;
    }
}
