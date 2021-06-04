using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActiveQuestButton : MonoBehaviour
{
    public Quest buttonsActiveQuest;
    private TextMeshProUGUI questDescrioptionText;
    void Start()
    {
        //TODO: find description textbox
        questDescrioptionText = gameObject.transform.parent.gameObject.transform.parent.
            gameObject.transform.parent.Find("QuestDescription").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void ShowDescriptionOnClick()
    {
        questDescrioptionText.text = buttonsActiveQuest.Description;
    }


}
