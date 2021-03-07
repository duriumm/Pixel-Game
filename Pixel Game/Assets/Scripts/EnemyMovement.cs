using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject playerObject;
    public float speed = 0.5f;

    public Animator enemyAnimator;
    public GameObject mainCamera;

    private Transform playerTransform;
    private Transform ghastTransform;

    Vector2 ghastPositionDifferenceToPlayer;

    float ghast_X_ValueForAnimator;
    float ghast_Y_ValueForAnimator;

    public AudioClip ghastAmbientSound;

    private bool isPlayingAmbientGhastSound = false;

    // Start is called before the first frame update
    void Start()
    {
        // TO-DO
        // Fix a nicer way of getting the player transform position
        // TO-DO
        playerTransform = GameObject.FindGameObjectWithTag("MyPlayer").GetComponent<Transform>();
        ghastTransform = gameObject.transform;
    }

    void Update()
    {
        // TO-DO
        // Make a state machine for the ghast for 1 roaming around,
        // 2 chasing player, 3 dying etc etc
        // TO-DO

        // If ghast is close to player he starts moving towards the player and 
        // ghast position difference to the player is saved to use it in 
        // animator controller easier
        if (Vector2.Distance(ghastTransform.position, playerTransform.position) > 1.5f)
        {
            // Plays ambient sound of ghast movement and waits 9 seconds to play it again
            // TO-DO
            // Play ambient when player is in range of 4 - 1.6f only
            // TO-DO
            if(isPlayingAmbientGhastSound == false)
            {
                StartCoroutine(waitForAmbientSoundToPlay());
            }



            ghastTransform.position = Vector2.MoveTowards(ghastTransform.position, playerTransform.position, speed * Time.deltaTime);
            
            ghastPositionDifferenceToPlayer = ghastTransform.position - playerTransform.position;
            //Debug.Log("Distance difference ghast and player: " + ghastPositionDifferenceToPlayer);
        }

        // Why we make this small calculation is because otherwise the ghast will look in the wrong direction
        // when chasing the player, looking right when the player is left of the ghast.
        ghast_X_ValueForAnimator = ghastPositionDifferenceToPlayer.x - (ghastPositionDifferenceToPlayer.x * 2);
        ghast_Y_ValueForAnimator = ghastPositionDifferenceToPlayer.y - (ghastPositionDifferenceToPlayer.y * 2);

        enemyAnimator.SetFloat("Horizontal", ghast_X_ValueForAnimator);
        enemyAnimator.SetFloat("Vertical", ghast_Y_ValueForAnimator);

        enemyAnimator.SetFloat("Speed", speed);       
    }
    private IEnumerator waitForAmbientSoundToPlay()
    {
        isPlayingAmbientGhastSound = true;
        AudioSource.PlayClipAtPoint(ghastAmbientSound, mainCamera.transform.position);        
        yield return new WaitForSeconds(9);
        isPlayingAmbientGhastSound = false;
    }
}
