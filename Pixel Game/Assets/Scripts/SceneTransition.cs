﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerInput playerInput;
    private Attack playerAttack;
    private Weapon playerWeapon;
    public string sceneToLoad;
    //private GameObject dataToPassBetweenScenesGameObject;
    private DataToPassBetweenScenes dataToPassBetweenScenes;
    private GameObject myPlayerObject;
    private GameObject invManager;

    public Animator transition;

    private string transitionId;

    private Vector3 spawnPos;

    public TextMeshProUGUI bannerText;
    private void Start()
    {
        dataToPassBetweenScenes = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        myPlayerObject = GameObject.FindGameObjectWithTag("MyPlayer");
        playerHealth = myPlayerObject.GetComponent<PlayerHealth>();
        playerInput = myPlayerObject.GetComponent<PlayerInput>();
        playerAttack = myPlayerObject.GetComponent<Attack>();
        playerWeapon = myPlayerObject.GetComponent<Weapon>();
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        transitionId = gameObject.name;

        // Get the savedSceneTransitionId from database and act only if not null (first ever scene)
        string dbSceneTransId = dataToPassBetweenScenes.savedSceneTransitionId;
        if (dbSceneTransId != "")
        {
            spawnPos = GameObject.Find(dbSceneTransId).transform.Find("RespawnSpot").gameObject.transform.position;
            myPlayerObject.transform.position = spawnPos;

            // For now the banner is playing everytime we enter a new scene. This can be changed to only 
            // display when entering scenes outdoors etc in the future
            bannerText.text = dataToPassBetweenScenes.currentAreaName;
        }
        
        
        // Run the Start() function for the scripts that are transfered between scenes
        // since they cant run Start() on scene change by themselves
        dataToPassBetweenScenes.Start(); 
        playerHealth.Start();
        playerInput.Start();
        playerAttack.Start();
        playerWeapon.Start();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            dataToPassBetweenScenes.savedSceneTransitionId = transitionId;

            // Here we save all data to our database (dataToPassBetweenScenesGameObject) on entering a new scene
            dataToPassBetweenScenes.playerHealthDB = myPlayerObject.GetComponent<PlayerHealth>().Hp;
            invManager.GetComponent<PlayerInventory>().SaveInvGameObjectsOnSceneChange();
            dataToPassBetweenScenes.playerMoneyDB = invManager.GetComponent<PlayerInventory>().playerInvMoney;
            StartCoroutine(LoadLevel(sceneToLoad));
            dataToPassBetweenScenes.currentAreaName = sceneToLoad;
        }
    }

    IEnumerator LoadLevel(string sceneToLoad)
    {
        transition.SetTrigger("Start");
        // TO-DO - Disable player movement, Make player invincible

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneToLoad);
        
    }
}


