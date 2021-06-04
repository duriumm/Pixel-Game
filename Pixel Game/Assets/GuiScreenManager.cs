using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuiScreenType 
{ 
    Inventory,
    Stats,
    Shop,
    Journal,
}

public class GuiScreenManager : MonoBehaviour
{
    StatsScreen stats;
    InventoryScreen inventory;
    ShopScreen shop;
    JournalScreen journal;
    GuiScreen activeScreen;
    GameObject tabs;
    Attack playerAttack;

    public StatsScreen Stats => stats;
    public InventoryScreen Inventory => inventory;
    public ShopScreen Shop => shop;
    public JournalScreen Journal => journal;

    void Awake()
    {
        stats = gameObject.transform.Find("StatsScreen").gameObject.GetComponent<StatsScreen>();
        inventory = gameObject.transform.Find("InventoryScreen").gameObject.GetComponent<InventoryScreen>();
        shop = gameObject.transform.Find("ShopScreen").gameObject.GetComponent<ShopScreen>();
        journal = gameObject.transform.Find("JournalScreen").gameObject.GetComponent<JournalScreen>();
        tabs = gameObject.transform.Find("GuiTabsButtons").gameObject;
        tabs.SetActive(false);
        playerAttack = GameObject.Find("MyCharacter").GetComponent<Attack>();
    }

    //Return false if already open
    bool Open(GuiScreen screen)
    {
        if (screen != activeScreen)
        {
            if (activeScreen != null)
                activeScreen.Close();
            activeScreen = screen;
            activeScreen.Open();
            if (screen == shop)
            {
                tabs.SetActive(false);
                //The right part of inventory screen is used together with shop
                inventory.Open(); 
            }
            else
            {
                tabs.SetActive(true);
            }
            playerAttack.enabled = false;
            return true;
        }
        return false;
    }

    public void Toggle(GuiScreenType type)
    {
        if (!Open(type))
        {
            //Screen was already open so close it
            Close(type);
        }
    }

    public void Close(GuiScreenType type)
    {
        tabs.SetActive(false);
        activeScreen.Close();
        if (activeScreen == shop)
            inventory.Close();
        activeScreen = null;
        playerAttack.enabled = true;
    }

    public bool Open(GuiScreenType type)
    {
        return Open(GetScreen(type));
    }

    GuiScreen GetScreen(GuiScreenType type)
    {
        switch (type)
        {
            case GuiScreenType.Inventory:
                return inventory;
            case GuiScreenType.Stats:
                return stats;
            case GuiScreenType.Shop:
                return shop;
            case GuiScreenType.Journal:
                return journal;
            default:
                throw new NotImplementedException();
        }
    }

    public bool IsOpen(GuiScreenType type)
    {
        return activeScreen == GetScreen(type);
    }

    
    //These parameter-less methods should be called when clicking on tab buttons
    
    public void OpenStats()
    {
        Open(stats);
    }

    public void OpenInventory()
    {
        Open(inventory);
    }
}
    
