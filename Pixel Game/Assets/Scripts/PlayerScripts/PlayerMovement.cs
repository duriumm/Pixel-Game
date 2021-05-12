using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{

    protected override void Update()
    {
        // Input for movement on both axis
        movementDir.x = Input.GetAxisRaw("Horizontal");
        movementDir.y = Input.GetAxisRaw("Vertical");
        if (movementDir != Vector2.zero)
            faceDir = movementDir;
        base.Update();
    }
}
