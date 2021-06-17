using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScreen : GuiScreen
{
    // CanvasGroups for the different cloud objects. CG = CanvasGroup
    private CanvasGroup woodAreaCG;
    public AudioClip openMapSound;
    private GameObject mainCamera;
    void Start()
    {
        woodAreaCG = transform.Find("CloudsWoodArea").GetComponent<CanvasGroup>();
        Close();
        mainCamera = GameObject.FindWithTag("MainCamera");
    }



    public override void Open()
    {
        base.Open();
        AudioSource.PlayClipAtPoint(openMapSound,mainCamera.transform.localPosition);
    }

    public IEnumerator RemoveCloudsOnDiscoveredArea()
    {
        for (float i = 1f; i >= 0; i-= 0.05f)
        {
            woodAreaCG.alpha = i;
            yield return new WaitForSeconds(0.10f);
        }
    }
}
