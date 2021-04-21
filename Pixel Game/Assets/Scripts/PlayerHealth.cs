using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    protected override void Start()
    {
        base.Start();
        var canvasPrefab = GameObject.FindWithTag("Canvas");
        slider = canvasPrefab.transform.GetChild(0).gameObject.GetComponent<Slider>();
        // TODO, this loads players HP from the data script. :D
        Hp = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>().playerHealthDB;
    }

    protected override void Kill()
    {
        base.Kill();
        Respawn();
    }
}
