using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public AudioClip openingSound;
    public AudioClip lootSound;


    private GameObject mainCamera;
    private bool isOpened = false;
    private Interactable chestInteractableScript;
    public Sprite openedEmptySprite;
    public Sprite openedWithLootSprite;
    private SpriteRenderer currentChestSprite;

    private GameObject dataToPassGameObject;
    public GameObject gameObjectLoot;
    public int moneyLoot;

    private PlayerInventory playerInventory;
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        chestInteractableScript = gameObject.GetComponent<Interactable>();
        currentChestSprite = gameObject.GetComponent<SpriteRenderer>();
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();

        if (dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().openedChestGameObjectNames.Contains(this.gameObject.name)){
            ClearChest();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // OpenChest() should only be useable if the chest is closed
    public void OpenChest()
    {
        currentChestSprite.sprite = openedWithLootSprite;
        // BUG - On looting the chest after opening it first time, the opening sound plays again
        AudioSource.PlayClipAtPoint(openingSound, mainCamera.transform.position);
        Debug.Log("YOU OPENED CHEST");
        chestInteractableScript.interactAction.AddListener(LootChest);          
    }

    public void LootChest()
    {
        openingSound.UnloadAudioData();
        currentChestSprite.sprite = openedEmptySprite;
        AudioSource.PlayClipAtPoint(lootSound, mainCamera.transform.position);
        chestInteractableScript.enabled = false;
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().openedChestGameObjectNames.Add(this.gameObject.name);
        if(moneyLoot != 0)
        {
            playerInventory.AddCoinAmount(moneyLoot);
        }
        if(gameObjectLoot != null)
        {
            // Get original name of obj so we dont get (clone) as name when we instantiate
            string originalItemName = gameObjectLoot.name;
            GameObject instantiatedObj = Instantiate(gameObjectLoot) as GameObject;
            instantiatedObj.name = originalItemName;
            playerInventory.LootItem(instantiatedObj);
        }
        
    }

    public void ClearChest()
    {
        currentChestSprite.sprite = openedEmptySprite;
        chestInteractableScript.enabled = false;
        this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }
}
