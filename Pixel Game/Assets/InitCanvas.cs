using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCanvas : MonoBehaviour
{
    private void Awake()
    {
        PlaceEquipmentSlots();
    }

    private void PlaceEquipmentSlots()
    {
        float margin = 1;
        var equipmentSlotsContainer = GameObject.Find("EquipmentSlots");
        
        int stride = 3;

        //Loop through slots and place them in a grid
        for (int i = 0; i < equipmentSlotsContainer.transform.childCount; i++)
        {
            var transform = equipmentSlotsContainer.transform.GetChild(i);
            var rectTransform = transform.GetComponent<RectTransform>();

            //rectTransform.anchorMax = rectTransform.anchorMin = new Vector2(0.5f, 1);
            int row = i / stride - 1;
            int column = (i % stride) - 1;
            float x = (rectTransform.rect.width + margin) * column;
            float y = -(rectTransform.rect.height + margin) * row - margin + rectTransform.rect.height;

            if (i == 9) //Center last slot
                x = 0;

            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}
