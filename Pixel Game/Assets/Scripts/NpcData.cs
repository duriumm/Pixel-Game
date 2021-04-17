using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : MonoBehaviour
{
    // Assign a twine convo fort this NPC
    public TextAsset singleTwineConvo;
    private GameObject dialogueController;
    private GameObject dataToPassGameObject;

    void Start()
    {
        dialogueController = GameObject.Find("DialogueController");
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            // Set this NPCs twine convo to be active in dialoguereader and then initialize the dialogue for the convo so it is loaded for next convo :)
            dialogueController.GetComponent<DialogueController>().twineText = singleTwineConvo;
            dialogueController.GetComponent<DialogueController>().InitializeDialogue();
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc = this.gameObject.name;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActivateNpc = "";
        }
    }

    // To-Do - Activate a quest in a list of quests!
    public void ActivateQuest()
    {
        Debug.Log("QUEST ACTIVATYED on npc named: "+ this.gameObject.name);
    }
}
