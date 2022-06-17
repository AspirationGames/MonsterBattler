using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScreen : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDesciption;
    List<ItemBase> availableItems;

    public event Action<ItemBase> onItemBought;

    public void ShowShopUI(List<ItemBase> availableItems)
    {
        gameObject.SetActive(true);
        this.availableItems = availableItems;
        UpdateItemList();

    }

    private void OnEnable() 
    {
        ItemSlotUI.itemUIHover += ItemHover;
        ItemSlotUI.itemUISelected += ItemSelected;    
    }

    private void OnDisable() 
    {
        ItemSlotUI.itemUIHover -= ItemHover;
        ItemSlotUI.itemUISelected -= ItemSelected;    
    }

    void UpdateItemList()
    {
        
        //Clear all existing items
        foreach(Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(ItemBase item in availableItems)
        {
            var itemSlotUIObj = Instantiate(itemSlotUI, itemList.transform);

            itemSlotUIObj.SetNameAndPrice(item);

        }
        
        //Set Initial Item icon and descriptions
        if(availableItems.Count > 0)
        {
            //Activate objects
            itemIcon.gameObject.SetActive(true);
            itemDesciption.gameObject.SetActive(true);

            var item = availableItems[0];
            itemIcon.sprite = item.Icon;
            itemDesciption.text = item.Description;
        }
        else
        {
            //Deactivate objects
            itemIcon.gameObject.SetActive(false);
            itemDesciption.gameObject.SetActive(false);
        }
        
    }

    public void ItemHover(ItemSlotUI hoverItemSlotUI)
    {

        int hoverItemIndex = hoverItemSlotUI.transform.GetSiblingIndex(); //this returns the index of the item slot
        var item = availableItems[hoverItemIndex];
        itemIcon.sprite = item.Icon;
        itemDesciption.text = item.Description;
          
    }

    public void ItemSelected(ItemSlotUI selectedItemSlotUI)
    {   
        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex();
        ItemBase selectedItem = availableItems[selectedItemIndex];
        onItemBought.Invoke(selectedItem);
    }

    public void CloseShopUI()
    {
        gameObject.SetActive(false);
    }
}
