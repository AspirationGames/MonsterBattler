using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory{RecoveryItems, BindingCrystals, Spells}
public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> recoveryItemSlots;
    [SerializeField] List<ItemSlot> crystalSlots;
    [SerializeField] List<ItemSlot> spellSlots; //TMs and HMs

    List<List<ItemSlot>> itemSlots;

    List<ItemSlot> currentItemSlots;
    public event Action InventoryUpdated;

    private void Awake() 
    {
        itemSlots = new List<List<ItemSlot>>(){recoveryItemSlots, crystalSlots, spellSlots};    
    }

    public static List<string> ItemCategories {get; set;} = new List<string>()
    {
        "RECOVERY ITEMS", "BINDING CRYSTALS", "SPELL BOOKS & SCROLLS"
    };

    public List<ItemSlot> currentItemSlotsCategory(int itemCategoryIndex)
    {
        currentItemSlots = itemSlots[itemCategoryIndex];
        return currentItemSlots;
    } 
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public bool CanUseItem(ItemSlot selectedItemSlot, Monster selectedMonster)
    {

        if(selectedItemSlot.Quantity == 0)
        {
            return false;
        }

        var item = selectedItemSlot.Item;
        return item.CanUse(selectedMonster); //checks if you can use the item on the selected monster

    }

    public ItemBase UseItem(ItemSlot selectedItemSlot, Monster selectedMonster)
    {

        var item = selectedItemSlot.Item;
        item.Use(selectedMonster);

        return item;
        

    }

    public void DecreaseQuantity(ItemBase item)
    {
        var itemSlot = currentItemSlots.First(slot => slot.Item == item);

        itemSlot.Quantity--;

        if(itemSlot.Quantity == 0)
        {
            currentItemSlots.Remove(itemSlot);
        }

        InventoryUpdated?.Invoke();
    }

    public void IncreaseQuantity(ItemBase item)
    {
        var itemSlot = currentItemSlots.First(slot => slot.Item == item);

        if(itemSlot.Quantity == 0)
        {
            currentItemSlots.Add(itemSlot);
        }
        else
        {
            itemSlot.Quantity++; //note need to figure out what to do if you get more than a single unit of an item.
        }

        InventoryUpdated?.Invoke();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData
        {
            sRecoveryItems = recoveryItemSlots.Select(i => i.GetSaveData()).ToList(),
            sCrystals = crystalSlots.Select(i => i.GetSaveData()).ToList(),
            sSpells = spellSlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (InventorySaveData)state;

        recoveryItemSlots = saveData.sRecoveryItems.Select(i => new ItemSlot(i)).ToList();
        crystalSlots = saveData.sCrystals.Select(i => new ItemSlot(i)).ToList();
        spellSlots = saveData.sSpells.Select(i => new ItemSlot(i)).ToList();

        itemSlots = new List<List<ItemSlot>>(){recoveryItemSlots, crystalSlots, spellSlots};

        InventoryUpdated?.Invoke();
    }

}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int quantity;

    public ItemSlot(ItemSaveData saveData) //initializer to reload item data
    {
        item = ItemDB.GetItemByName(saveData.sItemName);
        quantity = saveData.sQuantity;
    }

    public ItemBase Item => item;
    public int Quantity
    {
       get  => quantity;
       set  => quantity = value;
    }

    public ItemSaveData GetSaveData() //get Item Slot save data
    {
        var saveData = new ItemSaveData()
        {
            sItemName = item.ItemName,
            sQuantity = quantity
        };

        return saveData;
    }
    

}

[Serializable]
public class ItemSaveData
{
    public string sItemName;
    public int sQuantity;
}


[Serializable]
public class InventorySaveData //list of our item save data. We should have one per category
{
    public List<ItemSaveData> sRecoveryItems;
    public List<ItemSaveData> sCrystals;
    public List<ItemSaveData> sSpells;

}
