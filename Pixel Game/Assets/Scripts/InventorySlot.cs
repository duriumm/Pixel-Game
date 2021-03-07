using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image slotIcon;
    public Button itemSlotButton;
    private GameObject playerCharacter;
    Color ColorOfSlot;
    private Vector2 itemDropPosition;

    public CustomItem customItemInSlot;
    public GameObject customItemGameObject;

    private void Awake()
    {

    }
    public void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        itemSlotButton = this.gameObject.GetComponent<Button>();
        slotIcon = this.gameObject.GetComponent<Image>();
        SetAlphaOfColor(0);
    }
    public void AddItem(GameObject gameObjectToAdd)
    {
        // BUG
        // Sometimes when picking up item, the item get duplicated. 
        // This is probably because the picked up item gets instantiated
        // and the original doesnt get destroyed...
        // BUG
        // Set alpha of slot to 1 and assign the items to the slot.
        SetAlphaOfColor(1);
        customItemInSlot = gameObjectToAdd.GetComponent<CustomItem>();
        customItemGameObject = gameObjectToAdd;
        slotIcon.sprite = customItemInSlot.itemIcon;       
        Debug.Log("Added inventory item to GUI");
    }

    public void RemoveItem()
    {
        Debug.Log("Test 1");
        Debug.Log(customItemInSlot.name);
        if(customItemInSlot != null)
        {
            Debug.Log("Test 2");
            // TO-DO
            // Set spawning of item position further away from player so loot 
            // doesnt get picked up instantly again by the player. 
            // TO-DO

            // TO-DO
            // make a NEW item and not a clone of a clone of a clone... it wont work later on..
            // For now we only make clones of the item we first picked up.
            // TO-DO
            GameObject instantiatedGameObject = Instantiate(customItemGameObject) as GameObject;
            // Set spawn position of item to where the player is standing currently
            itemDropPosition.x = playerCharacter.transform.position.x + 1f;
            itemDropPosition.y = playerCharacter.transform.position.y + 1f;
            instantiatedGameObject.transform.position = itemDropPosition;
            instantiatedGameObject.SetActive(true);
            Destroy(customItemGameObject);

            // Set alpha of slot to 0 and assign the items to the slot.
            SetAlphaOfColor(0);
            customItemInSlot = null;
            slotIcon.sprite = null;       
            Debug.Log("Removed inventory item from GUI");
        }
        Debug.Log("Test 3");
    }

    public void SetAlphaOfColor(float alphaColorValue)
    { 
        ColorOfSlot = slotIcon.color;
        ColorOfSlot.a = alphaColorValue;
        slotIcon.color = ColorOfSlot;
    }

}
