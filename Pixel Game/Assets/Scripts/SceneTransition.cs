using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    private GameObject dataToPassBetweenScenesGameObject;
    private GameObject myPlayerObject;
    private GameObject invManager;

    public Animator transition;

    private string transitionId;

    private Vector3 spawnPos;

    public TextMeshProUGUI bannerText;
    private void Start()
    {
        dataToPassBetweenScenesGameObject = GameObject.FindGameObjectWithTag("PassData");
        myPlayerObject = GameObject.FindGameObjectWithTag("MyPlayer");
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        transitionId = gameObject.name;

        // Get the savedSceneTransitionId from database and act only if not null (first ever scene)
        string dbSceneTransId = dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().savedSceneTransitionId;
        if (dbSceneTransId != "")
        {
            spawnPos = GameObject.Find(dbSceneTransId).transform.Find("RespawnSpot").gameObject.transform.position;
            myPlayerObject.transform.position = spawnPos;

            // For now the banner is playing everytime we enter a new scene. This can be changed to only 
            // display when entering scenes outdoors etc in the future
            bannerText.text = dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().currentAreaName;
        }
        

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().savedSceneTransitionId = transitionId;

            // Here we save all data to our database (dataToPassBetweenScenesGameObject) on entering a new scene
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerHealthDB = myPlayerObject.GetComponent<PlayerHealth>().Hp;
            invManager.GetComponent<PlayerInventory>().SaveInvGameObjectsOnSceneChange();
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerMoneyDB = invManager.GetComponent<PlayerInventory>().playerInvMoney;
            StartCoroutine(LoadLevel(sceneToLoad));
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().currentAreaName = sceneToLoad;
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


