using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private GameObject enemyGameObject;
    public GameObject playerObject;
    //private GameObject playersAttackColliderObject;       // Not used???
    public bool isEnemyAttackable = true;
    private SpriteRenderer enemySpriteRenderer;
    private AudioSource hurtClip;

    
    void Start()
    {
        enemyGameObject = this.gameObject;
        //playersAttackColliderObject = playerObject.transform.Find("AttackCollider").gameObject;       // Not used???
        enemySpriteRenderer = enemyGameObject.GetComponent<SpriteRenderer>();
        hurtClip = enemyGameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the players attack collider is what triggers the enemy
        if (collision.gameObject.tag == "MyPlayerAttackCollider" && isEnemyAttackable == true)
        {
            StartCoroutine(KnockbackEnemy(collision));
        }       
    }

    private IEnumerator BlinkWhenHit()
    {
        // This blinks the enemy for 0.2 seconds when its being hit
        enemySpriteRenderer.enabled = false;
        //Debug.Log("BLINKED");
        yield return new WaitForSeconds(0.08f);
        enemySpriteRenderer.enabled = true;
    }
    private IEnumerator KnockbackEnemy(Collider2D collision)
    {
        // Play hurt sound
        if (hurtClip != null)
        {
            hurtClip.Play();
        }
        
        //Debug.Log("trigger enter2d on enemy fired");

        // Damage the enemy 20 HP
        enemyGameObject.GetComponent<EnemyHealth>().TakeDamage(20);

        // Check if the enemys health is larger than zero. Do the regular knockback
        if(enemyGameObject.GetComponent<EnemyHealth>().enemyHealth > 0)
        {
            isEnemyAttackable = false;
            StartCoroutine(BlinkWhenHit());
            // Set movementscript to false so we can push the enemy back without issues
            enemyGameObject.GetComponent<EnemyMovement>().enabled = false;
            // This is the actual knockback functionality, might need some adjustments later but for now it looks good
            Vector2 difference = transform.position - collision.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y +difference.y);

        
            // Wait for 1 second before enabling the enemys movement again
            yield return new WaitForSeconds(1);
        
            // Enable enemy movement again
            enemyGameObject.GetComponent<EnemyMovement>().enabled = true;
            isEnemyAttackable = true;
        }
        // Else if we have put the enemy to 0 hp or below we dont want the
        // regular knockback which fiddles with enabling the movementscript too fast
        // here we just want to blink the enemy and knock it bnack!
        else
        {
            StartCoroutine(BlinkWhenHit());
            // This is the actual knockback functionality, might need some adjustments later but for now it looks good
            Vector2 difference = transform.position - collision.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
        }
    }
}
