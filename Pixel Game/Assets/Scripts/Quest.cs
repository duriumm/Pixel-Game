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
        WAIT_TIME
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

    // Kill enemies quest
    public int amountToKill;
    public int currentKilled;

    public bool isQuestFinished = false;

    // Gather items quest
    public GameObject itemToGather;
    public int amountToCollect;
    public int currentCollected;



    public void ClearCollectedItems(GameObject gameObjToClear)
    {

    }
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
