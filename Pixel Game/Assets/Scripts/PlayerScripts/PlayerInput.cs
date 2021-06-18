using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private GuiScreenManager guiScreenManager;
    private Attack playerAttack;
    private Torch torch;

    public void Start()
    {
        OnSceneChange();
        playerAttack = GetComponent<Attack>();
        torch = gameObject.transform.Find("Torch").gameObject.GetComponent<Torch>();
    }

    public void OnSceneChange()
    {
        guiScreenManager = GameObject.Find("Canvas/Screens").gameObject.GetComponent<GuiScreenManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){

            torch.toggleTorch();
        }

        // checks if mousbuttonLEFT is pressed
        if (Input.GetMouseButtonDown(0) && playerAttack.enabled)
        {
            playerAttack.Execute();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            guiScreenManager.Toggle(GuiScreenType.Inventory);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            guiScreenManager.Toggle(GuiScreenType.Stats);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            guiScreenManager.Toggle(GuiScreenType.Journal);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            guiScreenManager.Toggle(GuiScreenType.Map);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            MapScreen mapScreen = guiScreenManager.gameObject.transform.Find("MapScreen").GetComponent<MapScreen>();
            mapScreen.StartCoroutine("RemoveCloudsOnDiscoveredArea");
        }
    }
}
