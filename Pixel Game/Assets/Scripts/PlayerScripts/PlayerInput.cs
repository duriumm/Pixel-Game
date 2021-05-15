﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject playerAttackColliderObject;
    private GameObject inventoryManager;
    private PlayerInventory playerInventory;
    private GameObject shopScreen;

    private GameObject playerGameObject;

    private Torch torch;

    public GameObject dataToPassGameObject;

    // Start() only runs once on game startup. FakeStart() runs when a scene is loaded
    public void FakeStart()
    {
        playerGameObject = this.gameObject;
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
        shopScreen = GameObject.Find("ShopScreen");
        torch = playerGameObject.transform.GetChild(2).gameObject.GetComponent<Torch>(); // gets the index of where torch is in MyCharacter gameobject which is 4. Keep it at 4
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){

            // TODO, make this fucking get component stuff easier. no get component ffs!!
            if(torch.isTorchActive == false) {
                torch.isTorchActive = true;
                torch.toggleTorch();
            }
            else if (torch.isTorchActive == true) {
                torch.isTorchActive = false;
                torch.toggleTorch();
            }
        }

        // checks if mousbuttonLEFT is pressed AND if the collider in question is not currently active
        // Checking that the inventory is closed before attacking
        if (Input.GetMouseButtonDown(0) && playerGameObject.GetComponent<Attack>().enabled == true)
        {
            playerGameObject.GetComponent<Attack>().Execute();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (playerInventory.isInventoryOpen == false && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
            {
                playerInventory.OpeningGUI();
            }
            else if(playerInventory.isInventoryOpen == true && shopScreen.GetComponent<ShopScreen>().isShopScreenOpen == false)
            {
                playerInventory.ClosingUI();
            }
        }
    }
}