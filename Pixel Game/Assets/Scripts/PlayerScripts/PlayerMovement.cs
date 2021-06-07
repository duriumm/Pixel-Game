using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{

    protected override void Update()
    {
        // Input for movement on both axis
        MovementDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            );
        if (MovementDir != Vector2.zero)
            FaceDir = MovementDir;
        base.Update();
    }
}
