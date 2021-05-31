using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableItem : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private GameObject vacuumColliderObject;
    private GameObject pickUpColliderObject;
    void Start()
    {
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
        var player = GameObject.Find("MyCharacter");
        vacuumColliderObject = player.transform.Find("InteractionCollider").gameObject;
        pickUpColliderObject = player; //Player's feet
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == pickUpColliderObject)
        {
            playerInventory.LootItem(this.gameObject);
            //Debug.Log("Picked up something");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == vacuumColliderObject)
        {
            var direction = collision.gameObject.transform.position - transform.position;
            transform.position += direction.normalized * 0.1f;
        }
    }
}
