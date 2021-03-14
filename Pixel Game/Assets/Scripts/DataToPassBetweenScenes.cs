using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToPassBetweenScenes : MonoBehaviour
{

    //static DataToPassBetweenScenes instance;

    //private void Awake()
    //{
    //    if (instance != null)
    //    {
    //        Destroy(gameObject);
    //    }

    //    else
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}
    public int playerHealth;
    //public CustomItem[] playerInventoryCustomItems;

    //public InventorySlot[] playerSlots = { };

    //public int[] testIntArray = {1, 2, 3 };

    public GameObject[] invGameObjectArray = new GameObject[20];

    public GameObject[] lootDatabase = { }; // populated in inspector       // THIS IS WHAT WE USE

   // public string[] savedItemStringArray = new string[20];

    public List<string> mySavedStringListDatabase = new List<string>();      // THIS IS WHAT WE USE

    public void PrintAllSavedInventoryObjsInDataToPass(bool isExitScene)
    {
        foreach (GameObject gameObj in invGameObjectArray)
        {
            if(gameObj != null)
            {
                if(isExitScene == true)
                {
                    Debug.Log("DataToPass gameobjlist exiting scene: " + gameObj.name);
                }
                else
                {
                    Debug.Log("DataToPass gameobjlist entering new scene: " + gameObj.name);

                }

            }
            //Debug.Log("Its null =(");
        }
    }
}
