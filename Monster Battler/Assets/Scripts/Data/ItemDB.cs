using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;
    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        var itemsArray = Resources.LoadAll<ItemBase>("");

        foreach(var item in itemsArray)
        {
            if(items.ContainsKey(item.ItemName))
            {
                Debug.Log($"Two items have the name {item.ItemName} unable to add duplicate, check game object named {item.name}");
            }

            items[item.ItemName] = item; //adds to a dictionary
        }

    }

    public static ItemBase GetItemByName(string itemName)
    {
        if(!items.ContainsKey(itemName))
        {
            Debug.Log($"item with name {itemName} doese not exist.");
            return null;
        }

        return items[itemName];
    }
}
