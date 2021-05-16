using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{

    public override void Start()
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
