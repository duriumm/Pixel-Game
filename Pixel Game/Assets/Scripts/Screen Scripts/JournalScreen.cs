using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalScreen : GuiScreen
{
    public GameObject activeQuestPanelPrefab;
    private GameObject startingActiveQuestPanel;
    private GameObject questListCanvas;
    private GameObject itemRewardCanvas;

    private GameObject startingItemReward;
    public GameObject StartingItemReward => startingItemReward;
    private float activeQuestPanelHeight = 5.17f;
    private Vector3 origQuestPanelPos;

    private Vector3 origItemRewardPos;
    public Vector3 OrigItemRewardPos => origItemRewardPos;
    string originalPanelName;
    ActiveQuests activeQuests;

    DataToPassBetweenScenes dataToPass;
    private TextMeshProUGUI questPanelText;
    private TextMeshProUGUI questDescriptionText;
    private TextMeshProUGUI goldRewardText;

    void Start()
    {
        // Get the ActiveQuestPanel gameobject that is there at start so we can gather position from it
        startingActiveQuestPanel = transform.Find("ScrollRectImage/QuestListCanvas/ActiveQuestPanel").gameObject;
        startingItemReward = transform.Find("QuestDescription/ItemRewardBG/ItemRewardCanvas/ItemRewardSprite").gameObject;
        
        origQuestPanelPos = startingActiveQuestPanel.transform.position;
        origItemRewardPos = startingItemReward.transform.position;


        questListCanvas = startingActiveQuestPanel.transform.parent.gameObject;
        itemRewardCanvas = startingItemReward.transform.parent.gameObject;

        originalPanelName = activeQuestPanelPrefab.name;

        // After we get all the data needed from The quest panel we destroy it to clear out active quest panel list in the GUI
        // otherwise we will always have one panel there at start of the scene
        Destroy(startingActiveQuestPanel);
        Destroy(startingItemReward);

        dataToPass = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        activeQuests = dataToPass.ActiveQuests;



        // Long way to reset the gold amount when opening questscreen
        questDescriptionText = transform.Find("QuestDescription").gameObject.GetComponent<TextMeshProUGUI>();
        goldRewardText = transform.Find("QuestDescription/MoneyRewardBG/MoneyRewardCanvas/AmountOfMoneyText").GetComponent<TextMeshProUGUI>();
        goldRewardText.text = "";

        Close();
    }

    // For each quest we have in the list we add a height of 5.17f to our QuestListCanvas
    // So that the scrollable field is dynamic. Longer for more quests, shorter and unscrollable 
    // when the amount of quests does not reach outside QuestListCanvas field
    private float calculateQuestListCanvasHeight()
    {
        return activeQuestPanelHeight * activeQuests.Count;
    }
    public override void Open()
    {
        base.Open();
        foreach (var quest in activeQuests)
        {
            CreateActiveQuestPanel(quest);
        }
        questDescriptionText.text = "Press any of your active quests to see the quest description along with attached rewards";

        // Set the Height of QuestListCanvas
        questListCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculateQuestListCanvasHeight());

    }
    public override void Close()
    {
        foreach (Transform item in questListCanvas.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in itemRewardCanvas.transform)
        {
            Destroy(item.gameObject);
        }
        base.Close();
    }

    private void CreateActiveQuestPanel(Quest quest)
    {
        GameObject questPanelCopy = GameObject.Instantiate(activeQuestPanelPrefab) as GameObject;
        questPanelCopy.name = originalPanelName;
        questPanelCopy.transform.SetParent(questListCanvas.transform);
        questPanelCopy.transform.position = new Vector3(origQuestPanelPos.x, (origQuestPanelPos.y + activeQuestPanelHeight), 0);

        // Scale needs to be set otherwise the questpanel gets scaled down to 0.04 instead of the regular 1.00
        questPanelCopy.transform.localScale = new Vector3(1, 1, 1);

        questPanelText = questPanelCopy.transform.Find("QuestPanelText").GetComponent<TextMeshProUGUI>();
        questPanelText.text = quest.QuestName;

        // Send quest data into button gameobject
        questPanelCopy.GetComponent<ActiveQuestButton>().buttonsActiveQuest = quest;
    }
}
