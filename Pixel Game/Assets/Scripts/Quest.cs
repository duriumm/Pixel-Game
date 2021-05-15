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

    public enum ENEMYTYPE
    {
        None,
        GHOST,
        HUMAN,
        BEAST
    }
    public ENEMYTYPE enemyTypeToKill = ENEMYTYPE.None;


    public string questName;
    [TextArea(2,5)]
    public string description;

    public int moneyReward;
    public GameObject gameObjectReward;
    private PlayerInventory playerInventory;

    public bool isQuestFinished = false;
    // Kill enemies quest
    [Header("Kill enemies quest")]
    public int amountToKill;
    public int currentKilled;


    // Gather items quest
    [Header("Gather items quest")]
    public GameObject itemToGather;
    public int amountToCollect;
    public int currentCollected;

    // Item delivery quest
    [Header("Item delivery quest")]
    public GameObject itemToDeliver;
    public GameObject npcDeliveryTarget;

    void Start()
    {
        playerInventory = GameObject.FindWithTag("InventoryManager").GetComponent<PlayerInventory>();
    }


    //public void DeliverAndDestroyItem(GameObject gameObjectToDestroy)
    //{
    //    // TODO: delete item from player inventory 
    //    playerInventory.
    //    isQuestFinished = true;
    //}
    public void IncrementItemsCollected()
    {
        currentCollected++;
        if(currentCollected >= amountToCollect)
        {
            isQuestFinished = true;
        }
    }
    public void DecrementItemsCollected()
    {
        if(currentCollected > 0)
        {
            currentCollected--;
            if (currentCollected < amountToCollect)
            {
                isQuestFinished = false;
            }
        }  
    }
    public void IncrementKilledEnemies()
    {

        currentKilled++;
        if(currentKilled >= amountToKill)
        {
            isQuestFinished = true;
        }
    }




}
