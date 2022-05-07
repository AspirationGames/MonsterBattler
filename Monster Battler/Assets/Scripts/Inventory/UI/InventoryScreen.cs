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
    [SerializeField] TextMeshProUGUI itemCategoryText;
    
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;
    Inventory inventory;

    InventoryScreenState inventoryScreenState;
    public InventoryScreenState InventoryScreenState => inventoryScreenState;
    
    ItemSlot selectedItemSlot;

    int selectedCategoryIndex = 0;

    List<ItemSlot> currentItemSlots;

    private void Awake() 
    {
        
        inventory = Inventory.GetInventory();
    }
    
    private void Start() 
    {
        SetItemCategory(0); //sets initial item category at index 0
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

        foreach(ItemSlot itemSlot in currentItemSlots)
        {
            var itemSlotUIObj = Instantiate(itemSlotUI, itemList.transform);

            itemSlotUIObj.SetData(itemSlot);

        }
        
        //Set Initial Item icon and descriptions
        if(currentItemSlots.Count > 0)
        {
            //Activate objects
            itemIcon.gameObject.SetActive(true);
            itemDesciption.gameObject.SetActive(true);

            var item = currentItemSlots[0].Item;
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

    public void SetItemCategory(int indexChange)
    {
        selectedCategoryIndex = selectedCategoryIndex + indexChange;
        //Set Category
        if(selectedCategoryIndex > Inventory.ItemCategories.Count - 1) //if we go 1 above the index maximum
        {
            selectedCategoryIndex = 0;
        }
        else if(selectedCategoryIndex < 0) //if we go below zero
        {
            selectedCategoryIndex = Inventory.ItemCategories.Count - 1;
        }
        


        //Update current item slots based on category
        currentItemSlots = inventory.currentItemSlotsCategory(selectedCategoryIndex);

        //Change Category Test
        itemCategoryText.text = Inventory.ItemCategories[selectedCategoryIndex];

        //Update category list and current item slots
        UpdateItemList();
    }

    public void ItemHover(ItemSlotUI hoverItemSlotUI)
    {

        int hoverItemIndex = hoverItemSlotUI.transform.GetSiblingIndex(); //this returns the index of the item slot
        var item = currentItemSlots[hoverItemIndex].Item;
        itemIcon.sprite = item.Icon;
        itemDesciption.text = item.Description;
          
    }

    public void ItemSelected(ItemSlotUI selectedItemSlotUI)
    {
        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex(); //sets the selected item for use.
        selectedItemSlot = currentItemSlots[selectedItemIndex];

        OpenPartyScreen(); //Open party screen to select monster to use item on
        inventoryScreenState = InventoryScreenState.PartyScreen;
        
    }

    public void PartyMemberSelected(Monster selectedMonster)
    {
        
        if(inventoryScreenState != InventoryScreenState.PartyScreen)
        {
            return;
        }
        
        inventoryScreenState = InventoryScreenState.Busy;

        //might want to consider turning this into a couroutine to help with button spamming to make sure dialogue printd

        if(selectedItemSlot.Quantity == 0) //you are out of item
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText($"You are out of {selectedItemSlot.Item.ItemName}."));
            inventoryScreenState = InventoryScreenState.PartyScreen;
        }
        else if(!GetCanUseItem(selectedMonster)) //can't be used on monster
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText($"{selectedItemSlot.Item.ItemName} won't have any effect on {selectedMonster.Base.MonsterName}."));
            inventoryScreenState = InventoryScreenState.PartyScreen;
        }
        else if(GetCanUseItem(selectedMonster)) //you can use item
        {
            StartCoroutine(UseItem(selectedMonster));
            
        }
        
    }

    public bool GetCanUseItem(Monster selectedMonster)
    {
       return inventory.CanUseItem(selectedItemSlot, selectedMonster);
    }

    public IEnumerator UseItem(Monster selectedMonster)
    {
        inventoryScreenState = InventoryScreenState.Busy;

        var usedItem = inventory.UseItem(selectedItemSlot, selectedMonster); //this method uses the item but also returns the item used from inventory script

        
        if(usedItem != null && GameController.Instance.GameState != GameState.Battle) //of mpt om batt;e ise Dialog manager and keep in party state
        {
           yield return DialogManager.Instance.ShowDialogText($"You used a {usedItem.ItemName}.");
           inventoryScreenState = InventoryScreenState.PartyScreen; //I want the player to remain on party screen and can continue to use items if they would like.
        }
        else if(usedItem == null)
        {
            Debug.Log("item used was null. Check for errors");
        }

        DecreaseItemQuanity();
        
    }

    public void IncreaseQuantity()
    {
        inventory.IncreaseQuantity(selectedItemSlot.Item);
    }
    public void DecreaseItemQuanity()
    {
        inventory.DecreaseQuantity(selectedItemSlot.Item);
    }

    public void OpenPartyScreen()
    {

        partyScreen.gameObject.SetActive(true);

        //GameController.Instance.ShowPartyScreen(); don't think we need this since we aren't changing the game state

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
