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
    private DataToPassBetweenScenes dataToPass;
    public AudioClip buyAndSellSound;
    private GameObject mainCamera;
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

        //Transform a = prefabCanvas.transform.Find("InventoryScreen");
        inventoryScreenGameObject = GameObject.Find("InventoryScreen");
        //inventoryScreenGameObject = a.gameObject;
        inventorySlotsTransform = inventoryScreenGameObject.transform.GetChild(0).transform;
        slots = inventorySlotsTransform.GetComponentsInChildren<InventorySlot>();

        equipmentSlots = inventoryScreenGameObject.transform.GetChild(1).
        transform.GetComponentsInChildren<InventorySlot>(); // Get all equipment slots in player inventory

        // Player is set in awake since it needs to be called before the start in shopscreen 
        // since shopscreen uses shopscreen.close() 
        playerGameObject = GameObject.FindGameObjectWithTag("MyPlayer");
        dataToPass = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>();


    }
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        LoadInvGameObjectOnStartScene();
        LoadEquipmentOnStartScene();
        //ClosingUI();
        SetCoinAmount(dataToPass.playerMoneyDB);

    }

    // Check player inventory if there are already some of the items we need to collect
    // so we can increment the value correctly 
    public void CheckInventoryForCollectedItems(string itemNameToRemove)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.ItemDataInSlot.itemName == itemNameToRemove)
            {
                dataToPass.currentActivePlayerQuest.IncrementItemsCollected();
            }
        }
    }
    // When a gather items quest is finished, we want to remove the gathered items
    // from the players inventory, that is done here
    public void RemoveCollectedQuestItemsFromInventory(string itemNameToRemove)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.ItemDataInSlot.itemName == itemNameToRemove)
            {
                slot.DropItem();
                Destroy(slot.ItemDataGameObject);
                slot.ClearSlot();
            }
        }
    }

    // Adding items to players inventory is started here
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
        {
            if (dataToPass.currentActivePlayerQuest.questType == Quest.QUESTTYPE.GATHER_ITEMS)
            {
                if (lootedGameObject.GetComponent<ItemData>().itemName ==
                    dataToPass.currentActivePlayerQuest.itemToGather.GetComponent<ItemData>().itemName)
                {
                    dataToPass.currentActivePlayerQuest.IncrementItemsCollected();
                    Debug.Log("WE incremented FFS");
                }
            }
            AddItemToEmptySlot(lootedGameObject);
        }           
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
        dataToPass.mySavedStringListDatabase.Clear(); // Clear the database so we dont stack duplicates
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].ItemDataGameObject != null)
            {
                dataToPass.mySavedStringListDatabase.Add(slots[i].ItemDataGameObject.GetComponent<ItemData>().itemIdString); 
                //Debug.Log("This is what we saved before scene change: "+slots[i].ItemDataGameObject.ToString());
            }
        }
    }
    public void SaveEquipmentOnSceneChange()
    {
        dataToPass.savedEquipmentListDB.Clear(); // Clear the database so we dont stack duplicates
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].ItemDataGameObject != null)
            {
                dataToPass.savedEquipmentListDB.Add(equipmentSlots[i].ItemDataGameObject.GetComponent<ItemData>().itemIdString);
            }
        }
    }

    public void LoadEquipmentOnStartScene()
    {
        // Items previously equipped get added to the inventory 
        foreach (string savedEquipmentName in dataToPass.savedEquipmentListDB)
        {         
            for (int i = 0; i < dataToPass.lootDatabase.Length; i++)
            {
                if (savedEquipmentName == dataToPass.lootDatabase[i].name)
                {
                    GameObject instantiatedGameObject = Instantiate(dataToPass.lootDatabase[i]) as GameObject; // Instantiate object so we dont touch prefab
                    instantiatedGameObject.name = savedEquipmentName;  // Give the copied object the name of the original object so it doesnt get named (clone)
                    AddItemToEmptySlot(instantiatedGameObject);
                }
            }
        }
        // Items get equipped by checking databases saved equipped item list for a match
        foreach (var slot in slots)
        {
            // TODO: Check why we get a object reference not set here on line 276
            if (!slot.IsEmpty && dataToPass.savedEquipmentListDB.Count != 0 && 
                dataToPass.savedEquipmentListDB.Contains(slot.ItemDataInSlot.itemIdString))
            {
                StartCoroutine(slot.EquipItem());
            }
        }

        // On loading the inventory, if there is not any sprite in each inventory slot we want to
        // set the slots Alpha color to 0
        foreach (var item in equipmentSlots)
        {
            if (item.slotIcon.sprite == null) // try to: check sprite null, check Alpha null, check obj in slot null
            {
                item.SetAlphaOfColor(0f);
            }
        }
    }
    // here we check our itemDatabase for names matching our savedStringItems
    public void LoadInvGameObjectOnStartScene()
    {
        int counterGang = 0;
   
        foreach (string savedItemStringName in dataToPass.mySavedStringListDatabase)
        {
            Debug.Log(savedItemStringName);
            for (int i = 0; i < dataToPass.lootDatabase.Length; i++)
            {
                if(savedItemStringName == dataToPass.lootDatabase[i].name)
                {       
                    GameObject instantiatedGameObject = Instantiate(dataToPass.lootDatabase[i]) as GameObject; // Instantiate object so we dont touch prefab
                    instantiatedGameObject.name = savedItemStringName;  // Give the copied object the name of the original object so it doesnt get named (clone)
                    AddItemToEmptySlot(instantiatedGameObject);               
                }  
                counterGang++;
            }
        }

        
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
