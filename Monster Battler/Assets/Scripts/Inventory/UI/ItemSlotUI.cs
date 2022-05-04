using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemQuantityText; 

    public static event Action<ItemSlotUI> Hover;

    public void SetData(ItemSlot itemSlot)
    {

        itemNameText.text = itemSlot.Item.ItemName;
        itemQuantityText.text = $"x {itemSlot.Quantity}";
    }

    public void OnHover()
    {
        Hover(this);

    }


}
