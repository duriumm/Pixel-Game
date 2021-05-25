using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIScreenType 
{ 
    Inventory,
    Stats,
    Shop,
}

public class UIScreenManager : MonoBehaviour
{
    StatsScreen stats;
    InventoryScreen inventory;
    ShopScreen shop;
    UIScreen activeScreen;
    GameObject tabs;
    Attack playerAttack;

    public StatsScreen Stats => stats;
    public InventoryScreen Inventory => inventory;
    public ShopScreen Shop => shop;

    void Start()
    {
        stats = gameObject.transform.Find("StatsScreen").gameObject.GetComponent<StatsScreen>();
        inventory = gameObject.transform.Find("InventoryScreen").gameObject.GetComponent<InventoryScreen>();
        shop = gameObject.transform.Find("ShopScreen").gameObject.GetComponent<ShopScreen>();
        tabs = gameObject.transform.Find("GuiTabsButtons").gameObject;
        tabs.SetActive(false);
        playerAttack = GameObject.Find("MyCharacter").GetComponent<Attack>();
    }

    //Return false if already open
    bool Open(UIScreen screen)
    {
        if (screen != activeScreen)
        {
            if (activeScreen != null)
                activeScreen.Close();
            activeScreen = screen;
            activeScreen.Open();
            if (screen != shop)
                tabs.SetActive(true);
            else
                inventory.Open();
            playerAttack.enabled = false;
            return true;
        }
        return false;
    }

    public void Toggle(UIScreenType type)
    {
        if (!Open(type))
        {
            //Screen was already open so close it
            Close(type);
        }
    }

    public void Close(UIScreenType type)
    {
        tabs.SetActive(false);
        activeScreen.Close();
        if (activeScreen == shop)
            inventory.Close();
        activeScreen = null;
        playerAttack.enabled = true;
    }

    public bool Open(UIScreenType type)
    {
        return Open(GetScreen(type));
    }

    public void OpenStats()
    {
        Open(stats);
    }

    public void OpenInventory()
    {
        Open(inventory);
    }

    public void OpenShop()
    {
        Open(shop);
    }

    UIScreen GetScreen(UIScreenType type)
    {
        switch (type)
        {
            case UIScreenType.Inventory:
                return inventory;
            case UIScreenType.Stats:
                return stats;
            case UIScreenType.Shop:
                return shop;
            default:
                throw new NotImplementedException();
        }
    }

    public bool IsOpen(UIScreenType type)
    {
        return activeScreen == GetScreen(type);
    }
}
    
