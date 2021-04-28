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

    public int playerHealthDB;
    public int playerArmorDB;
    public int playerAttackDB;

    public int playerMoneyDB;
    public string currentActiveTrader;
    public Quest currentActivePlayerQuest;

    public string[] NpcDatabase = { "FatNpc", "Pawi", "SecondFatNPC" }; // A list of all the NPCs in our game. This list wont change
    public GameObject[] lootDatabase = { }; // A list of all the gameObjects in our game. This list wont change.

    public string currentActivateNpc;


    public List<string> mySavedStringListDatabase = new List<string>();
    // String List with names of all our gameobjects, all player inventory items names are saved here on entering new scene.

    private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MyPlayer");
    }

    // We only need to update the player health stat as playerHealth has its own script 
    // This is because playerhealth will change a lot so we dont want to update this stat here
    // unneccecary :D

    // Armor for example will update the playerArmorDB value on equipping a new armor and therefor
    // will not need updating function but just grab the value straight from the DB or set the value 
    // straight to the DB
    public void UpdatePlayerHealthDB()
    {
        playerHealthDB = player.GetComponent<PlayerHealth>().Hp;
    }
}
