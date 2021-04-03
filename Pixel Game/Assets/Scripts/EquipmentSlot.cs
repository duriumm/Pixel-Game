using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentSlot : InventorySlot
{
    private static PlayerInventory inventory;
    new void Awake()
    {
        if (inventory == null)
        {
            var inventoryManager = GameObject.FindWithTag("InventoryManager");
            inventory = inventoryManager.GetComponent<PlayerInventory>();
        }
        base.Awake();
    }

    public void UseItem()
    {
        inventory.addItemToSlot(ItemDataInSlot.gameObject);
    }
}
