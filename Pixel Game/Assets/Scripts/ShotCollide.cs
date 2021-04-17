using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotCollide : MonoBehaviour
{
    private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MyPlayer");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "damageCollider")
        {
            player.GetComponent<PlayerHealth>().TakeDamage(20);
        }
    }

}
