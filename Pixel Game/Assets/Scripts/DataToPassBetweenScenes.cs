﻿using System.Collections;
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


    public GameObject[] lootDatabase = { }; // A list of all the gameObjects in our game. This list wont change.

    public List<string> mySavedStringListDatabase = new List<string>(); 
    // String List with names of all our gameobjects, all player inventory items names are saved here on entering new scene.
}