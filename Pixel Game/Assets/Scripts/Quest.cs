using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public enum QUESTTYPE
    {
        None,
        KILL_ENEMIES,
        GATHER_ITEMS,
        WAIT_TIME,
        DELIVER_ITEM
    }
    [SerializeField]
    private QUESTTYPE questType;

    // Made GET for questName so i can reference it // L
    [SerializeField]
    private string questName;
    public string QuestName => questName;


    [TextArea(2,5)]
    [SerializeField]
    private string description;
    [SerializeField]
    private int moneyReward;
    public int MoneyReward => moneyReward;
    [SerializeField]
    private GameObject gameObjectReward;
    public GameObject GameObjectReward => gameObjectReward;

    private PlayerInventory playerInventory;
    private bool isQuestFinished = false;
    public bool IsQuestFinished => isQuestFinished;
    // Kill enemies quest
    [Header("Kill enemies quest")]
    [SerializeField]
    private string enemyToKill;
    [SerializeField]
    private int amountToKill;
    private int currentKilled;

    // Gather items quest
    [Header("Gather items quest")]
    [SerializeField]
    private GameObject itemToGather;
    [SerializeField]  
    private int amountToCollect;
    private int currentCollected;

    // Item delivery quest
    [Header("Item delivery quest")]
    [SerializeField]
    private GameObject itemToDeliver;
    [SerializeField]
    private GameObject npcDeliveryTarget;

    public void Init()
    {
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
    }

    public bool TryIncrementItemsCollected(string itemName)
    {
        if (IsCorrectItem(itemName))
        {
            isQuestFinished = ++currentCollected >= amountToCollect;
            Debug.Log($"{currentCollected}/{amountToCollect} {itemName}s collected");
            return true;
        }
        else
            return false;
    }

    public bool TryDecrementItemsCollected(string itemName)
    {
        if (IsCorrectItem(itemName))
        {
            isQuestFinished =  --currentCollected < amountToCollect;
            Debug.Log($"{currentCollected}/{amountToCollect} {itemName}s collected");
            return true;
        }
        return false;
    }

    bool IsCorrectItem(string itemName)
    {
        return questType == Quest.QUESTTYPE.GATHER_ITEMS && 
            itemName == itemToGather.GetComponent<ItemData>().itemName;
    }

    public bool TryIncrementKilledEnemies(string enemyName)
    {
        if (questType == QUESTTYPE.KILL_ENEMIES && enemyName.Contains(enemyToKill))
        {
            isQuestFinished = ++currentKilled >= amountToKill;
            Debug.Log($"{currentKilled}/{amountToKill} {enemyToKill}s killed.");
            return true;
        }
        return false;        
    }

    public bool TryDeliverItem(string npcName, string itemName)
    {
        if (questType == QUESTTYPE.DELIVER_ITEM)
        {
            isQuestFinished = npcName == npcDeliveryTarget.name
                && itemName == itemToDeliver.name;
            return true;
        }
        return false;
    }

    public void Activate()
    {
        if (questType == Quest.QUESTTYPE.GATHER_ITEMS)
        {
            // Count number of required items already in inventory
            playerInventory.CheckInventoryForCollectedItems(
                itemToGather.GetComponent<ItemData>().itemName);
        }
        else if (questType == Quest.QUESTTYPE.DELIVER_ITEM)
        {
            // Create item to deliver and place in inventory
            string originalItemName = itemToDeliver.name;
            GameObject instantiatedObj = GameObject.Instantiate(itemToDeliver) as GameObject;
            instantiatedObj.name = originalItemName;
            playerInventory.LootItem(instantiatedObj);
        }
        Debug.Log("Current active playerquest is: " + questName);
    }

    public void End()
    {
        if (questType == Quest.QUESTTYPE.GATHER_ITEMS)
        {
            // When a gather items quest is done we want to clear the inventory of said items
            string itemName = itemToGather.GetComponent<ItemData>().itemName;
            playerInventory.RemoveCollectedQuestItemsFromInventory(itemName, amountToCollect);
        }

        playerInventory.AddCoinAmount(moneyReward);

        if (gameObjectReward != null)
        {
            // To-Do: Check if inventory is full
            // Get original name of obj so we dont get (clone) when we instantiate
            string originalItemName = gameObjectReward.name;
            GameObject instantiatedObj = GameObject.Instantiate(gameObjectReward) as GameObject;
            instantiatedObj.name = originalItemName;

            // Add instantiated item to inventory and add text to reward window showing what item we got
            playerInventory.LootItem(instantiatedObj);
        }

    }
}
