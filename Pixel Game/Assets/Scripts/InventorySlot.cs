using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


/* Inventory slot script is used on each inventory slot of the player AND on each shop screens slot.
   Players inventory slot is using Remove() func on buttonpress while shop slot is using BuyItemFromShop()
   on button press
*/

public class InventorySlot : MonoBehaviour
{
    public Image slotIcon;
    public Button itemSlotButton;
    private GameObject playerCharacter;
    private Color ColorOfSlot;
    private Vector2 itemDropPosition;

    public ItemData ItemDataInSlot;
    public GameObject ItemDataGameObject;
    private GameObject invManager;
    private GameObject shopScreen;

    private EventTrigger eventTrigger;
    

    private void Awake()
    {
        eventTrigger = this.gameObject.GetComponent<EventTrigger>();
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        slotIcon = this.gameObject.GetComponent<Image>();
        itemSlotButton = this.gameObject.GetComponent<Button>();
        shopScreen = GameObject.Find("ShopScreen");
    }
    public void Start()
    {

    }

    public void ClearAllDataFromSlot()
    {
        ItemDataInSlot = null;
        slotIcon.sprite = null;
        ItemDataGameObject = null;
        SetAlphaOfColor(0f);
    }
    public void AddHelmetSlotToInventory()
    {
        invManager.GetComponent<PlayerInventory>().addItemToSlot(ItemDataGameObject);
    }
    public void AddItem(GameObject gameObjectToAdd)
    {
        // BUG
        // Sometimes when picking up item, the item get duplicated. 
        // This is probably because the picked up item gets instantiated
        // and the original doesnt get destroyed...
        // BUG

        ItemDataInSlot = gameObjectToAdd.GetComponent<ItemData>();
        ItemDataGameObject = gameObjectToAdd;
        slotIcon.sprite = ItemDataInSlot.itemIcon; 
        
        // Set alpha of slot to 1 so we can see the item sprite
        SetAlphaOfColor(1f);
        //ItemDataGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
    }
    public void BuyItemFromShop()
    {
        // If there is an actual item in current slot and inventory is actually open
        if (ItemDataInSlot != null && invManager.GetComponent<PlayerInventory>().isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            if(invManager.GetComponent<PlayerInventory>().playerInvMoney >= ItemDataInSlot.value)
            {
                invManager.GetComponent<PlayerInventory>().BuyAndSellItemSound();
                string originalItemName = ItemDataGameObject.name;
                GameObject instantiatedGameObject = Instantiate(ItemDataGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                invManager.GetComponent<PlayerInventory>().addItemToSlot(instantiatedGameObject);

                invManager.GetComponent<PlayerInventory>().RemoveCoinAmount(ItemDataInSlot.value);
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
        Debug.Log("You pressed button");

        Debug.Log("Is shot screen open: " + shopScreen.GetComponent<ShopScreen>().isShopScreenOpen);
        // If there is an actual item in current slot and inventory is actually open
        if (ItemDataInSlot != null && invManager.GetComponent<PlayerInventory>().isInventoryOpen == true)
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
                invManager.GetComponent<PlayerInventory>().BuyAndSellItemSound();
                invManager.GetComponent<PlayerInventory>().AddCoinAmount(ItemDataGameObject.GetComponent<ItemData>().value);
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
            if (ItemDataInSlot.itemType != ItemData.ITEMTYPE.EDIBLE)
            {
                ItemStatsText.text = "Damage: "+ItemDataInSlot.damage+"\n"+"Defense: "+ItemDataInSlot.defense+"\n"+"Value: "+ItemDataInSlot.value;
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
