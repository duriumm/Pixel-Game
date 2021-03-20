using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class CustomItem : MonoBehaviour
{
    public string itemName;
    public string description;
    public int damage;
    public int defense;
    public int durability;
    public Sprite itemIcon;
    public string itemIdString;
    public enum ITEMTYPE
    {
        WOOD,
        ROCK,
        SPECTRAL,
        EDIBLE
    }
    public ITEMTYPE itemType;

    public CustomItem(string itemName, string description, int damage, int defense, int durability, Sprite itemIcon, string itemIdString)
    {
        this.itemName = itemName;
        this.description = description;
        this.damage = damage;
        this.defense = defense;
        this.durability = durability;
        this.itemIcon = itemIcon;
        this.itemIdString = itemIdString;
}
}