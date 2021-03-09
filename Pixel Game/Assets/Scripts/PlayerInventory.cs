using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //public List<CustomItem> inventoryList = new List<CustomItem>();
    public InventorySlot[] slots;

    //public Transform parentObjectOfInventorySlots;
    private GameObject inventoryScreenGameObject;

    public GameObject prefabCanvas;
    private Transform inventorySlotsTransform;

    public bool isInventoryOpen = false;

    private CustomItem lootedCustomItem;


    public GameObject dataToPassGameObject;
    private void Awake()
    {
        if(GameObject.FindGameObjectWithTag("PassData") == null) 
        {
            Instantiate(dataToPassGameObject);
        }

    }
    void Start()
    {

        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        slots = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots;

        inventoryScreenGameObject = prefabCanvas.transform.GetChild(1).gameObject; // Get the second index gameobhject which is the inventoryscreen
        inventorySlotsTransform = inventoryScreenGameObject.transform.GetChild(6).transform; // Get the transform of inv screens child index 6 which is the InventorySlots gameobjects transform
        slots = inventorySlotsTransform.GetComponentsInChildren<InventorySlot>();
        ClosingUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClosingUI()
    {
        isInventoryOpen = false;
        inventoryScreenGameObject.GetComponent<CanvasGroup>().alpha = 0;

    }
    public void OpeningGUI()
    {
        isInventoryOpen = true;
        inventoryScreenGameObject.GetComponent<CanvasGroup>().alpha = 1;

    }

    // Loop through our slots list and check if any of the slots sprite icon is null
    // if it is, we can add our new item in there!
    public void addItemToSlot(/*CustomItem customItem*/GameObject lootedGameObject)
    {
        lootedCustomItem = lootedGameObject.GetComponent<CustomItem>();

        for (int i = 0; i < slots.Length; i++)
        {
            // Check each slots Image.sprite component if the sprite is empty == nothing is in that slot.
            if(slots[i].slotIcon.sprite == null)
            {
                Debug.Log(i + " sprite is null!");
                slots[i].AddItem(lootedGameObject);
                
                break;
            }
            else { Debug.Log(i + " sprite ISNT null!"); }        
        }

        // Destroying the gameobject you pick up makes it so the inventory 
        // cannot reference it for future deletes or usage. Disable it for now
        // but fix a List later so we can access these objects to instantiate etc etc

        //Destroy(lootedGameObject);
        lootedGameObject.SetActive(false);
    }


    public void LookThruArray()
    {
        int counter = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].customItemGameObject != null)
            {
                counter++;
            }
        }
        Debug.Log("We found " + counter + " gameObjects inside INV slots array");
    }
}
