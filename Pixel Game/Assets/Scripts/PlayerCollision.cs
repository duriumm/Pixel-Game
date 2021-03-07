using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameObject myPlayer;

    void Start()
    {
        myPlayer = this.gameObject;       
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // PROBLEM
        // This acts as a collider for the whole player object
        // When i am activating the attackCollider it blends in with this collider
        // and thereby making the player get damaged by the enemy

        // this seems to have fixed it with checking for attackCollider to be false :)
        if (collision.gameObject.tag == "Enemy") // && player.enumstate != attacking.state - TODO
        {
            myPlayer.GetComponent<PlayerHealth>().TakeDamage(20);
            //Debug.Log("Taken damage");
            Debug.Log("Current hp: " + myPlayer.GetComponent<PlayerHealth>().playerHealth);
        }
        else if(collision.gameObject.tag == "LootableItem")
        {
            //if ()
            //{
            //    Debug.Log("collided with lootable plant");
            //}
            
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collisionenter2d on PLAYER fired");
    }
}
