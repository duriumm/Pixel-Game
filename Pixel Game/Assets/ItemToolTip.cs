using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemToolTip : MonoBehaviour
{
    private Attack playerAttack;

    private void Awake()
    {
        // We need this to run as soon as "itemRewardCopy" is instantiated in ActiveQuestButton.
        // So we use Awake instead of Start.
        var playerCharacter = GameObject.FindGameObjectWithTag("MyPlayer");
        playerAttack = playerCharacter.GetComponent<Attack>();
    }

    public void UpdateContent(ItemData itemData)
    {
        if (itemData == null)
            return;

        Debug.Log(transform.parent.name);
        TextMeshProUGUI itemNameText = transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
        itemNameText.text = itemData.itemName;
        TextMeshProUGUI itemDescriptionText = transform.Find("ItemDescriptionText").GetComponent<TextMeshProUGUI>();
        itemDescriptionText.text = itemData.description;
        TextMeshProUGUI itemStatsText = transform.Find("ItemStatsText").GetComponent<TextMeshProUGUI>();
        itemStatsText.text = "";
        if (itemData.itemType == ItemData.ITEMTYPE.WEAPON)
        {
            ShowWeaponStats(itemData, itemStatsText);
        }
        else if (itemData.isEquippable)
        {
            ShowEquipmentStats(itemData, itemStatsText);
        }
        else if (itemData.itemType == ItemData.ITEMTYPE.EDIBLE)
        {   // Show the text in green to indicate hp gain on eating item
            itemStatsText.text = "Effect on eating: " + "<color=green>+" + itemData.healingCapability + " hp</color>";
        }
        else if (itemData.itemType == ItemData.ITEMTYPE.QUEST_ITEM)
        {
            itemStatsText.text = null;
        }
        if (itemData.value > 0)
            itemStatsText.text += $"Value: <color=yellow>{itemData.value} coins</color> ";
    }

    private void ShowEquipmentStats(ItemData itemData, TextMeshProUGUI itemStatsText)
    {
        string defenseDiff = "";
        bool showDefense = itemData.defense > 0;
        //If this is not an equipment slot, show difference with currently equipped item
        if (transform.parent.name != InventorySlot.GetEquipmentSlotNameForItemType(itemData))
        {
            var equipmentSlot = InventorySlot.GetEquipmentSlotForItemType(itemData);
            int equippedDefense = equipmentSlot.ItemDataInSlot != null ?
                equipmentSlot.ItemDataInSlot.defense : 0;
            if (equippedDefense > 0 || itemData.defense > 0)
            {
                showDefense = true;
                defenseDiff = GetHoverDiffText(itemData.defense - equippedDefense);
            }
        }
        itemStatsText.text = $"Defense: {itemData.defense} {defenseDiff}\n";
    }

    private void ShowWeaponStats(ItemData itemData, TextMeshProUGUI itemStatsText)
    {
        var weapon = itemData.GetComponent<Weapon>();
        string powerDiff = "", cooldownDiff = "", projectileSpeedDiff = "";

        //If this is not an equipment slot, show difference with currently equipped weapon
        if (transform.parent.name != InventorySlot.GetEquipmentSlotNameForItemType(itemData))
        {
            powerDiff = GetHoverDiffText(weapon.Damage - playerAttack.CurrentWeapon.Damage);
            cooldownDiff = GetHoverDiffText(weapon.Cooldown - playerAttack.CurrentWeapon.Cooldown, true);
            if (weapon.HasProjectileAttack && playerAttack.CurrentWeapon.HasProjectileAttack)
                projectileSpeedDiff = GetHoverDiffText(weapon.ProjectileAttack.Speed - playerAttack.CurrentWeapon.ProjectileAttack.Speed);
        }

        itemStatsText.text = $"Damage: {weapon.Damage} {powerDiff}\n";
        itemStatsText.text += $"Cooldown: {weapon.Cooldown}s {cooldownDiff}\n";
        if (weapon.HasProjectileAttack)
            itemStatsText.text += $"Projectile speed: {weapon.ProjectileAttack.Speed} {projectileSpeedDiff}\n";
    }

    // Returns a string describing the difference between the hovered item
    // and the currently equipped item
    // For example, if we have equipped a sword with power 10,
    // and the weapon we are hovering on has power 15, the text will be "+5" in green
    private string GetHoverDiffText(float diff, bool reverseColor = false)
    {
        string positive = "green";
        string negative = "#ff7070";
        string neutral = "white";
        string color;
        string sign;
        if (diff > 0)
        {
            color = reverseColor ? negative : positive;
            sign = "+";
        }
        else if (diff == 0)
        {
            color = neutral;
            sign = "+/-";
        }
        else
        {
            color = reverseColor ? positive : negative;
            sign = "-";
        }
        return $"<color={color}> {sign}{Math.Abs(diff)}</color>";
    }

    public void Open()
    {
        GetComponent<CanvasGroup>().alpha = 1;
    }

    public void Close()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }
}
