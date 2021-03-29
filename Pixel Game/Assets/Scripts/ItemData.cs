using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class ItemData : MonoBehaviour
{
    public string itemName;
    public string description;
    public int damage;
    public int defense;
    public int durability;
    public int value;
    public Sprite itemIcon;
    public string itemIdString;
    public int healingCapability;
    public enum ITEMTYPE
    {
        WOOD,
        ROCK,
        SPECTRAL,
        EDIBLE,
        MONEY,
        WEAPON,
        ARMOR
    }
    public ITEMTYPE itemType;

    public ItemData(string itemName, string description, int damage, int defense,
        int durability, int value, Sprite itemIcon, string itemIdString, int healingCapability)
    {
        this.itemName = itemName;
        this.description = description;
        this.damage = damage;
        this.defense = defense;
        this.durability = durability;
        this.value = value;
        this.itemIcon = itemIcon;
        this.itemIdString = itemIdString;
        this.healingCapability = healingCapability;
}
}