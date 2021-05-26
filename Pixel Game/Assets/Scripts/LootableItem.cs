using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableItem : MonoBehaviour
{
    private GameObject inventoryManager;
    private GameObject playerColliderObject;
    void Start()
    {
        inventoryManager = GameObject.FindWithTag("InventoryManager");
        playerColliderObject = GameObject.Find("MyCharacter");
        playerColliderObject = playerColliderObject.transform.Find("AttackColliders/DamageCollider").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == playerColliderObject)
        {
            inventoryManager.GetComponent<PlayerInventory>().LootItem(this.gameObject/*.GetComponent<ItemData>()*/);
            //Debug.Log("Picked up something");
        }
    }
}
