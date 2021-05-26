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
    public QUESTTYPE questType;

    // Refers to Enemyhealth enumtype (to not make double enums)
    public EnemyHealth.ENEMYTYPE enemyTypeToKill = EnemyHealth.ENEMYTYPE.BEAST;

    public string questName;
    [TextArea(2,5)]
    public string description;

    public int moneyReward;
    public GameObject gameObjectReward;
    private PlayerInventory playerInventory;

    private bool isQuestFinished = false;
    public bool IsQuestFinished => isQuestFinished;
    // Kill enemies quest
    [Header("Kill enemies quest")]
    public int amountToKill;
    private int currentKilled;

    // Gather items quest
    [Header("Gather items quest")]
    public GameObject itemToGather;
    public int amountToCollect;
    private int currentCollected;

    // Item delivery quest
    [Header("Item delivery quest")]
    public GameObject itemToDeliver;
    public GameObject npcDeliveryTarget;

    void Start()
    {
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
    }

    public void IncrementItemsCollected(string itemName)
    {
        if (CorrectItem(itemName))
        {
            isQuestFinished = ++currentCollected >= amountToCollect;
            Debug.Log($"{currentCollected}/{amountToCollect} {itemName} collected");
        }
    }

    public void DecrementItemsCollected(string itemName)
    {
        if (CorrectItem(itemName))
        {
            isQuestFinished =  --currentCollected < amountToCollect;
            Debug.Log($"{currentCollected}/{amountToCollect} {itemName} collected");
        }
    }

    bool CorrectItem(string itemName)
    {
        return questType == Quest.QUESTTYPE.GATHER_ITEMS && 
            itemName == itemToGather.GetComponent<ItemData>().itemName;
    }

    public void IncrementKilledEnemies()
    {

        currentKilled++;
        if(currentKilled >= amountToKill)
        {
            isQuestFinished = true;
        }
    }

    public bool TryDeliverItem(string npcName)
    {
        if (npcName == npcDeliveryTarget.name)
        {
            isQuestFinished = true;
            return true;
        }
        return false;
    }
}
