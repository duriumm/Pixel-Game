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
        base.Update();
    }
}
