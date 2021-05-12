using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{

    // Start() only runs once on game startup. FakeStart() runs when a scene is loaded.
    // If only FakeStart() is used, we run into some bug. Probably since we override
    // base Start() right? 
    protected override void Start()
    {
        var canvasPrefab = GameObject.FindWithTag("Canvas");
        slider = canvasPrefab.transform.GetChild(0).gameObject.GetComponent<Slider>();
        base.Start();
        Hp = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>().playerHealthDB;
    }

    public void FakeStart()
    {
        var canvasPrefab = GameObject.FindWithTag("Canvas");
        slider = canvasPrefab.transform.GetChild(0).gameObject.GetComponent<Slider>();
        base.Start();
        Hp = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>().playerHealthDB;
    }

    

    protected override void Kill()
    {
        base.Kill();
        Respawn();
    }
}
