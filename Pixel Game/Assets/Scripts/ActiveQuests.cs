using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuests : IEnumerable<Quest>
{
    private List<Quest> quests = new List<Quest>();

    public float Count => quests.Count;

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

    //Lets you use ActiveQuests objects in foreach loops
    public IEnumerator<Quest> GetEnumerator()
    {
        return quests.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    // Provides access to individual quests, just in case it will be needed
    // Example: var quest = activeQuests[0];
    public Quest this[int index] => quests[index];
}
