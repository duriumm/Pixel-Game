﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject playerAttackColliderObject;
    //public GameObject plantBaseObject;            // Use plant object later on in the game, not like this. Make code in its collision
    private GameObject inventoryManager;
    //private GameObject mainCamera;
    private GameObject playerAttack;


    //public GameObject feetSocket;   // Do all this when equipment is added to the character
    //private Animator playerAnimator;

    // Test dragging on the boots here and see if it works? =)
    //public Equipment equippedBoots;   // Do all this when equipment is added to the character

    private GameObject playerGameObject;

    private GameObject torchGameObject;

    //public AudioClip attackSound; // TO-DO , move this to attackscript

    public GameObject dataToPassGameObject;

    void Start()
    {
        playerGameObject = this.gameObject;
        inventoryManager = GameObject.FindWithTag("InventoryManager");
        //mainCamera = GameObject.FindWithTag("MainCamera");
        //playerAnimator = playerGameObject.GetComponent<Animator>();
        //feetSocket.SetActive(false);  // Do all this when equipment is added to the character

        torchGameObject = playerGameObject.transform.GetChild(2).gameObject; // gets the index of where torch is in MyCharacter gameobject which is 4. Keep it at 4
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){
            // TODO, make this fucking get component stuff easier. no get component ffs!!
            if(torchGameObject.GetComponent<Torch>().isTorchActive == false) { 
                torchGameObject.GetComponent<Torch>().isTorchActive = true;
                torchGameObject.GetComponent<Torch>().toggleTorch();
            }
            else if (torchGameObject.GetComponent<Torch>().isTorchActive == true) {
                torchGameObject.GetComponent<Torch>().isTorchActive = false;
                torchGameObject.GetComponent<Torch>().toggleTorch();
            }


        }
        // checks if space is pressed AND if the collider in question is not currently active
        if (Input.GetMouseButtonDown(0) && playerGameObject.GetComponent<PlayerAttack>().enabled == true/* && playerAttackColliderObject.activeSelf == false*/) // We activate the collider in animation so might not need false check
        {
           playerGameObject.GetComponent<PlayerAttack>().getAttackCoroutine();
            // We activate the collider in animation so might not need this function
            //StartCoroutine(EnableAttackColliderAndWaitForSeconds(0.1f));
            //if (!inventoryManagerTest.GetComponent<PlayerInventory>().isInventoryOpen){
            // if (!inventoryManager.GetComponent<PlayerInventory>().isInventoryOpen){ // Dont attack if inventory is open
            //     StartCoroutine(AttackCoroutine());
            //}
            // }
        }
        // Primary test to see if the plant "growth" works. IT DOES!
        if (Input.GetKeyDown(KeyCode.C))
        {
            //playerGameObject.GetComponent<PlayerHealth>().myTestNumbers[0] = 8;
            for (int i = 0; i < dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().invGameObjectArray.Length; i++)
            {
                    Debug.Log("gameobject in DataToPass gameObj array: "+ dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().invGameObjectArray[i]);

            }
            
            //plantBaseObject.GetComponent<PlantScript>().IncrementPlantStage();    // Use plant object later on in the game, not like this. Make code in its collision
        }

        // TO-DO
        // In the future make it so all items that a player can
        // pickup is available for pickup when you press the B key.
        // Instead of as it is now, the plant checks the players collision,
        // make it so player can pick up any item.
        // TO-DO

        //if (Input.GetKeyDown(KeyCode.B) && plantBaseObject.GetComponent<PlantScript>().isPlantReadyForPickup == true)     // Use plant object later on in the game, not like this. Make code in its collision
        //{
        //    plantBaseObject.GetComponent<PlantScript>().PickUpPlant();
        //}

        if (Input.GetKeyDown(KeyCode.E))
        {

            // TO-DO
            // Fix get compnent here below so its faster with created object variable, no fucking get component!!!
            // TO-DO
            if (inventoryManager.GetComponent<PlayerInventory>().isInventoryOpen == false)
            {
                playerGameObject.GetComponent<PlayerAttack>().enabled = false;
                inventoryManager.GetComponent<PlayerInventory>().OpeningGUI();
            }
            else if(inventoryManager.GetComponent<PlayerInventory>().isInventoryOpen == true)
            {
                playerGameObject.GetComponent<PlayerAttack>().enabled = true;
                inventoryManager.GetComponent<PlayerInventory>().ClosingUI();
            }

        }
        // TEST 
        // Testing to equip the boots and deEquip them
        // TEST
        if (Input.GetKeyDown(KeyCode.F))
        {
            //// If boots are not equipped we DO equip them.        // Do all this when equipment is added to the character
            //if(feetSocket.activeSelf == false)
            //{
            //    feetSocket.SetActive(true);
            //}
            //// else if boots ARE equipped we DEequip them
            //else if(feetSocket.activeSelf == true)
            //{
            //    feetSocket.SetActive(false);
            //}

        }
    }
    /*
    private IEnumerator AttackCoroutine()
    {
        AudioSource.PlayClipAtPoint(attackSound, mainCamera.transform.position);
        // Play the attack animation
        playerAnimator.SetBool("isAttacking", true);
        yield return null; // skip a frame and then set isAttacking to false so we wont loop the attack
        playerAnimator.SetBool("isAttacking", false);

        playerGameObject.GetComponent<PlayerMovement>().enabled = false;
        
        yield return new WaitForSeconds(0.5f); // Wait for 1 seconds which is the animation (DIDNT NEED THIS??? MIGHT BE IF WE WANT TO SWITCH STATE LATER)
        playerGameObject.GetComponent<PlayerMovement>().enabled = true;

    }
    */

    public void EquipArmorTest(Equipment equipment)
    {
        Debug.Log("Equipping " + equipment.name);
    }
}
