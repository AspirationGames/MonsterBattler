using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemQuantityText; 

    public void SetData(ItemSlot itemSlot)
    {

        itemNameText.text = itemSlot.Item.ItemName;
        itemQuantityText.text = $"x {itemSlot.Quantity}";
    }
}
