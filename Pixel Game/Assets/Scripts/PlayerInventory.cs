using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private InventorySlot[] slots;
    private GameObject inventoryScreenGameObject;
    public GameObject prefabCanvas;
    private Transform inventorySlotsTransform;
    public bool isInventoryOpen = false;
    private CustomItem lootedCustomItem;
    public GameObject dataToPassGameObject;
    public AudioClip buyAndSellSound;
    private GameObject mainCamera;

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
        inventorySlotsTransform = inventoryScreenGameObject.transform.GetChild(6).transform; // Get the transform of inv screens child index 6 which is the InventorySlots gameobjects transform
        slots = inventorySlotsTransform.GetComponentsInChildren<InventorySlot>();
        
    }
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        LoadInvGameObjectOnStartScene();
        ClosingUI();
        SetCoinAmount(dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerMoney);
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
        if(lootedGameObject.GetComponent<CustomItem>().itemType != CustomItem.ITEMTYPE.MONEY)
        {
            // lootedCustomItem = lootedGameObject.GetComponent<CustomItem>(); // Never use this :(
            for (int i = 0; i < slots.Length; i++)
            {

                // Check each slots customGameobject if its empty == nothing is in that slot.
                if(slots[i].customItemGameObject == null)
                {
                    //Debug.Log("index "+ i + " sloticon.sprite is null! therefor we can add item there");
                    slots[i].AddItem(lootedGameObject);
                
                    break;
                    //if(slots[i].slotIcon.sprite == null)
                    //{
                    //}
                    //else { Debug.Log(i + " sprite ISNT null!"); }        
                }
            }

            // Destroying the gameobject you pick up makes it so the inventory 
            // cannot reference it for future deletes or usage. Disable it for now
            // but fix a List later so we can access these objects to instantiate etc etc

            //Destroy(lootedGameObject);
            lootedGameObject.SetActive(false);
        }
        else if(lootedGameObject.GetComponent<CustomItem>().itemType == CustomItem.ITEMTYPE.MONEY)
        {
            playerInvMoney+= lootedGameObject.GetComponent<CustomItem>().value;
            SetCoinAmount(playerInvMoney);
            Destroy(lootedGameObject);
        }
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

    // Here we take each CustomitemGameobject from each slot in our inventory and add 
    // its NAME to our stringDatabase 
    public void SaveInvGameObjectsOnSceneChange()
    {
        int saveCounter = 0;
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Clear(); // Clear the database so we dont stack duplicates
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].customItemGameObject != null)
            {
                dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Add(slots[i].customItemGameObject.GetComponent<CustomItem>().itemIdString); 
                Debug.Log("This is what we saved before scene change: "+slots[i].customItemGameObject.ToString());
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
                    addItemToSlot(instantiatedGameObject);               
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
