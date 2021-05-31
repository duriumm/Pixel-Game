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

    public int playerMoneyDB;
    public ActiveQuests activeQuests = new ActiveQuests();
    public ActiveQuests ActiveQuests => activeQuests;

    public string[] NpcDatabase = { "FatNpc", "Pawi", "SecondFatNPC" }; // A list of all the NPCs in our game. This list wont change
    public GameObject[] lootDatabase = { }; // A list of all the gameObjects in our game. This list wont change. 

    public string currentActiveNpc;

    public Vector3 sceneTransitionSpawnPoint;

    public string savedSceneTransitionId;

    public List<string> openedChestGameObjectNames= new List<string>();
    
    public string currentAreaName;

    // String List with names of all our gameobjects, all player inventory items names are saved here on entering new scene.
    public List<string> mySavedStringListDatabase = new List<string>();
    public List<string> savedEquipmentListDB = new List<string>(10);

    [SerializeField]
    private GameObject player;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("MyPlayer");
    }    
}
