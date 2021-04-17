using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacked : MonoBehaviour
{
    public bool isAttackable = true;
    private SpriteRenderer spriteRenderer;
    private AudioSource hurtClip;
    private Rigidbody2D rigidBody;

    
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        hurtClip = this.gameObject.GetComponent<AudioSource>();
        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if an attack collider is what triggers the enemy
        if (collision.gameObject.tag == "MyPlayerAttackCollider" && isAttackable == true)
        {
            StartCoroutine(Knockback(collision));
        }       
    }

    private IEnumerator BlinkWhenHit()
    {
        // This blinks the enemy for 0.2 seconds when its being hit
        spriteRenderer.enabled = false;
        //Debug.Log("BLINKED");
        yield return new WaitForSeconds(0.08f);
        spriteRenderer.enabled = true;
    }
    private IEnumerator Knockback(Collider2D collision)
    {
        // Play hurt sound
        if (hurtClip != null)
        {
            hurtClip.Play();
        }

        //Debug.Log("trigger enter2d on enemy fired");

        // Damage the enemy 20 HP
        this.gameObject.GetComponent<EnemyHealth>().TakeDamage(20);

        // Check if the enemys health is larger than zero. Do the regular knockback
        if(this.gameObject.GetComponent<EnemyHealth>().enemyHealth > 0)
        {
            isAttackable = false;
            StartCoroutine(BlinkWhenHit());
            // Set movementscript to false so we can push the enemy back without issues
            this.gameObject.GetComponent<EnemyMovement>().enabled = false;
            // This is the actual knockback functionality, might need some adjustments later but for now it looks good
            Vector2 difference = transform.position - collision.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y +difference.y);
        
            // Wait for 1 second before enabling the enemys movement again
            yield return new WaitForSeconds(1);

            // Enable enemy movement again
            this.gameObject.GetComponent<EnemyMovement>().enabled = true;
            isAttackable = true;
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
