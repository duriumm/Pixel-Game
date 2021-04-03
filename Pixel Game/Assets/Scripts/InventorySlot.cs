using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;


/* Inventory slot script is used on each inventory slot of the player AND on each shop screens slot.
   Players inventory slot is using Remove() func on buttonpress while shop slot is using BuyItemFromShop()
   on button press
*/

public class InventorySlot : MonoBehaviour
{
    public Image slotIcon;
    private GameObject playerCharacter;
    private Color ColorOfSlot;
    private Vector2 itemDropPosition;

    private ItemData ItemDataInSlot;
    public GameObject ItemDataGameObject => ItemDataInSlot?.gameObject;
    private GameObject shopScreen;

    private EventTrigger eventTrigger;

    private GameObject dropItemButton;
    // Enable this bool in inspector for all inventory slots but disable for all shop slots
    public bool isInventoryPanel;
    private static PlayerInventory inventory;

    protected void Awake()
    {
        if (inventory == null)
        {
            var inventoryManager = GameObject.FindWithTag("InventoryManager");
            inventory = inventoryManager.GetComponent<PlayerInventory>();
        }
        if (isInventoryPanel)
        {
            // Find the DropButton inside the slotpanel only if its inventory panel slot
            dropItemButton = gameObject.transform.GetChild(1).gameObject;
            dropItemButton.SetActive(false);
        }
        Debug.Log("object we foudn was: " + dropItemButton);
        eventTrigger = this.gameObject.GetComponent<EventTrigger>();
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        slotIcon = this.gameObject.GetComponent<Image>();
        shopScreen = GameObject.Find("ShopScreen");
    }
    public void Start()
    {

    }

