using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public string questName;
    private string description;
    private int moneyReward;
    public Quest(string questName, string description, int moneyReward)
    {
        this.questName = questName;
        this.description = description;
        this.moneyReward = moneyReward;
    }

}
