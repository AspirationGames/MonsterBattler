using System;
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
    
    [SerializeField] PartyScreen partyScreen;
    Inventory inventory;

    private void Awake() 
    {
        
        inventory = Inventory.GetInventory();
    }
    
    private void Start() 
    {
        UpdateItemList();
        ItemSlotUI.itemUIHover += ItemHover;
        ItemSlotUI.itemUISelected += ItemSelected;

        
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
        
        //Set Initial Item icon and descriptions
        var item = inventory.ItemSlots[0].Item;
        itemIcon.sprite = item.Icon;
        itemDesciption.text = item.Description;
    }

    public void ItemHover(ItemSlotUI hoverItemSlotUI)
    {

        int hoverItemIndex = hoverItemSlotUI.transform.GetSiblingIndex(); //this returns the index of the item slot
        var item = inventory.ItemSlots[hoverItemIndex].Item;
        itemIcon.sprite = item.Icon;
        itemDesciption.text = item.Description;
          
    }

    public void ItemSelected(ItemSlotUI selectedItemSlotUI)
    {
        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex(); //this returns the index of the item slot
        var item = inventory.ItemSlots[selectedItemIndex].Item;

        OpenPartyScreen(); 
    }

    public void OpenPartyScreen()
    {
        GameController.Instance.ShowPartyScreen();

    }

    public void OnBack()
    {
        GameController.Instance.CloseInventoryScreen();

    }

}
