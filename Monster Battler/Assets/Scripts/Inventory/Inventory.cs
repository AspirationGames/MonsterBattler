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

    public List<ItemSlot> SetCurrentItemSlots(int itemCategoryIndex)
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

    public void AddItem(ItemBase item, int addedQuantity=1)
    {
        int itemCategoryIndex = (int)GetItemCategory(item);
        SetCurrentItemSlots(itemCategoryIndex);
        
        var itemSlot = currentItemSlots.FirstOrDefault(slot => slot.Item == item); //checks if an item slot alrady exist for the item being added

        if(itemSlot != null)
        {
            IncreaseQuantity(item, addedQuantity);
        }
        else
        {
            currentItemSlots.Add(new ItemSlot()
            {
                Item = item,
                Quantity = addedQuantity
                
            }
            );
        }

        InventoryUpdated?.Invoke();
        

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

    public void IncreaseQuantity(ItemBase item, int addedQuantity=1)
    {
        var itemSlot = currentItemSlots.First(slot => slot.Item == item);

        itemSlot.Quantity += addedQuantity;

        InventoryUpdated?.Invoke();
    }

    public ItemCategory GetItemCategory(ItemBase item)
    {
        if(item is RecoveryItem)
        {
            return ItemCategory.RecoveryItems;
        }
        else if(item is BindingCrystal)
        {
            return ItemCategory.BindingCrystals;
        }
        else if(item is SpellItem)
        {
            return ItemCategory.Spells;
        }

        Debug.Log("Item class is not accounted for in Get Item Category fucntion");
        return ItemCategory.RecoveryItems;

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

    public ItemSlot()
    {
        
    }
    public ItemSlot(ItemSaveData saveData) //initializer to reload item data
    {
        item = ItemDB.GetItemByName(saveData.sItemName);
        quantity = saveData.sQuantity;
    }

    public ItemBase Item { get => item; set => item = value;}
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
