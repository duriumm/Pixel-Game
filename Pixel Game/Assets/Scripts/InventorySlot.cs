using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    public CustomItem customItemInSlot;
    public GameObject customItemGameObject;
    private GameObject invManager;
    private GameObject shopScreen;
    

    private void Awake()
    {
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        slotIcon = this.gameObject.GetComponent<Image>();
        itemSlotButton = this.gameObject.GetComponent<Button>();
        shopScreen = GameObject.Find("ShopScreen");
    }
    public void Start()
    {

    }
    public void AddItem(GameObject gameObjectToAdd)
    {
        // BUG
        // Sometimes when picking up item, the item get duplicated. 
        // This is probably because the picked up item gets instantiated
        // and the original doesnt get destroyed...
        // BUG

        customItemInSlot = gameObjectToAdd.GetComponent<CustomItem>();
        customItemGameObject = gameObjectToAdd;
        slotIcon.sprite = customItemInSlot.itemIcon; 
        
        // Set alpha of slot to 1 so we can see the item sprite
        SetAlphaOfColor(1f);
        //customItemGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
    }
    public void BuyItemFromShop()
    {
        // If there is an actual item in current slot and inventory is actually open
        if (customItemInSlot != null && invManager.GetComponent<PlayerInventory>().isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            if(invManager.GetComponent<PlayerInventory>().playerInvMoney >= customItemInSlot.value)
            {
                invManager.GetComponent<PlayerInventory>().BuyAndSellItemSound();
                string originalItemName = customItemGameObject.name;
                GameObject instantiatedGameObject = Instantiate(customItemGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                invManager.GetComponent<PlayerInventory>().addItemToSlot(instantiatedGameObject);

                invManager.GetComponent<PlayerInventory>().RemoveCoinAmount(customItemInSlot.value);
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
        if (customItemInSlot != null && invManager.GetComponent<PlayerInventory>().isInventoryOpen == true)
        {
            if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
            {
                // TO-DO
                // Set spawning of item position further away from player so loot 
                // doesnt get picked up instantly again by the player. 
                // TO-DO

                string realItemName = customItemInSlot.name; // Get the original name of the gameobject
                GameObject instantiatedGameObject = Instantiate(customItemGameObject) as GameObject; // Create a new gameObject
                instantiatedGameObject.name = realItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)

                // Set spawn position of item to where the player is standing currently
                // TO-DO - Spawn the item in a cricle around the player of random numbers
                itemDropPosition.x = playerCharacter.transform.position.x + 1f;
                itemDropPosition.y = playerCharacter.transform.position.y + 1f;
                instantiatedGameObject.transform.position = itemDropPosition;
                instantiatedGameObject.SetActive(true);
                Destroy(customItemGameObject);

                // Set alpha of slot to 0 and assign the items to the slot.
                SetAlphaOfColor(0f);
                customItemInSlot = null;
                slotIcon.sprite = null; 
                Debug.Log("Removed inventory item from GUI");
                instantiatedGameObject.hideFlags = HideFlags.HideInHierarchy; // THIS HIDES THE GAMEOBJECTS IN THE HIREARCHY SCENE SO WE CANT SEE THEM
            }
            // If the shops item screen is open, we will sell our inventory slot item instead of dropping it on the ground
            else if(shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
            {
                invManager.GetComponent<PlayerInventory>().BuyAndSellItemSound();
                invManager.GetComponent<PlayerInventory>().AddCoinAmount(customItemGameObject.GetComponent<CustomItem>().value);
                Destroy(customItemGameObject);
                SetAlphaOfColor(0f);
                customItemInSlot = null;
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

}
