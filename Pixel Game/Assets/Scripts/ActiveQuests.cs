using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuests
{
    List<Quest> quests = new List<Quest>();
    public void Add(Quest quest)
    {
        quests.Add(quest);
        quest.Activate();
    }

    public void Remove(Quest quest)
    {
        quests.Remove(quest);
        quest.End();
    }

    public bool TryIncrementItemsCollected(string itemName)
    {
        foreach (var quest in quests)
        {
            if (quest.TryIncrementItemsCollected(itemName))
                return true;
        }
        return false;
    }
    public bool TryDecrementItemsCollected(string itemName)
    {
        foreach (var quest in quests)
        {
            if (quest.TryDecrementItemsCollected(itemName))
                return true;
        }
        return false;
    }

    public bool TryDeliverItem(string npcName, string itemName)
    {
        foreach (var quest in quests)
        {
            if (quest.TryDeliverItem(npcName, itemName))
                return true;
        }
        return false;
    }

    public bool TryIncrementKilledEnemies(string name)
    {
        foreach (var quest in quests)
        {
            if (quest.TryIncrementKilledEnemies(name))
                return true;
        }
        return false;
    }
}
