using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScreen : GuiScreen
{
    public InventorySlot[] shopSlots;
    public List<GameObject> ItemList = new List<GameObject>();
    void Start()
    {
        shopSlots = this.gameObject.transform.GetComponentsInChildren<InventorySlot>();

        // Set all shop slot sprites to alpha color 0, otherwise we will see a white box instead of empty item slot
        foreach (var item in shopSlots)
        {
            if (item.slotIcon.sprite == null) // try to: check sprite null, check Alpha null, check obj in slot null
            {
                item.SetAlphaOfColor(0f);
            }
        }
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        base.Close();
    }

    public override void Open()
    {
        // Clear all data flor shop slots so we can add new data!
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].ClearSlot();
        }
        
        // Populate shop screens slots with copies of prefab items
        for (int i = 0; i < ItemList.Count; i++)
        {
            // We add the original gameobject prefab here into the shop slot since we instantiate the object on buying items anyway!
            shopSlots[i].AddItem(ItemList[i]); 
        }
        base.Open();
    }
}
