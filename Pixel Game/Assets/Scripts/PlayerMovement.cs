using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    //public Animator bootsAnimator;        // Use when equipment is being added

    protected override void Update()
    {
        // Input for movement on both axis
        movementDir.x = Input.GetAxisRaw("Horizontal");
        movementDir.y = Input.GetAxisRaw("Vertical");
        faceDir = movementDir;
        // Dont delete this, it will be used for equipment later on
        //bootsAnimator.SetFloat("Horizontal", targetVelocity.x);             // Use when equipment is being added
        //bootsAnimator.SetFloat("Vertical", targetVelocity.y);               // Use when equipment is being added
        //bootsAnimator.SetFloat("Speed", targetVelocity.sqrMagnitude/* * bootsSpeedGang*/);      // Use when equipment is being added

        base.Update();
    }
}
