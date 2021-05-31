using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public override void Start()
    {
        OnSceneChange();
        base.Start();
    }

    public override void OnSceneChange()
    {
        var canvasPrefab = GameObject.FindWithTag("Canvas");
        slider = canvasPrefab.transform.GetChild(0).gameObject.GetComponent<Slider>();
        slider.value = Hp;
        base.OnSceneChange();
    }

    protected override void Kill()
    {
        base.Kill();
        Respawn();
    }
}
