using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : MonoBehaviour
{

    public TextAsset currentActiveConvo;
    public List<TextAsset> questConvoList = new List<TextAsset>();
    // For now the NPC can only hold 1 quest at a time. Add list in the future for several
    public Quest currentNpcQuest;
    public AudioClip npcTalkSound;
    private DialogueController dialogueController;
    private DataToPassBetweenScenes dataToPass;
    private GameObject dialogueCanvas;
    private GameObject mainCamera;
    private PlayerInventory playerInventory;

    void Start()
    {
        dialogueController = GameObject.Find("DialogueController").GetComponent<DialogueController>();
        dataToPass = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        dialogueCanvas = GameObject.Find("DialogueCanvas");
        mainCamera = GameObject.FindWithTag("MainCamera");
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            // Some npcs might not have a sound attached which is fine. Therefor we check if its null
            if(npcTalkSound != null)
            {
                AudioSource.PlayClipAtPoint(npcTalkSound, mainCamera.transform.position);
            }

            // Set this NPCs dialogue to be active in dialogueController
            dialogueController.twineText = currentActiveConvo;
            // Initialize the NPCs dialogue so that when we open the dialogue window next time
            // we will see the NPCs dialogue instead of whatever was there before
            dialogueController.InitializeDialogue();
            // Set the currentActiveNpc to this NPC so that we can run the ActiveQuest() function for
            // only this specific NPC
            dataToPass.currentActivateNpc = this.gameObject.name;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            // Remove this npc from current active npc and set the dialogueCanvas to false
            dataToPass.currentActivateNpc = "";
            dialogueCanvas.SetActive(false);
        }
    }

    // To-Do - Activate a quest in a list of quests!
    public void ActivateQuest()
    {
        Debug.Log("QUEST ACTIVATYED on npc named: " + this.gameObject.name);

        foreach (var item in questConvoList)
        {
            if (item.name.Contains("Quest_Accepted"))
            {
                currentActiveConvo = item;
                Debug.Log("Current active convo is: " + currentActiveConvo.name);

                // Set this NPCs dialogue to be active in dialogueController gameobject 
                dialogueController.twineText = currentActiveConvo;
                // Initialize the NPCs dialogue so that when we open the dialogue window next time
                // we will see the NPCs dialogue instead of whatever was there before
                dialogueController.InitializeDialogue();

                break;
            }
        }
        
        // We turn dialogueCanvas off after the activation of the quest since otherwise 
        // the next conversation will be started immediately
        dialogueCanvas.SetActive(false);


        dataToPass.currentActivePlayerQuest = currentNpcQuest;

        // On activating a gather item quest, we need to check if we already have
        // one or more of said item in our inventory
        if(dataToPass.currentActivePlayerQuest.questType == Quest.QUESTTYPE.GATHER_ITEMS)
        {
            playerInventory.CheckInventoryForCollectedItems(
                dataToPass.currentActivePlayerQuest.itemToGather.GetComponent<ItemData>().itemName);
        }

        Debug.Log("Current active playerquest is: "+ dataToPass.currentActivePlayerQuest.questName);
    }
    public bool CheckIfQuestIsDone()
    {
        Debug.Log("Checking if the quest is finished!");

        if (dataToPass.currentActivePlayerQuest.isQuestFinished == true)
        {
            Debug.Log("quest is done!");
            foreach (var item in questConvoList)
            {
                if (item.name.Contains("Finished_Quest"))
                {
                    //convoList.Remove(currentActiveConvo);
                    currentActiveConvo = item;
                    
                }
            }
            // If the quest requirements are fullfilled, we want to start the next dialogue
            // immediately without closing dialogue canvas. So here below we start the next dialogue
            dialogueController.twineText = currentActiveConvo;
            dialogueController.InitializeDialogue();

            return true;
        }
        else
        {
            return false;
        }
    }

    // This will load the text for when the quest is COMPLETELY finished, for example
    // when old mans quest is completely done. He will just reply with "Great job before!" 
    // With this convo loaded we could either trigger the start of a normal conversation or a 
    // new quest conversation
    public void EndTheQuest()
    {
        foreach (var item in questConvoList)
        {
            if (item.name.Contains("After_Quest"))
            {
                if(dataToPass.currentActivePlayerQuest.questType == Quest.QUESTTYPE.GATHER_ITEMS)
                {
                    // When a gather items quest is done we want to clear the inventory of said items
                    string itemName = dataToPass.currentActivePlayerQuest.itemToGather.
                        GetComponent<ItemData>().itemName;
                    playerInventory.RemoveCollectedQuestItemsFromInventory(itemName);

                }
                //convoList.Remove(currentActiveConvo);
                currentActiveConvo = item;
            }
        }
    }
}
