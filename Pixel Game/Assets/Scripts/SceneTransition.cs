using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    private GameObject dataToPassBetweenScenesGameObject;
    private GameObject myPlayerObject;
    private GameObject invManager;

    public Animator transition;

    // NOT WORKING AS OF NOW, CONTINUE WHEN THERE IS TIME
    public Vector3 spawnPosition;

    public bool isInside = false;
    // NOT WORKING AS OF NOW, CONTINUE WHEN THERE IS TIME


    private string transitionId;

    public Vector3 testSpawnPos;
    private void Start()
    {
        dataToPassBetweenScenesGameObject = GameObject.FindGameObjectWithTag("PassData");
        myPlayerObject = GameObject.FindGameObjectWithTag("MyPlayer");
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        //myPlayerObject.transform.position = 
        // NOT WORKING AS OF NOW, CONTINUE WHEN THERE IS TIME

        transitionId = gameObject.name;
        Debug.Log("Local scene trans object is: "+transitionId);


        string dbSceneTransId = dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().savedSceneTransitionId;
        testSpawnPos = GameObject.Find(dbSceneTransId).transform.Find("RespawnSpot").gameObject.transform.position;
        myPlayerObject.transform.position = testSpawnPos;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            // NOT WORKING AS OF NOW, CONTINUE WHEN THERE IS TIME
            //dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().outsidePos = thisRespawnSpot; // This works aswell
            //dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().sceneTransitionSpawnPoint = testSpawnPos;
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().savedSceneTransitionId = transitionId;
            // NOT WORKING AS OF NOW, CONTINUE WHEN THERE IS TIME


            // Here we save all data to our database (dataToPassBetweenScenesGameObject) on entering a new scene
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerHealthDB = myPlayerObject.GetComponent<PlayerHealth>().Hp;
            invManager.GetComponent<PlayerInventory>().SaveInvGameObjectsOnSceneChange();
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerMoneyDB = invManager.GetComponent<PlayerInventory>().playerInvMoney;
            StartCoroutine(LoadLevel(sceneToLoad));
        }
    }

    IEnumerator LoadLevel(string sceneToLoad)
    {
        // this actually works, turns the player pos to correct place (used as test)
        //myPlayerObject.transform.position = dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().currentHouseRespawnPosition;

        transition.SetTrigger("Start");
        // TO-DO - Disable player movement, Make player invincible

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneToLoad);
        
    }

    
}


