using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> itemSlots;

    public event Action InventoryUpdated;

    public List<ItemSlot> ItemSlots => itemSlots;
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public ItemBase UseItem(ItemSlot selectedItemSlot, Monster selectedMonster)
    {
        //first make sure selectedItemSlot still has quanity in the event the player continue to use

        if(selectedItemSlot.Quantity == 0)
        {
            return null;
        }

        var item = selectedItemSlot.Item;
        bool itemUsed = item.Use(selectedMonster);

        if(itemUsed)
        {
            RemoveItem(item);
            return item; //we return the item to be able to include it in dialogue
        }

        return null;

    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = ItemSlots.First(slot => slot.Item == item);

        itemSlot.Quantity--;

        if(itemSlot.Quantity == 0)
        {
            itemSlots.Remove(itemSlot);
        }

        InventoryUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int quantity;

    public ItemBase Item => item;
    public int Quantity
    {
       get  => quantity;
       set  => quantity = value;


    }

    

}
