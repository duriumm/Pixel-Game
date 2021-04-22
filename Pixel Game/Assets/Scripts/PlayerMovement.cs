using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    void Update()
    {
        // Input for movement on both axis
        movementDir.x = Input.GetAxisRaw("Horizontal");
        movementDir.y = Input.GetAxisRaw("Vertical");

        // Animate character based on movement
        animator.SetFloat("Horizontal", movementDir.x);
        animator.SetFloat("Vertical", movementDir.y);

        // Save the last move position of the character so we can load the Idle animation correctly based on last move position
        if (movementDir != Vector2.zero)
        {
            animator.SetFloat("LastMoveX", movementDir.x);
            animator.SetFloat("LastMoveY", movementDir.y);
        }

        // Dont delete this, it will be used for equipment later on
        //bootsAnimator.SetFloat("Horizontal", targetVelocity.x);             // Use when equipment is being added
        //bootsAnimator.SetFloat("Vertical", targetVelocity.y);               // Use when equipment is being added

        animator.SetFloat("Speed", movementDir.sqrMagnitude);
        //bootsAnimator.SetFloat("Speed", targetVelocity.sqrMagnitude/* * bootsSpeedGang*/);      // Use when equipment is being added
    }
}
