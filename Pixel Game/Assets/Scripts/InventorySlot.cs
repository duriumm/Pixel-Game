using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        slotIcon = this.gameObject.GetComponent<Image>();
        itemSlotButton = this.gameObject.GetComponent<Button>();
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
    }

    public void RemoveItem()
    {
        Debug.Log("You pressed button");
        if(customItemInSlot != null && invManager.GetComponent<PlayerInventory>().isInventoryOpen == true)
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
        }
    }

    public void SetAlphaOfColor(float alphaColorValue)
    { 
        ColorOfSlot = slotIcon.color;
        ColorOfSlot.a = alphaColorValue;
        slotIcon.color = ColorOfSlot;
    }

}
