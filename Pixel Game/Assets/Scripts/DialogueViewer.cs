using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DialogueObject;
using UnityEngine.Events;
using System;
using System.Runtime.InteropServices;

public class DialogueViewer : MonoBehaviour
{
    [SerializeField] Transform parentOfResponses;
    [SerializeField] Button prefab_btnResponse;
    [SerializeField] UnityEngine.UI.Text txtNodeDisplay;
    [SerializeField] DialogueController dialogueController;
    DialogueController controller;
    private GameObject dialogueCanvas;
    private DataToPassBetweenScenes dataToPass;
    private PlayerInventory playerInv;
    private PlayerInput playerInput;
    
    [DllImport("__Internal")]
    private static extern void openPage(string url);


    private void Start()
    {
        controller = dialogueController;
        controller.onEnteredNode += OnNodeEntered;
        controller.InitializeDialogue();

        // Start the dialogue
        var curNode = controller.GetCurrentNode();

        dialogueCanvas = this.gameObject;
        dialogueCanvas.SetActive(false);
        dataToPass = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        playerInv = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
        playerInput = GameObject.Find("MyCharacter").GetComponent<PlayerInput>();
    }

    public static void KillAllChildren(UnityEngine.Transform parent)
    {
        UnityEngine.Assertions.Assert.IsNotNull(parent);
        for (int childIndex = parent.childCount - 1; childIndex >= 0; childIndex--)
        {
            UnityEngine.Object.Destroy(parent.GetChild(childIndex).gameObject);
        }
    }

    private void OnNodeSelected(int indexChosen)
    {
        Debug.Log("Chose: " + indexChosen);
        controller.ChooseResponse(indexChosen);
    }

    private void OnNodeEntered(Node newNode)
    {
        if (dataToPass == null)
            return;
       
        Debug.Log("Entering node: " + newNode.title);
        txtNodeDisplay.text = newNode.text;

        KillAllChildren(parentOfResponses);
        for (int i = newNode.responses.Count - 1; i >= 0; i--)
        {
            int currentChoiceIndex = i;
            var response = newNode.responses[i];
            var responceButton = Instantiate(prefab_btnResponse, parentOfResponses);
            responceButton.GetComponentInChildren<UnityEngine.UI.Text>().text = response.displayText;
            responceButton.onClick.AddListener(delegate { OnNodeSelected(currentChoiceIndex); });
        }

        //TODO: Use member variable for current npc and update only when needed
        var npcObject = GameObject.Find(dataToPass.currentActiveNpc);
        if (npcObject == null)
            return;
        NpcData npc = npcObject.GetComponent<NpcData>();
        if (newNode.tags.Contains("END"))
        {
            Debug.Log("End!");
            dialogueCanvas.SetActive(false);
        }
        else if (newNode.tags.Contains("ACCEPT_QUEST"))
        {
            npc.ActivateQuest();
        }
        else if (newNode.tags.Contains("CHECK_QUEST"))
        {
            // If the function returns false, the textNodeDisplay prints "you are not done, and the option to
            // go back appears with a button
            if (!npc.CheckIfQuestIsDone())
            {
                txtNodeDisplay.text = "You are not done...";
            }
        }
        else if (newNode.tags.Contains("QUEST_DONE"))
        {
            // Give reward to the player accordingly to the current active quests reward
            // If there is a item like a sword as reward it is given to the player by
            // entering it into players inventory
                
            txtNodeDisplay.text = "You received:\n\n";
            if (npc.currentNpcQuest.MoneyReward > 0)
                txtNodeDisplay.text += npc.currentNpcQuest.MoneyReward + "gold\n";
            if (npc.currentNpcQuest.GameObjectReward != null)
                txtNodeDisplay.text += npc.currentNpcQuest.GameObjectReward.name;

            // Load the last conversation that COMPLETELY ends the quest, and makes the npc just say something along
            // the lines of "You did a good job before, thanks" 
            npc.EndQuest();
        }
    }

    // My own functions

    // For turning the dialogue canvas on/off
    public void SwitchDialogueCanvasOnOff()
    {
        if (dialogueCanvas.activeSelf == true) 
        { 
            dialogueCanvas.SetActive(false);
        }
        // Each time we turn the dialogue canvas ON, we want to also initialize the dialogue
        // that is the current active convo of the databases current active npc
        // With this fix, we can load up the correct convo even if we exit out of the convo
        else if (dialogueCanvas.activeSelf == false) {
            // Turn dialogue canvas off
            dialogueCanvas.SetActive(true);
            // Get the current active npc from database and Find that gameobject on the scene
            string currentActiveNpc = dataToPass.currentActiveNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);
            // Set our twinetext (dialogue text) to the Npcs current active convo and initialize the dialogue
            controller.twineText = npcGameobject.GetComponent<NpcData>().currentActiveConvo;
            controller.InitializeDialogue();
        }
    }

    void OnDisable()
    {
        Time.timeScale = 1;
        if (playerInput != null)
            playerInput.enabled = true;
    }

    void OnEnable()
    {
        Time.timeScale = 0;
        if (playerInput != null)
            playerInput.enabled = false;
    }
}