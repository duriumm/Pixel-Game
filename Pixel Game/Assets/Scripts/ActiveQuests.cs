using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuests
{
    // Made this public otherwise cant access all quests // L
    public List<Quest> quests = new List<Quest>();
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

    public void TryIncrementItemsCollected(string itemName)
    {
        foreach (var quest in quests)
            quest.TryIncrementItemsCollected(itemName);
    }

    public void TryDecrementItemsCollected(string itemName)
    {
        foreach (var quest in quests)
            quest.TryDecrementItemsCollected(itemName);
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

    public void TryIncrementKilledEnemies(string name)
    {
        foreach (var quest in quests)
            quest.TryIncrementKilledEnemies(name);
    }
}
