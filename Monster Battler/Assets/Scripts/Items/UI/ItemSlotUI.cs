using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemQuantityText; 

    public static event Action<ItemSlotUI> itemUIHover;

    public static event Action<ItemSlotUI> itemUISelected;

    public void SetData(ItemSlot itemSlot)
    {

        itemNameText.text = itemSlot.Item.ItemName;
        itemQuantityText.text = $"x {itemSlot.Quantity}";
    }

    public void SetNameAndPrice(ItemBase item) //used for Shop UI
    {
        itemNameText.text = item.ItemName;
        itemQuantityText.text = $"$ {item.BuyPrice}";
    }

    public void OnHover()
    {
        itemUIHover?.Invoke(this);
    }

    public void OnSelect()
    {
        itemUISelected?.Invoke(this);

    }


}
