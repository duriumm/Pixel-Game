using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private GuiScreenManager guiScreenManager;
    private Attack playerAttack;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Torch torch;
    private Rigidbody2D rb;

    public void Start()
    {
        OnSceneChange();
        playerAttack = GetComponent<Attack>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        torch = gameObject.transform.Find("Torch").gameObject.GetComponent<Torch>();
    }

    public void OnSceneChange()
    {
        guiScreenManager = GameObject.Find("Canvas/Screens").gameObject.GetComponent<GuiScreenManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

            torch.toggleTorch();
        }

        // checks if mousbuttonLEFT is pressed
        if (Input.GetMouseButtonDown(0) && playerAttack.enabled)
        {

            playerAttack.Execute();
            StartCoroutine(DisableMovementDuringAttack());

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

    private IEnumerator DisableMovementDuringAttack()
    {
        //wplayerMovement.MaxSpeed = 0;
        playerMovement.enabled = false;
        rb.simulated = false;
        
        yield return new WaitForSeconds(0.5f);
        playerMovement.enabled = true;
        rb.simulated = true;
        

    }
}
