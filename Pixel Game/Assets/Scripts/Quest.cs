using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public enum QUESTTYPE
    {
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

    public int amountToKill;

    public int currentKilled;

    public bool isQuestFinished = false;


    public void IncrementKilledEnemies()
    {

        currentKilled++;
        if(currentKilled >= amountToKill)
        {
            isQuestFinished = true;
        }
    }




}
