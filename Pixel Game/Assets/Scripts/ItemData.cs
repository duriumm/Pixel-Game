using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class ItemData : MonoBehaviour
{
    public string itemName;
    public string description;
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
        ARMOR,
        HELMET,
        QUEST_ITEM
    }
    public ITEMTYPE itemType;
    public bool isEquippable;
    public bool shallEquipOnSceneChange;

    public ItemData(string itemName, string description, int defense,
        int durability, int value, Sprite itemIcon, string itemIdString, int healingCapability)
    {
        this.itemName = itemName;
        this.description = description;
        this.defense = defense;
        this.durability = durability;
        this.value = value;
        this.itemIcon = itemIcon;
        this.itemIdString = itemIdString;
        this.healingCapability = healingCapability;
}
}