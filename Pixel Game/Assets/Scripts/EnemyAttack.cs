using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public GameObject particleAttackPrefab;
    public GameObject mainCamera;
    public float particleMovementSpeed = 2f;


    private GameObject playerGameObject;
    private GameObject particleAttackObject;
    private Vector2 enemyPosition;
    private Vector3 playerPositionWhenAttacking;

    
    private Vector3 movementVector = Vector3.zero;

    private bool isEnemyAttacking = false;

    // Implement this next !!
    //public AudioClip preAttackSound;
    public AudioClip attackSound;


    // Start is called before the first frame update
    void Start()
    {
        enemyPosition = gameObject.transform.position;
        // TO-DO
        // Fix a nicer way of getting the player gameobject?
        // TO-DO
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemyAttacking == false)
        {
            SpawnParticleAttack();
            StartCoroutine(WaitForNextAttack());
        }
        else if(isEnemyAttacking == true)
        {
            ShootParticleAttack();
        }
        
    }

    private void SpawnParticleAttack()
    {
        if(particleAttackObject != null)
        {
            // TO-DO
            // Here i would like to have object pooling to use the same 1 ,2 or 3 attack objects over and over
            // TO-DO
            Destroy(particleAttackObject);
        }
        // Spawn the particle effect gameobject from a prefab on top of the ghast enemy
        particleAttackObject = Instantiate(particleAttackPrefab) as GameObject;
        // Get the CURRENT updated enemy position and then set our particle attack object to enemys position
        enemyPosition = gameObject.transform.position;
        particleAttackObject.transform.position = enemyPosition;
        // we change the Z axis since otherwise the particle effect doesnt play correctly 
        particleAttackObject.transform.position = new Vector3(particleAttackObject.transform.position.x, particleAttackObject.transform.position.y, -1);

        // Get where the player was standing at this exact moment and then
        // set the z value to -1 otherwise we use vector2 which automatically 
        // sets z value to 0 which will cause flickering of particle object
        playerPositionWhenAttacking = playerGameObject.transform.position;
        playerPositionWhenAttacking.z = -1;

        // Calculation of positions to make the particle attack object move PAST the player object when shooting
        movementVector = (playerPositionWhenAttacking - particleAttackObject.transform.position).normalized * particleMovementSpeed;

        // Play the attack sound at the player position
        AudioSource.PlayClipAtPoint(attackSound, mainCamera.transform.position);
        isEnemyAttacking = true;


    }
    
    private void ShootParticleAttack()
    {
        // Shoots away the particle attack towards the player by adding movementVector * deltatime
        // to the partiucle attack object. This makes it so particle keeps flying past the player
        particleAttackObject.transform.position += movementVector * Time.deltaTime;
    }

    IEnumerator WaitForNextAttack()
    {
        
        yield return new WaitForSeconds(2);
        
        isEnemyAttacking = false;
    }

    public void DestroyParticleAttackObject()
    {
        Destroy(particleAttackObject);
    }
    // Implement this next to make a pre attack sound
    //IEnumerator GeneralWaitForSeconds(int secondsToWait)
    //{
    //    AudioSource.PlayClipAtPoint(preAttackSound, mainCamera.transform.position);
    //    yield return new WaitForSeconds(secondsToWait);

    //}
}
