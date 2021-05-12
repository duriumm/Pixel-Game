using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScreen : MonoBehaviour
{
    public bool isShopScreenOpen = false;
    private GameObject inventoryManager;
    public InventorySlot[] shopSlots;
    private GameObject inventoryScreenGameObject;
    public GameObject sceneCanvas;
    private Transform shopSlotsTransform;
    private GameObject dataToPassGameObject;
    public List<GameObject> shopScreenItemList = new List<GameObject>();
    public string currentActiveNpc;
    void Start()
    {
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        inventoryManager = GameObject.FindWithTag("InventoryManager");
        CloseShopScreen();
        //inventoryScreenGameObject = sceneCanvas.transform.GetChild(1).gameObject; // Get the second index gameobhject which is the inventoryscreen
        //shopSlotsTransform = inventoryScreenGameObject.transform.GetChild(6).transform; // Get the transform of inv screens child index 6 which is the InventorySlots gameobjects transform
        shopSlots = this.gameObject.transform.GetComponentsInChildren<InventorySlot>();

        //Debug.Log("Amount of shopslots is: " + shopSlots.Length);

        // Set all shop slot sprites to alpha color 0, otherwise we will see a white box instead of empty item slot
        foreach (var item in shopSlots)
        {
            if (item.slotIcon.sprite == null) // try to: check sprite null, check Alpha null, check obj in slot null
            {
                item.SetAlphaOfColor(0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseShopScreen()
    {
        isShopScreenOpen = false;
        inventoryManager.GetComponent<PlayerInventory>().ClosingUI();
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OpenShopScreen()
    {

        // Clear all data flor shop slots so we can add new data!
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].ClearSlot();
        }

        // Populate shop screens slots with copies of prefab items
        for (int i = 0; i < shopScreenItemList.Count; i++)
        {
            // We add the original gameobject prefab here into the shop slot since we instantiate the object on buying items anyway!
            shopSlots[i].AddItem(shopScreenItemList[i]); 
        }

        isShopScreenOpen = true;
        inventoryManager.GetComponent<PlayerInventory>().OpeningGUI();
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
