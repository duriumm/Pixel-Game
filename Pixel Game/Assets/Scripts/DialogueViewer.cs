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
    private GameObject dataToPassGameObject;

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
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
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

        // TO-DO - Implement this part and put all node tag if statements inside this if statement
        //if(newNode.tags != null)
        //{
        //    string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
        //    GameObject npcGameobject = GameObject.Find(currentActiveNpc);
        //}

        if (newNode.tags.Contains("END"))
        {
            // To-Do
            // Here we can close the window or something :) since the talk is finished
            Debug.Log("End!");
            dialogueCanvas.SetActive(false);
        }
        else if (newNode.tags.Contains("ACCEPT_QUEST"))
        {
            // If the current node has a "ACCEPT_QUEST" tag on it, we will take the current active npc from our database
            // then find the GameObject with the same name in the scene and run ActivateQuest() on that NpcData script

            string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);
            npcGameobject.GetComponent<NpcData>().ActivateQuest();
        }
        else if (newNode.tags.Contains("CHECK_QUEST"))
        {
            string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);

            // If the function returns false, the textNodeDisplay prints "you are not done, and the option to
            // go back appears with a button
            if(npcGameobject.GetComponent<NpcData>().CheckIfQuestIsDone() == false)
            {
                txtNodeDisplay.text = "You are not done...";
            }
        }
        else if (newNode.tags.Contains("QUEST_DONE"))
        {
            // Here we can enter the actual reward with a function GetReward() from a future 
            // questclass or just enter what the quest reward actually is
            txtNodeDisplay.text = "You got 10 gold and a bag of shit!!!";

            string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);
            // Load the last conversation that COMPLETELY ends the quest, and makes the npc just say something along
            // the lines of "You did a good job before, thanks" 
            npcGameobject.GetComponent<NpcData>().EndTheQuest();         
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
            string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);
            // Set our twinetext (dialogue text) to the Npcs current active convo and initialize the dialogue
            controller.twineText = npcGameobject.GetComponent<NpcData>().currentActiveConvo;
            controller.InitializeDialogue();
        }

    }
}