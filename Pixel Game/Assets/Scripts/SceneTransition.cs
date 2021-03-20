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

    private void Start()
    {
        dataToPassBetweenScenesGameObject = GameObject.FindGameObjectWithTag("PassData");
        myPlayerObject = GameObject.FindGameObjectWithTag("MyPlayer");
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MyPlayer")
        {
            dataToPassBetweenScenesGameObject.GetComponent<DataToPassBetweenScenes>().playerHealth = myPlayerObject.GetComponent<PlayerHealth>().playerHealth;

            invManager.GetComponent<PlayerInventory>().SaveInvGameObjectsOnSceneChange();        
            StartCoroutine(LoadLevel(sceneToLoad));
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