    public void UseItem()
    {
        if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false && 
            ItemDataInSlot != null && inventory.isInventoryOpen == true)
        {
            if(ItemDataInSlot.isEquippable)
            {
                EquipItem();
            }
            else if(ItemDataInSlot.itemType == ItemData.ITEMTYPE.EDIBLE)
            {
                Debug.Log("Ate food, we gained: "+ItemDataInSlot.healingCapability+" health");
                playerCharacter.GetComponent<PlayerHealth>().GainHealth(ItemDataInSlot.healingCapability);

                Destroy(ItemDataGameObject);
                ClearAllDataFromSlot();
            }
            else if(ItemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            {
                Debug.Log("Equipped Weapon");
            }
            else if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.ARMOR)
            {
                Debug.Log("Equipped Armor");
            }
        }
        else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            RemoveItem();
        }
    }

    void EquipItem()
    {
        //Add item to equipment slot
        var equipmentSlotName = Enum.GetName(typeof(ItemData.ITEMTYPE), ItemDataInSlot.itemType) + "_SlotPanel";
        var equipmentSlotGameObject = GameObject.Find(equipmentSlotName);
        var equipmentSlotMono = equipmentSlotGameObject.GetComponent<InventorySlot>();
        equipmentSlotMono.AddItem(ItemDataInSlot.gameObject);
        ClearAllDataFromSlot();
    }

    public void UnequipItem()
    {
        inventory.addItemToSlot(ItemDataInSlot.gameObject);
        ClearAllDataFromSlot();
    }

    public void ClearAllDataFromSlot()
    {
        ItemDataInSlot = null;
        slotIcon.sprite = null;
        SetAlphaOfColor(0f);
        // Here we check if dropItemButton is null or not, since we use same invSlot script for shops slots
        // Since it should only apply for the inventory slots, we only enter if statement if this slot is an inventory slot
        if (dropItemButton != null)
        {
            dropItemButton.SetActive(false);
        }
    }

    public void AddItem(GameObject gameObjectToAdd)

    {   // Only inventory slot panels has the dropItemButton therefor we dont touch it for shop slots
        if (isInventoryPanel)
        {
            dropItemButton.SetActive(true);
        }
        // BUG
        // Sometimes when picking up item, the item get duplicated. 
        // This is probably because the picked up item gets instantiated
        // and the original doesnt get destroyed...
        // BUG

        ItemDataInSlot = gameObjectToAdd.GetComponent<ItemData>();
        slotIcon.sprite = ItemDataInSlot.itemIcon; 
        
        // Set alpha of slot to 1 so we can see the item sprite
        SetAlphaOfColor(1f);
        
        //ItemDataGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
    }
    public void BuyItemFromShop()
    {
        // If there is an actual item in current slot and inventory is actually open
        if (ItemDataInSlot != null && inventory.isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            if(inventory.playerInvMoney >= ItemDataInSlot.value)
            {
                inventory.BuyAndSellItemSound();
                string originalItemName = ItemDataGameObject.name;
                GameObject instantiatedGameObject = Instantiate(ItemDataGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                inventory.addItemToSlot(instantiatedGameObject);

                inventory.RemoveCoinAmount(ItemDataInSlot.value);
                Debug.Log("trying to buy");
            }
            else
            {
                //TO-DO - Play a nuh-uh! Sound from the npc
                Debug.Log("Sorry you do not have enough money...");
            }
        }
    }

    public void RemoveItem()
    {
        // Only inventory slot panels has the dropItemButton therefor we dont touch it for shop slots
        if (isInventoryPanel)
        {
            dropItemButton.SetActive(false);
        }

        Debug.Log("You pressed button");

        Debug.Log("Is shot screen open: " + shopScreen.GetComponent<ShopScreen>().isShopScreenOpen);
        // If there is an actual item in current slot and inventory is actually open
        if (ItemDataInSlot != null && inventory.isInventoryOpen == true)
        {
            if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
            {
                // TO-DO
                // Set spawning of item position further away from player so loot 
                // doesnt get picked up instantly again by the player. 
                // TO-DO

                string realItemName = ItemDataInSlot.name; // Get the original name of the gameobject
                GameObject instantiatedGameObject = Instantiate(ItemDataGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = realItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                // Set spawn position of item to where the player is standing currently
                // TO-DO - Spawn the item in a cricle around the player of random numbers
                itemDropPosition.x = playerCharacter.transform.position.x + 1f;
                itemDropPosition.y = playerCharacter.transform.position.y + 1f;
                instantiatedGameObject.transform.position = itemDropPosition;
                instantiatedGameObject.SetActive(true);
                Destroy(ItemDataGameObject);

                // Set alpha of slot to 0 and assign the items to the slot.
                SetAlphaOfColor(0f);
                ItemDataInSlot = null;
                slotIcon.sprite = null; 
                Debug.Log("Removed inventory item from GUI");
                instantiatedGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
            }
            // If the shops item screen is open, we will sell our inventory slot item instead of dropping it on the ground
            else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
            {
                inventory.BuyAndSellItemSound();
                inventory.AddCoinAmount(ItemDataGameObject.GetComponent<ItemData>().value);
                Destroy(ItemDataGameObject);
                SetAlphaOfColor(0f);
                ItemDataInSlot = null;
                slotIcon.sprite = null;
            }
        }
    }

    public void SetAlphaOfColor(float alphaColorValue)
    { 
        ColorOfSlot = slotIcon.color;
        ColorOfSlot.a = alphaColorValue;
        slotIcon.color = ColorOfSlot;
    }

    public void ShowDataOnHover()
    {
        if(ItemDataGameObject != null)
        {

            // TO-DO - This might need to be optimized for future. Maybe assign the text game objects in the inspector beforehand?
            TextMeshProUGUI itemNameText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemNameText").gameObject.GetComponent<TextMeshProUGUI>();
            itemNameText.text = ItemDataInSlot.itemName;
            TextMeshProUGUI ItemDescriptionText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemDescriptionText").gameObject.GetComponent<TextMeshProUGUI>();
            ItemDescriptionText.text = ItemDataInSlot.description;
            TextMeshProUGUI ItemStatsText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemStatsText").gameObject.GetComponent<TextMeshProUGUI>();
            if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            {
                ItemStatsText.text = "Damage: "+ItemDataInSlot.damage+"\n"+ "Value: <color=yellow>" + ItemDataInSlot.value + " coins</color> ";
            }
            else if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.HELMET || ItemDataInSlot.itemType == ItemData.ITEMTYPE.ARMOR)
            {
                ItemStatsText.text = "Armor: " + ItemDataInSlot.defense + "Value: <color=yellow>" + ItemDataInSlot.value + " coins</color> ";
            }
            else if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.EDIBLE)
            {   // Show the text in green to indicate hp gain on eating item
                ItemStatsText.text = "Effect on eating: " + "<color=green>+" + ItemDataInSlot.healingCapability + " hp</color>";
            }




            this.gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            Debug.Log("no item in slot :(");

        }

        // TO-DO - Enable a tooltip box that shows data of the item of THIS current slot
    }
    public void RemoveDataShowingOnExit()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    

}
