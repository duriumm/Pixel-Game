﻿using System.Collections;
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
    public Image slotIcon { get; private set; }
    private GameObject playerCharacter;
    private Color ColorOfSlot;
    private Vector2 itemDropPosition;

    private ItemData itemDataInSlot; // Changed from private to public. All good?

    // "Get" function for returning private itemDataInSlot variable
    public ItemData ItemDataInSlot => itemDataInSlot;

    public GameObject ItemDataGameObject => itemDataInSlot?.gameObject;
    public bool IsEmpty => itemDataInSlot == null;
    private GameObject shopScreen;
    private EventTrigger eventTrigger;
    private GameObject dropItemButton;
    // Enable this bool in inspector for all inventory slots but disable for all shop slots
    public bool isInventoryPanel;
    private static PlayerInventory inventory;
    private Attack playerAttack;
    private DataToPassBetweenScenes dataToPass;
    private PlayerHealth playerHealth;

    void Awake()
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
        //Debug.Log("object we foudn was: " + dropItemButton);
        eventTrigger = this.gameObject.GetComponent<EventTrigger>();
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        playerAttack = playerCharacter.GetComponent<Attack>();
        playerHealth = playerCharacter.GetComponent<PlayerHealth>();
        slotIcon = this.gameObject.GetComponent<Image>();
        shopScreen = GameObject.Find("ShopScreen");
        
    }
    private void Start()
    {
        dataToPass = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
    }

    public void UseItem()
    {
        if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false && 
            itemDataInSlot != null && inventory.isInventoryOpen == true)
        {
            if(itemDataInSlot.isEquippable)
            {
                StartCoroutine(EquipItem());
            }
            else if(itemDataInSlot.itemType == ItemData.ITEMTYPE.EDIBLE)
            {
                if (!playerHealth.HasFullHp)
                {
                    Debug.Log("Ate food, we gained: " + itemDataInSlot.healingCapability + " health");
                    playerHealth.Hp += itemDataInSlot.healingCapability;
                    Destroy(ItemDataGameObject);
                    ClearSlot();
                }
            }
            // Using a Quest item when in range of the target delivery npc will mark the quest as finished
            // and destroy said quest item
            else if(itemDataInSlot.itemType == ItemData.ITEMTYPE.QUEST_ITEM)
            {
                if (dataToPass.currentActivateNpc == dataToPass.currentActivePlayerQuest.npcDeliveryTarget.name)
                {
                    Destroy(ItemDataGameObject);
                    ClearSlot();
                    dataToPass.currentActivePlayerQuest.isQuestFinished = true;
                }
            }
        }
        else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            DropItem();
        }
    }

    public IEnumerator EquipItem()
    {
        //Add item to equipment slot
        Debug.Log("we came IN HERE EQUIP");

        //Make sure the item has been initialized
        ItemDataGameObject.SetActive(true);
        yield return null;
        ItemDataGameObject.SetActive(false);

        if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
        {
            var weapon = ItemDataGameObject.GetComponent<Weapon>();
            playerAttack.EquipWeapon(weapon);
        }

        var equipmentSlot = GetEquipmentSlotForItemType();
        equipmentSlot.UnequipItem();
        playerHealth.Armor += itemDataInSlot.defense;
        equipmentSlot.AddItem(ItemDataGameObject, this);
    }

    InventorySlot GetEquipmentSlotForItemType()
    {
        string equipmentSlotName = GetEquipmentSlotNameForItemType();
        var equipmentSlotGameObject = GameObject.Find(equipmentSlotName);
        return equipmentSlotGameObject.GetComponent<InventorySlot>();
    }

    string GetEquipmentSlotNameForItemType()
    {
        return Enum.GetName(typeof(ItemData.ITEMTYPE), itemDataInSlot.itemType) + "_SlotPanel";
    }

    public void MoveEquippedItemToInventory()
    {
        if (itemDataInSlot == null)
            return;
        //Only unequip if the inventory has an empty slot to put the item in
        if (inventory.AddItemToEmptySlot(ItemDataGameObject))
        {
            UnequipItem();
            ClearSlot();
        }
    }

    public void DropEquippedItem()
    {
        UnequipItem();
        DropItem();
    }

    private void UnequipItem()
    {
        if (itemDataInSlot == null)
            return;

        if (itemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            playerAttack.EquipWeapon(null);
        playerHealth.Armor -= ItemDataInSlot.defense;
    }

    public void ClearSlot()
    {
        itemDataInSlot = null;
        slotIcon.sprite = null;
        SetAlphaOfColor(0f);
        // Here we check if dropItemButton is null or not, since we use same invSlot script for shops slots
        // Since it should only apply for the inventory slots, we only enter if statement if this slot is an inventory slot
        if (dropItemButton != null)
        {
            dropItemButton.SetActive(false);
        }
        RemoveDataShowingOnExit();
    }

    public void AddItem(GameObject itemToAdd, InventorySlot sourceSlot = null)
    {
        if (itemToAdd == null)
            throw new ArgumentNullException("itemToAdd");
        // Only inventory slot panels has the dropItemButton therefor we dont touch it for shop slots
        if (isInventoryPanel)
        {
            dropItemButton.SetActive(true);
        }

        var tempItem = ItemDataGameObject;
        itemDataInSlot = itemToAdd.GetComponent<ItemData>();
        slotIcon.sprite = itemDataInSlot.itemIcon;

        //If sourceSlot is not null it will be cleared or get the item of this slot if not null
        if (sourceSlot != null)
        {
            sourceSlot.ClearSlot();
            if (tempItem != null)
            {
                sourceSlot.AddItem(tempItem.gameObject);
                sourceSlot.ShowDataOnHover();
            }
        }
        // Set alpha of slot to 1 so we can see the item sprite
        SetAlphaOfColor(1f);
    }

    public void BuyItemFromShop()
    {
        // If there is an actual item in current slot and inventory is actually open
        if (itemDataInSlot != null && inventory.isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            if(inventory.playerInvMoney >= itemDataInSlot.value)
            {
                inventory.BuyAndSellItemSound();
                string originalItemName = ItemDataGameObject.name;
                GameObject instantiatedGameObject = Instantiate(ItemDataGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                inventory.AddItemToEmptySlot(instantiatedGameObject);

                inventory.RemoveCoinAmount(itemDataInSlot.value);
                Debug.Log("trying to buy");
            }
            else
            {
                //TO-DO - Play a nuh-uh! Sound from the npc
                Debug.Log("Sorry you do not have enough money...");
            }
        }
    }

    public void DropItem()
    {
        if(dataToPass.currentActivePlayerQuest.questType == Quest.QUESTTYPE.GATHER_ITEMS)
        {
            if (itemDataInSlot.itemName ==
                dataToPass.currentActivePlayerQuest.itemToGather.GetComponent<ItemData>().itemName)
            {
                dataToPass.currentActivePlayerQuest.DecrementItemsCollected();
                Debug.Log("WE DECREMENTED FFS");
            }
            
        }
        // Only inventory slot panels has the dropItemButton therefor we dont touch it for shop slots
        if (isInventoryPanel)
        {
            dropItemButton.SetActive(false);
        }

        Debug.Log("You pressed button");

        Debug.Log("Is shot screen open: " + shopScreen.GetComponent<ShopScreen>().isShopScreenOpen);
        // If there is an actual item in current slot and inventory is actually open
        if (itemDataInSlot != null && inventory.isInventoryOpen == true)
        {
            if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
            {
                // Set spawn position of item to where the player is standing currently
                // TO-DO - Spawn the item in a cricle around the player of random numbers
                itemDropPosition.x = playerCharacter.transform.position.x + 1f;
                itemDropPosition.y = playerCharacter.transform.position.y + 1f;
                ItemDataGameObject.transform.position = itemDropPosition;
                ItemDataGameObject.SetActive(true);
                ClearSlot();
                Debug.Log("Removed inventory item from GUI");
                //instantiatedGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
            }
            // If the shops item screen is open, we will sell our inventory slot item instead of dropping it on the ground
            else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
            {
                inventory.BuyAndSellItemSound();
                inventory.AddCoinAmount(ItemDataGameObject.GetComponent<ItemData>().value);
                Destroy(ItemDataGameObject);
                ClearSlot();
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
            itemNameText.text = itemDataInSlot.itemName;
            TextMeshProUGUI ItemDescriptionText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemDescriptionText").gameObject.GetComponent<TextMeshProUGUI>();
            ItemDescriptionText.text = itemDataInSlot.description;
            TextMeshProUGUI ItemStatsText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemStatsText").gameObject.GetComponent<TextMeshProUGUI>();
            if (itemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            {
                ShowWeaponStats(ItemStatsText);
            }
            else if (itemDataInSlot.itemType == ItemData.ITEMTYPE.HELMET || itemDataInSlot.itemType == ItemData.ITEMTYPE.ARMOR)
            {
                ItemStatsText.text = "Armor: " + itemDataInSlot.defense + "\n" + "Value: <color=yellow>" + itemDataInSlot.value + " coins</color> ";
            }
            else if (itemDataInSlot.itemType == ItemData.ITEMTYPE.EDIBLE)
            {   // Show the text in green to indicate hp gain on eating item
                ItemStatsText.text = "Effect on eating: " + "<color=green>+" + itemDataInSlot.healingCapability + " hp</color>";
            }
            else if(itemDataInSlot.itemType == ItemData.ITEMTYPE.QUEST_ITEM)
            {
                ItemStatsText.text = null;
            }
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            Debug.Log("no item in slot :(");

        }

        // TO-DO - Enable a tooltip box that shows data of the item of THIS current slot
    }

    private void ShowWeaponStats(TextMeshProUGUI itemStatsText)
    {
        var weapon = ItemDataGameObject.GetComponent<Weapon>();
        string powerDiff = "", cooldownDiff = "", projectileSpeedDiff = "";
        if (gameObject.name != GetEquipmentSlotNameForItemType())
        {
            powerDiff = GetHoverDiffText(weapon.Damage - playerAttack.CurrentWeapon.Damage);
            cooldownDiff = GetHoverDiffText(weapon.Cooldown - playerAttack.CurrentWeapon.Cooldown, true);
            if (weapon.HasProjectileAttack && playerAttack.CurrentWeapon.HasProjectileAttack)
                projectileSpeedDiff = GetHoverDiffText(weapon.ProjectileAttack.Speed - playerAttack.CurrentWeapon.ProjectileAttack.Speed);
        }
        itemStatsText.text = $"Damage: {weapon.Damage} {powerDiff}\n";
        itemStatsText.text += $"Cooldown: {weapon.Cooldown}s {cooldownDiff}\n";
        if (weapon.HasProjectileAttack)
            itemStatsText.text += $"Projectile speed: {weapon.ProjectileAttack.Speed} {projectileSpeedDiff}\n";
        itemStatsText.text += $"Value: <color=yellow>{itemDataInSlot.value} coins</color> ";
    }

    // Returns a string describing the difference between the hovered item
    // and the currently equipped item
    // For example, if we have equipped a sword with power 10,
    // and the weapon we are hovering on has power 15, the text will be "+5" in green
    private string GetHoverDiffText(float diff, bool reverseColor = false)
    {
        string positive = "green";
        string negative = "#ff7070";
        string neutral = "white";
        string color;
        string sign;
        if (diff > 0)
        {
            color = reverseColor ? negative : positive;
            sign = "+";
        }
        else if (diff == 0)
        {
            color = neutral;
            sign = "+/-";
        }
        else
        {
            color = reverseColor ? positive : negative;
            sign = "-";
        }
        return $"<color={color}> {sign}{Math.Abs(diff)}</color>";
    }

    public void RemoveDataShowingOnExit()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    

}
