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

    private const string questText = "QUEST";

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

        if (newNode.tags.Contains("END"))
        {
            // To-Do
            // Here we can close the window or something :) since the talk is finished
            Debug.Log("End!");
        }
        if (newNode.tags.Contains(questText))
        {
            // If the current node has a "QUEST" tag on it, we will take the current active npc from our database
            // then find the GameObject with the same name in the scene and run ActivateQuest() on that NpcData script
            Debug.Log("DE VA EN KVEEEST");

            string currentActiveNpc = dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc;
            GameObject npcGameobject = GameObject.Find(currentActiveNpc);
            npcGameobject.GetComponent<NpcData>().ActivateQuest();
        }
    }

    // My own functions

    // For turning the dialogue canvas on/off
    public void SwitchDialogueCanvasOnOff()
    {
        if (dialogueCanvas.activeSelf == true) { dialogueCanvas.SetActive(false); }
        else if (dialogueCanvas.activeSelf == false) { dialogueCanvas.SetActive(true); }

    }
}