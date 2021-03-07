using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    private GameObject dataToPassBetweenScenesGameObject;
    private GameObject testMyPlayerObject;
    private GameObject invManager;

    private void Start()
    {
        dataToPassBetweenScenesGameObject = GameObject.FindGameObjectWithTag("PassData");
        testMyPlayerObject = GameObject.FindGameObjectWithTag("MyPlayer");
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");

        // Look thru array of saves slots :) on start of scene
        //LookThruArray(dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots, true);

        // Look at the first object in saved datapass array
        //Debug.Log("First index of saved dataPass array AFTER LOADING NEW SCENE: " + dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots[0]);

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            // This saves current health to the dontDestroyOnLoad object correctly
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerHealth = testMyPlayerObject.GetComponent<PlayerHealth>().playerHealth;
 
            // This should save list of stuff to the same dontDestroyOnLoad
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots = invManager.GetComponent<PlayerInventory>().slots;

            // Look at the first object in saved datapass array
           // Debug.Log("First index of saved dataPass array BEFORE LOADING NEW SCENE: " + dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots[0].customItemInSlot.itemName);
            //Loop thru the saved array BEFORE switching scenes but AFTER saving the data
            LookThruArray(dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots, false);

           Debug.Log("dataPass slots after copy but before scene change: "+ dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerSlots.Length);



           DontDestroyOnLoad(dataToPassBetweenScenesGameObject);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    public void LookThruArray(InventorySlot[] arrayToLoopThru, bool isSecondScene)
    {

        int counter = 0;
        for (int i = 0; i < arrayToLoopThru.Length; i++)
        {
            if (arrayToLoopThru[i].customItemGameObject != null)
            {
                counter++;
            }
        }
        if(isSecondScene == true)
        {
            Debug.Log("We found " + counter + " gameObjects inside dataPass array AFTER switching scenes");
        }
        else
        {
            Debug.Log("We found " + counter + " gameObjects inside dataPass array BEFORE switching scenes");
        }
    }
}


