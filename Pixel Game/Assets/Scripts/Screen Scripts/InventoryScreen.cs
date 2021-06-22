using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : GuiScreen
{
    private InventorySlot[] inventorySlots;
    public InventorySlot[] InventorySlots => inventorySlots;
    private InventorySlot[] equipmentSlots;
    public InventorySlot[] EquipmentSlots => equipmentSlots;

    private void Awake()
    {
        inventorySlots = transform.GetChild(0).transform.GetComponentsInChildren<InventorySlot>();
        equipmentSlots = transform.GetChild(1).transform.GetComponentsInChildren<InventorySlot>();
    }

    private void Start()
    {
        PlaceEquipmentSlots();
        Close();
    }

    public override void Close()
    {
        foreach (var item in inventorySlots)
            item.RemoveDataShowingOnExit();
        foreach (var item in equipmentSlots)
            item.RemoveDataShowingOnExit();
        base.Close();
    }


    private void PlaceEquipmentSlots()
    {
        float marginX = 1.4f;
        float marginY = 0.9f;
        var equipmentSlotsContainer = GameObject.Find("EquipmentSlots");
        int stride = 3;

        //Loop through slots and place them in a grid
        for (int i = 0; i < equipmentSlotsContainer.transform.childCount; i++)
        {
            var transform = equipmentSlotsContainer.transform.GetChild(i);
            var rectTransform = transform.GetComponent<RectTransform>();

            rectTransform.anchorMax = rectTransform.anchorMin = rectTransform.pivot = new Vector2(0.5f, 1);
            int row = i / stride;
            int column = (i % stride) - 1;
            float x = (rectTransform.rect.width + marginX) * column;
            float y = -(rectTransform.rect.height + marginY) * row;

            if (i == 9)  //Center last slot
                x = 0;
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}
