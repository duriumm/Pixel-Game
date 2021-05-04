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
    public Image slotIcon { get; private set; }
    private GameObject playerCharacter;
    private Color ColorOfSlot;
    private Vector2 itemDropPosition;
    private ItemData ItemDataInSlot;
    public GameObject ItemDataGameObject => ItemDataInSlot?.gameObject;
    public bool IsEmpty => ItemDataInSlot == null;
    private GameObject shopScreen;
    private EventTrigger eventTrigger;
    private GameObject dropItemButton;
    // Enable this bool in inspector for all inventory slots but disable for all shop slots
    public bool isInventoryPanel;
    private static PlayerInventory inventory;
    private Attack playerAttack;

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
        slotIcon = this.gameObject.GetComponent<Image>();
        shopScreen = GameObject.Find("ShopScreen");
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
                if (!playerCharacter.GetComponent<PlayerHealth>().HasFullHp)
                {
                    Debug.Log("Ate food, we gained: " + ItemDataInSlot.healingCapability + " health");
                    playerCharacter.GetComponent<PlayerHealth>().GainHealth(ItemDataInSlot.healingCapability);

                    Destroy(ItemDataGameObject);
                    ClearSlot();
                }
            }
        }
        else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            DropItem();
        }
    }

    void EquipItem()
    {
        //Add item to equipment slot
        
        var equipmentSlot = GetEquipmentSlotForItem();

        // If weapon, set as player's equipped weapon
        var weapon = ItemDataGameObject.GetComponent<Weapon>();
        if (weapon != null)
            playerAttack.EquipWeapon(weapon);

        equipmentSlot.AddItem(ItemDataGameObject, this);
    }

    InventorySlot GetEquipmentSlotForItem()
    {
        string equipmentSlotName = GetEquipmentSlotNameForItem();
        var equipmentSlotGameObject = GameObject.Find(equipmentSlotName);
        return equipmentSlotGameObject.GetComponent<InventorySlot>();
    }

    string GetEquipmentSlotNameForItem()
    {
        return Enum.GetName(typeof(ItemData.ITEMTYPE), ItemDataInSlot.itemType) + "_SlotPanel";
    }

    public void MoveEquippedItemToInventory()
    {
        if (ItemDataInSlot == null)
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
        if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            playerAttack.EquipWeapon(null);
    }

    public void ClearSlot()
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
        ItemDataInSlot = itemToAdd.GetComponent<ItemData>();
        slotIcon.sprite = ItemDataInSlot.itemIcon;

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
        if (ItemDataInSlot != null && inventory.isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            if(inventory.playerInvMoney >= ItemDataInSlot.value)
            {
                inventory.BuyAndSellItemSound();
                string originalItemName = ItemDataGameObject.name;
                GameObject instantiatedGameObject = Instantiate(ItemDataGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                inventory.AddItemToEmptySlot(instantiatedGameObject);

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

    public void DropItem()
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
            itemNameText.text = ItemDataInSlot.itemName;
            TextMeshProUGUI ItemDescriptionText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemDescriptionText").gameObject.GetComponent<TextMeshProUGUI>();
            ItemDescriptionText.text = ItemDataInSlot.description;
            TextMeshProUGUI ItemStatsText = gameObject.transform.GetChild(0).gameObject.transform.Find("ItemStatsText").gameObject.GetComponent<TextMeshProUGUI>();
            if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.WEAPON)
            {
                ShowWeaponStats(ItemStatsText);
            }
            else if (ItemDataInSlot.itemType == ItemData.ITEMTYPE.HELMET || ItemDataInSlot.itemType == ItemData.ITEMTYPE.ARMOR)
            {
                ItemStatsText.text = "Armor: " + ItemDataInSlot.defense + "\n" + "Value: <color=yellow>" + ItemDataInSlot.value + " coins</color> ";
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

    private void ShowWeaponStats(TextMeshProUGUI itemStatsText)
    {
        var weapon = ItemDataGameObject.GetComponent<Weapon>();
        string powerDiff = "", cooldownDiff = "", projectileSpeedDiff = "";
        if (gameObject.name != GetEquipmentSlotNameForItem())
        {
            powerDiff = GetHoverDiffText(weapon.Power - playerAttack.CurrentWeapon.Power);
            cooldownDiff = GetHoverDiffText(weapon.Cooldown - playerAttack.CurrentWeapon.Cooldown, true);
            if (weapon.HasProjectileAttack && playerAttack.CurrentWeapon.HasProjectileAttack)
                projectileSpeedDiff = GetHoverDiffText(weapon.ProjectileAttack.Speed - playerAttack.CurrentWeapon.ProjectileAttack.Speed);
        }
        itemStatsText.text = $"Damage: {weapon.Power} {powerDiff}\n";
        itemStatsText.text += $"Cooldown: {weapon.Cooldown}s {cooldownDiff}\n";
        if (weapon.HasProjectileAttack)
            itemStatsText.text += $"Projectile speed: {weapon.ProjectileAttack.Speed} {projectileSpeedDiff}\n";
        itemStatsText.text += $"Value: <color=yellow>{ItemDataInSlot.value} coins</color> ";
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
