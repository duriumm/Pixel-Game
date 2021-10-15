using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : Movement
{
    // Tried making a overridden Start() function but did not manage, therefor 
    // i use a public audioManager dragged in from the hirearchy
    public AudioManager audioManager;

    
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

    // Playing from here since the playing script need to be attached to the player
    // for the sound to be able to play in animation keyframe
    public void PlayerFootstepSound()
    {
        audioManager.PlaySound();
        
    }

    // Triggering of the footstep collider is done 2 times per player step
    // twice is because we want to check groundtype more often
    public void TriggerFootstepCollider()
    {
        audioManager.TurnOnThenOffAudioFootstepCollider();
    }


    
}
