using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveQuestButton : MonoBehaviour
{
    public Quest buttonsActiveQuest;
    private TextMeshProUGUI questDescriptionObjText;
    private TextMeshProUGUI amountOfGoldReward;
    private GameObject questDescriptionObj;

    JournalScreen journalScreen;
    public GameObject prefabItemReward;

    private Transform itemRewardCanvasTransform;

    void Start()
    {
        journalScreen = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(3).GetComponent<JournalScreen>();

        questDescriptionObj = journalScreen.transform.Find("QuestDescription").gameObject;

        questDescriptionObjText = questDescriptionObj.GetComponent<TextMeshProUGUI>();

        amountOfGoldReward = questDescriptionObj.transform.Find("MoneyRewardBG").
            gameObject.transform.Find("MoneyRewardCanvas").transform.Find("AmountOfMoneyText").GetComponent<TextMeshProUGUI>();

        itemRewardCanvasTransform = questDescriptionObj.transform.GetChild(0).transform.GetChild(0);
    }

    private void ClearItemRewards()
    {
        foreach (Transform item in itemRewardCanvasTransform)
        {
            Destroy(item.gameObject);
        }
    }

    // TODO: Make it possible to instantiate several items, not just one (use a for loop)
    public void ShowQuestDataOnClick()
    {
        
        ClearItemRewards();
        questDescriptionObjText.text = buttonsActiveQuest.Description;
        amountOfGoldReward.text = buttonsActiveQuest.MoneyReward.ToString();

        if(buttonsActiveQuest.GameObjectReward != null)
        {
            GameObject itemRewardCopy = GameObject.Instantiate(prefabItemReward) as GameObject;
            itemRewardCopy.name = prefabItemReward.name;
            itemRewardCopy.transform.SetParent(questDescriptionObj.transform.GetChild(0).transform.GetChild(0));
            itemRewardCopy.transform.position = new Vector3(journalScreen.OrigItemRewardPos.x, journalScreen.OrigItemRewardPos.y, 0);

            // Scale needs to be set otherwise the questpanel gets scaled down to 0.04 instead of the regular 1.00
            itemRewardCopy.transform.localScale = new Vector3(1, 1, 1);
            //itemRewardCopy.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);

            itemRewardCopy.GetComponent<Image>().sprite = buttonsActiveQuest.GameObjectReward.GetComponent<SpriteRenderer>().sprite;

            itemRewardCopy.transform.Find("ToolTipHoverCanvas").GetComponent<ItemToolTip>().UpdateContent(
                buttonsActiveQuest.GameObjectReward.GetComponent<ItemData>()
                );

            // Could not set values in inspector of prefab so had to set them in code here. Probably since we instantiate a new
            // object and set its parent etc etc. Im tired
            RectTransform itemRewardCopyRectTransform = itemRewardCopy.transform.GetChild(0).GetComponent<RectTransform>();
            itemRewardCopyRectTransform.anchorMin = new Vector2(0, 0);
            itemRewardCopyRectTransform.anchorMax = new Vector2(1, 1);
            itemRewardCopyRectTransform.pivot = new Vector2(0.5f, 0.5f);
            itemRewardCopyRectTransform.offsetMin = new Vector2(0, 0);
            itemRewardCopyRectTransform.offsetMax = new Vector2(0, 0);

            // Remove popup so it does not show after pressing the quest button
            itemRewardCopy.GetComponent<OnHover>().RemoveDataOnExit();

        }
    }






}
