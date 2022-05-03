using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : MonoBehaviour
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    
    Inventory inventory;

    private void Awake() 
    {
        inventory = Inventory.GetInventory();
    }
    
    private void Start() 
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        //Clear all existing items
        foreach(Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(ItemSlot itemSlot in inventory.ItemSlots)
        {
            var itemSlotUIObj = Instantiate(itemSlotUI, itemList.transform);

            itemSlotUIObj.SetData(itemSlot);

        }

    }
    public void OnBack()
    {
        GameController.Instance.CloseInventoryScreen();

    }

}
