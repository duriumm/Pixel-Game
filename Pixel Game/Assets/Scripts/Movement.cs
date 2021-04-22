﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    protected float maxSpeed = 5;
    [SerializeField]
    private float acceleration = 5;

    private Health health;
    protected Vector2 movementDir;
    private Rigidbody2D body;
    //public Animator bootsAnimator;        // Use when equipment is being added
    public Animator animator;

    //Don't you just love properties and ternary operators? I do.
    private float EffectiveAcceleration =>
        health != null && health.KnockedBack ?
        0.7f : acceleration;

    //public float bootsSpeedGang;          // Use when equipment is being added

    //public GameObject feetSocketTest;     // Use when equipment is being added


    protected virtual void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<Health>();
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
