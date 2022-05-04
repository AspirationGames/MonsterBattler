using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum InventoryScreenState{Inventory, PartyScreen, Busy}
public class InventoryScreen : MonoBehaviour
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDesciption;
    
    [SerializeField] PartyScreen partyScreen;
    Inventory inventory;

    InventoryScreenState inventoryScreenState;
    
    ItemSlot selectedItemSlot; 

    private void Awake() 
    {
        
        inventory = Inventory.GetInventory();
    }
    
    private void Start() 
    {
        UpdateItemList();
        ItemSlotUI.itemUIHover += ItemHover;
        ItemSlotUI.itemUISelected += ItemSelected;
        inventory.InventoryUpdated += UpdateItemList;
        partyScreen.monsterSelected += PartyMemberSelected; //monster selected from party screen
        partyScreen.screenClosed += PartyScreenClosed;
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
        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex(); //sets the selected item for use.
        selectedItemSlot = inventory.ItemSlots[selectedItemIndex];

        OpenPartyScreen(); //Open party screen to select monster to use item on
        inventoryScreenState = InventoryScreenState.PartyScreen;
        
    }

    public void PartyMemberSelected(Monster selectedMonster)
    {
        if(inventoryScreenState != InventoryScreenState.PartyScreen)
        {
            return;
        }
        
        StartCoroutine(UseItem(selectedMonster));
    }

    IEnumerator UseItem(Monster selectedMonster)
    {
        inventoryScreenState = InventoryScreenState.Busy;

        var usedItem = inventory.UseItem(selectedItemSlot, selectedMonster); //this method uses the item but also returns the item used from inventory script

        if(usedItem != null)
        {
           yield return DialogManager.Instance.ShowDialogText($"You used a {usedItem.ItemName}.");
        }
        else if(selectedItemSlot.Quantity == 0)
        {
            yield return DialogManager.Instance.ShowDialogText($"You are out of {selectedItemSlot.Item.ItemName}.");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{selectedItemSlot.Item.ItemName} won't have any effect on {selectedMonster.Base.MonsterName}.");
        }

        inventoryScreenState = InventoryScreenState.PartyScreen; //I want the player to remain on party screen and can continue to use items if they would like.
        
    }

    public void OpenPartyScreen()
    {
        GameController.Instance.ShowPartyScreen();

    }

    public void PartyScreenClosed()
    {
        inventoryScreenState = InventoryScreenState.Inventory;
    }

    public void OnBack()
    {
        GameController.Instance.CloseInventoryScreen();

    }

}
