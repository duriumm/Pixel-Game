using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScreen : MonoBehaviour
{
    public bool isShopScreenOpen = false;
    private GameObject inventoryManager;
    private InventorySlot[] shopSlots;
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

        Debug.Log("Amount of shopslots is: " + shopSlots.Length);

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
    }
    public void OpenShopScreen()
    {
        //// TO-DO - Loop through the selected npc and his itemlist and populate the shop with his items.
        //GameObject currentTraderNpc = GameObject.Find(dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActiveTrader);
        //foreach (var item in currentTraderNpc.GetComponent<TraderNpc>().shopItems)
        //{
        //    // TO-DO - Here we want to populate the shop instead of just showing all items
        //    Debug.Log(item.name);
        //}
        //foreach (var item in shopScreenItemList)
        //{
        //    // TO-DO - Add each item from the shopScreenItemList into the shopSlots[i]
        //    Debug.Log(item);
        //}

        Debug.Log(shopScreenItemList.Count);


        // Populate shop screens slots with copies of prefab items
        for (int i = 0; i < shopScreenItemList.Count; i++)
        {
            // We add the original gameobject prefab here into the shop slot since we instantiate the object on buying items anyway!
            shopSlots[i].AddItem(shopScreenItemList[i]); 
        }

        isShopScreenOpen = true;
        inventoryManager.GetComponent<PlayerInventory>().OpeningGUI();
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }


}
