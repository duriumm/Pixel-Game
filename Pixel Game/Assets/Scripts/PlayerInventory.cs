using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool isNewScene = false;
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("PassData") == null && SceneManager.GetActiveScene().name != "Indoors") // Testing with static variables instead
        {
            Instantiate(dataToPassGameObject);
        }

    }
    void Start()
    {
        isNewScene = true;
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");  // Testing with static variables instead
        //slots = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots;


        inventoryScreenGameObject = prefabCanvas.transform.GetChild(1).gameObject; // Get the second index gameobhject which is the inventoryscreen
        inventorySlotsTransform = inventoryScreenGameObject.transform.GetChild(6).transform; // Get the transform of inv screens child index 6 which is the InventorySlots gameobjects transform
        slots = inventorySlotsTransform.GetComponentsInChildren<InventorySlot>();
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().PrintAllSavedInventoryObjsInDataToPass(false); // printing all saved items on startup (isExiting scene = false)

        LoadInvGameObjectOnStartScene();
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
        //DontDestroyOnLoad(lootedGameObject);    // If we dont do this, we cant bring the items over to next scene
        for (int i = 0; i < slots.Length; i++)
        {

            // Check each slots Image.sprite component if the sprite is empty == nothing is in that slot.
            if(slots[i].customItemGameObject == null)
            {
                Debug.Log("index "+ i + " sloticon.sprite is null! therefor we can add item there");
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

    // Here we take each CustomitemGameobject from each slot in our inventory and add 
    // its NAME to our stringDatabase 
    public void SaveInvGameObjectsOnSceneChange()
    {
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Clear(); // Clear the database so we dont stack duplicates
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].customItemGameObject != null)
            {
                dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase.Add(slots[i].customItemGameObject.name); 
                Debug.Log("This is what we added"+slots[i].customItemGameObject.ToString());
            }
        }
    }

    // here we check our itemDatabase for names matching our savedStringItems
    public void LoadInvGameObjectOnStartScene()
    {
        foreach (string savedItemStringName in dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().mySavedStringListDatabase)
        {
            for (int i = 0; i < dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase.Length; i++)
            {
                if(savedItemStringName == dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase[i].name)
                {
                    string originalItemName = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase[i].name; // Get the original name of the gameobject
                    GameObject instantiatedGameObject = Instantiate(dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().lootDatabase[i]) as GameObject; // Instantiate object so we dont touch prefab
                    instantiatedGameObject.name = originalItemName;  // Give the copied object the name of the original object so it doesnt get named (clone)
                    Debug.Log("item we are about to add to slots is: " + instantiatedGameObject.name);
                    addItemToSlot(instantiatedGameObject);
                }
            }
        }

    }

}
