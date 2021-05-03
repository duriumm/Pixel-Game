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
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        chestInteractableScript = gameObject.GetComponent<Interactable>();
        currentChestSprite = gameObject.GetComponent<SpriteRenderer>();      
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
        currentChestSprite.sprite = openedEmptySprite;
        AudioSource.PlayClipAtPoint(lootSound, mainCamera.transform.position);
        chestInteractableScript.interactAction = null;
    }
}
