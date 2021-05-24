using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableItem : MonoBehaviour
{
    private GameObject inventoryManager;
    void Start()
    {
        inventoryManager = GameObject.FindWithTag("InventoryManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            inventoryManager.GetComponent<PlayerInventory>().LootItem(this.gameObject/*.GetComponent<ItemData>()*/);
            //Debug.Log("Picked up something");
        }
    }
}
