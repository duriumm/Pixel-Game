using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : MonoBehaviour
{
    // Assign a twine convo fort this NPC

    public TextAsset currentActiveConvo;


    public List<TextAsset> questConvoList = new List<TextAsset>();
    //public List<Quest> questList = new List<Quest>();



    private GameObject dialogueController;
    private GameObject dataToPassGameObject;
    private GameObject dialogueCanvas;

    public bool isQuestFinished = false;

    void Start()
    {
        dialogueController = GameObject.Find("DialogueController");
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        dialogueCanvas = GameObject.Find("DialogueCanvas");
        //Quest myQuest = new Quest("The first querre", "Kill three ghasts!!", 50);
        //questList.Add(myQuest);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            // Set this NPCs dialogue to be active in dialogueController gameobject 
            dialogueController.GetComponent<DialogueController>().twineText = currentActiveConvo;
            // Initialize the NPCs dialogue so that when we open the dialogue window next time
            // we will see the NPCs dialogue instead of whatever was there before
            dialogueController.GetComponent<DialogueController>().InitializeDialogue();
            // Set the currentActiveNpc to this NPC so that we can run the ActiveQuest() function for
            // only this specific NPC
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc = this.gameObject.name;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            // Remove this npc from current active npc and set the dialogueCanvas to false
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc = "";
            dialogueCanvas.SetActive(false);
        }
    }

    public void ProgressDialogue()
    {
        //currentActiveConvo = convoList
    }

    // To-Do - Activate a quest in a list of quests!
    public void ActivateQuest()
    {
        Debug.Log("QUEST ACTIVATYED on npc named: " + this.gameObject.name);
        //currentActiveConvo = convoList.

        foreach (var item in questConvoList)
        {
            if (item.name.Contains("Quest_Is_Accepted"))
            {
                // We CAN remove the old convo so it doesnt get mixed up, 
                // Is it neccecary though??? This removal can enable more quests in the future though 
                // since they can all be named "[something]__Quest_Is_Accepted"

                //convoList.Remove(currentActiveConvo);
                currentActiveConvo = item;
                Debug.Log("Current active convo is: " + currentActiveConvo.name);

                // Set this NPCs dialogue to be active in dialogueController gameobject 
                dialogueController.GetComponent<DialogueController>().twineText = currentActiveConvo;
                // Initialize the NPCs dialogue so that when we open the dialogue window next time
                // we will see the NPCs dialogue instead of whatever was there before
                dialogueController.GetComponent<DialogueController>().InitializeDialogue();

                break;

            }
        }
        //dialogueController.GetComponent<DialogueController>().twineText = questTwineConvo;
        
        // We turn dialogueCanvas off after the activation of the quest since otherwise 
        // the next conversation will be started immediately
        dialogueCanvas.SetActive(false);
    }
    public bool CheckIfQuestIsDone()
    {
        Debug.Log("Checking if the quest is finished!");

        if (isQuestFinished == true)
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
            dialogueController.GetComponent<DialogueController>().twineText = currentActiveConvo;
            dialogueController.GetComponent<DialogueController>().InitializeDialogue();

            return isQuestFinished;
        }
        else
        {
            return isQuestFinished;
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
            if (item.name.Contains("After_Quest_Finished"))
            {
                //convoList.Remove(currentActiveConvo);
                currentActiveConvo = item;
            }
        }
    }
}
