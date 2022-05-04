using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryScreen : MonoBehaviour
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDesciption;
    
    Inventory inventory;

    private void Awake() 
    {
        
        inventory = Inventory.GetInventory();
    }
    
    private void Start() 
    {
        UpdateItemList();
        ItemSlotUI.Hover += ItemSelected;
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

    public void ItemSelected(ItemSlotUI selectedItemSlotUI)
    {

        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex(); //this might be possible with just a gameobject too
        var item = inventory.ItemSlots[selectedItemIndex].Item;
        itemIcon.sprite = item.Icon;
        itemDesciption.text = item.Description;
          
    }

    public void OnBack()
    {
        GameController.Instance.CloseInventoryScreen();

    }

}
