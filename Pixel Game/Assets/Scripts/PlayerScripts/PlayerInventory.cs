using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInventory : MonoBehaviour
{
    private InventorySlot[] slots;
    private InventorySlot[] equipmentSlots;
    private GameObject inventoryScreenGameObject;
    public GameObject prefabCanvas;
    private Transform inventorySlotsTransform;
    public bool isInventoryOpen = false;
    private ItemData lootedItemData;
    public GameObject dataToPassGameObject;
    public AudioClip buyAndSellSound;
    private GameObject mainCamera;
    private GameObject screenTabs;
    private GameObject shopScreen;

    private GameObject playerGameObject;

    // TO-DO - Move playerMoney to some other place.. not sure where yet
    public int playerInvMoney;
    public TextMeshProUGUI CoinAmountText;

    private void Awake()
    {
        
        if (GameObject.FindGameObjectWithTag("PassData") != null)
        {
            dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");  
        }
        else
        {
            // This instantiates the dataToPassGameObject ONLY ONCE since "PassData" tag will only be null once
            Instantiate(dataToPassGameObject);
            dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");  
        }

        inventoryScreenGameObject = prefabCanvas.transform.GetChild(1).gameObject; // Get the second index gameobhject which is the inventoryscreen
        inventorySlotsTransform = inventoryScreenGameObject.transform.GetChild(0).transform; // Get the transform of inv screens child index 6 which is the InventorySlots gameobjects transform
        slots = inventorySlotsTransform.GetComponentsInChildren<InventorySlot>();

        equipmentSlots = inventoryScreenGameObject.transform.GetChild(1).
            transform.GetComponentsInChildren<InventorySlot>(); // Get all equipment slots in player inventory

        screenTabs = prefabCanvas.transform.Find("GuiTabsButtons").gameObject;
        shopScreen = GameObject.Find("ShopScreen");
        // Player is set in awake since it needs to be called before the start in shopscreen 
        // since shopscreen uses shopscreen.close() 
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer"); 


    }
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        LoadInvGameObjectOnStartScene();
        ClosingUI();
        SetCoinAmount(dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerMoneyDB);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClosingUI()
    {
        // disable player attack
        playerGameObject.GetComponent<Attack>().enabled = true;

        isInventoryOpen = false;
        inventoryScreenGameObject.GetComponent<CanvasGroup>().alpha = 0;
        screenTabs.SetActive(false);
        foreach (var item in slots)
        {
            item.GetComponent<EventTrigger>().enabled = false;
        }
        foreach (var item in equipmentSlots)
        {

            item.gameObject.SetActive(false);
        }
    }
    public void OpeningGUI()
    {
        // enable player attack
        playerGameObject.GetComponent<Attack>().enabled = false;
        isInventoryOpen = true;
        inventoryScreenGameObject.GetComponent<CanvasGroup>().alpha = 1;

        // These 2 loops enable the ability to hover over each slot when inv is closed
        if (shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == true)
        {
            foreach (var item in equipmentSlots)
            {
                item.gameObject.SetActive(false);
                //item.GetComponent<EventTrigger>().enabled = false;
                //item.GetComponent<Button>().enabled = false;
                //item.gameObject.
                //item.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var item in equipmentSlots)
            {
                item.gameObject.SetActive(true);
                //Debug.Log(item.name);
                //item.GetComponent<EventTrigger>().enabled = true;
                //item.GetComponent<Button>().enabled = true;

            }

        }
        foreach (var item in slots)
        {
            item.GetComponent<EventTrigger>().enabled = true;

        }
        // We should only show the tabs for each screen when opening the inventory 
        // if the shop is closed. Otherwise they will render on top of shop window and look weird
        if (shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
        {
            screenTabs.SetActive(true);
        }
    }

    public void LootItem(GameObject lootedGameObject)
    {
        bool isMoney = lootedGameObject.GetComponent<ItemData>().itemType == ItemData.ITEMTYPE.MONEY;
        if (isMoney)
        {
            playerInvMoney += lootedGameObject.GetComponent<ItemData>().value;
            SetCoinAmount(playerInvMoney);
            Destroy(lootedGameObject);
        }
        else
            AddItemToEmptySlot(lootedGameObject);
    }

    public bool AddItemToEmptySlot(GameObject itemToAdd)
    {
        //Look for empty slot
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.AddItem(itemToAdd);
                //If this was an equipped item being unequipped, it is already inactive but still visible in equipment slot
                //It is the EquipmentSlot caller's responsibility to clear it from slot
                itemToAdd.SetActive(false);
                return true;
            }
        }
        Debug.Log("Inventory was full");
        return false;
    }

    public void BuyAndSellItemSound()
    {
        AudioSource.PlayClipAtPoint(buyAndSellSound, mainCamera.transform.position);
    }
    public void AddCoinAmount(int coinAmountToAdd)
    {
        playerInvMoney = playerInvMoney + coinAmountToAdd;
        CoinAmountText.text = playerInvMoney.ToString();
    }
    public void RemoveCoinAmount(int coinAmountToRemove)
    {
        playerInvMoney = playerInvMoney - coinAmountToRemove;
        CoinAmountText.text = playerInvMoney.ToString();
    }
    public void SetCoinAmount(int coinAmount)
    {
        playerInvMoney = coinAmount;
        CoinAmountText.text = coinAmount.ToString();
    }

    // Here we take each ItemDataGameobject from each slot in our inventory and add 
    // its NAME to our stringDatabase 
    public void SaveInvGameObjectsOnSceneChange()
    {
        int saveCounter = 0;
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Clear(); // Clear the database so we dont stack duplicates
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].ItemDataGameObject != null)
            {
                dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Add(slots[i].ItemDataGameObject.GetComponent<ItemData>().itemIdString); 
                Debug.Log("This is what we saved before scene change: "+slots[i].ItemDataGameObject.ToString());
            }
            saveCounter++;
        }
    }

    // here we check our itemDatabase for names matching our savedStringItems
    public void LoadInvGameObjectOnStartScene()
    {
        int counterGang = 0;
        // TODO - Instantiate Local list here which we can send to addItemToSlot()
        foreach (string savedItemStringName in dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase)
        {
            Debug.Log(savedItemStringName);
            for (int i = 0; i < dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase.Length; i++)
            {
                if(savedItemStringName == dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase[i].name)
                {
                    // TODO - Add to local list ??? dont know what i ment here         
                    GameObject instantiatedGameObject = Instantiate(dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase[i]) as GameObject; // Instantiate object so we dont touch prefab
                    instantiatedGameObject.name = savedItemStringName;  // Give the copied object the name of the original object so it doesnt get named (clone)
                    AddItemToEmptySlot(instantiatedGameObject);               
                }  
                counterGang++;
            }
        }
        // TODO - addItemToSlot(LocalListGang)
        
        // On loading the inventory, if there is not any sprite in each inventory slot we want to
        // set the slots Alpha color to 0
        foreach (var item in slots)
        {           
            if (item.slotIcon.sprite == null) // try to: check sprite null, check Alpha null, check obj in slot null
            {
                item.SetAlphaOfColor(0f);
            }
        }
    }

}
