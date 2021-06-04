using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalScreen : GuiScreen
{
    public GameObject activeQuestPanelPrefab;
    private GameObject originalQuestPanel;
    private GameObject parentOfOriginalQuestPanel;
    public float distanceToNextQuestPanel = 5.17f;
    private Vector3 origQuestPanelPos;
    string originalPanelName;
    ActiveQuests activeQuests;

    DataToPassBetweenScenes dataToPass;
    private TextMeshProUGUI questPanelText;
    void Start()
    {
        originalQuestPanel = gameObject.transform.Find("ScrollRectImage").gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        Debug.Log(originalQuestPanel.name);

        origQuestPanelPos = originalQuestPanel.transform.position;

        parentOfOriginalQuestPanel = originalQuestPanel.transform.parent.gameObject;

        originalPanelName = activeQuestPanelPrefab.name;

        
        // After we get all the data needed from The quest panel we destroy it to clear out active quest panel list in the GUI
        // otherwise we will always have one panel there at start of the scene
        Destroy(originalQuestPanel);


        dataToPass = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        activeQuests = dataToPass.ActiveQuests;
        Close();
    }

    public override void Open()
    {
        base.Open();
        // TODO: Load in quests
        foreach (var item in activeQuests.quests)
        {
            CreateActiveQuestPanel(item);
        }
    }
    public override void Close()
    {
        foreach (Transform item in parentOfOriginalQuestPanel.transform)
        {
            Debug.Log("Name of item gameobj: "+item.gameObject); // TODO: REMOVE QUEST OBJECTS PANELS
            Destroy(item.gameObject);
        }
        base.Close();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Debug.Log("hejsan");
        //    Open();
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    //ActiveQuests testActiveQuest = dataToPass.ActiveQuests;
        //    //foreach (var item in testActiveQuest.quests)
        //    //{
        //    //    //CreateActiveQuestPanel(item);
        //    //    Debug.Log(item.QuestName);
        //    //}
        //    Debug.Log(dataToPass.ActiveQuests.quests);
        //}
    }

    private void CreateActiveQuestPanel(Quest quest)
    {
        GameObject questPanelCopy = GameObject.Instantiate(activeQuestPanelPrefab) as GameObject;
        questPanelCopy.name = originalPanelName;

        questPanelCopy.transform.SetParent(parentOfOriginalQuestPanel.transform);

        questPanelCopy.transform.position = new Vector3(origQuestPanelPos.x, (origQuestPanelPos.y + distanceToNextQuestPanel), 0);

        // Scale needs to be set otherwise the questpanel gets scaled down to 0.04 instead of the regular 1.00
        questPanelCopy.transform.localScale = new Vector3(1, 1, 1);


        questPanelText = questPanelCopy.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        questPanelText.text = quest.QuestName;
    }
}
