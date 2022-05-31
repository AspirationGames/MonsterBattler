using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public enum InventoryScreenState{Inventory, PartyScreen, BindingTargetSelection, ForgettingMove, Busy}
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

    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] BattleDialogBox battleDialogueBox; //place holder until we find a better method to implement
    Inventory inventory;

    InventoryScreenState inventoryScreenState;
    public InventoryScreenState InventoryScreenState => inventoryScreenState;
    
    ItemSlot selectedItemSlot;

    int selectedCategoryIndex = 0;
    List<ItemSlot> currentItemSlots;
    public event Action bindingSelected;
    MoveBase moveToLearn; //new Move monster is trying to learn
    Monster monsterLearning; //The monster trying to learn a new move

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
        partyScreen.screenClosed += ResetInventoryState;
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
        currentItemSlots = inventory.SetCurrentItemSlots(selectedCategoryIndex);

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

        if(inventoryScreenState != InventoryScreenState.Inventory)
        {
            return;
        }

        inventoryScreenState = InventoryScreenState.Busy;
        int selectedItemIndex = selectedItemSlotUI.transform.GetSiblingIndex(); //sets the selected item for use.
        selectedItemSlot = currentItemSlots[selectedItemIndex];


        if(!selectedItemSlot.Item.CanUsedOutsideBattle && GameController.Instance.GameState != GameState.Battle) //if item can't be used outside of battle
        {
            StartCoroutine(DialogManager.Instance.ShowDialogText($"You can't use {selectedItemSlot.Item.ItemName} outside of battles."));
            inventoryScreenState = InventoryScreenState.Inventory;
            return;
        }
        if(!selectedItemSlot.Item.CanUsedInBattle && GameController.Instance.GameState == GameState.Battle) //if the item can't be used in battle
        {
            StartCoroutine(battleDialogueBox.TypeDialog($"You can't use {selectedItemSlot.Item.ItemName} during battles."));
            inventoryScreenState = InventoryScreenState.Inventory;
            return;
        }
            
        
        if(selectedCategoryIndex == (int)ItemCategory.BindingCrystals) //Binding crystals use a battle system event
        {
            Debug.Log("binding crystal selected");
            bindingSelected(); //event notification for battle system
            inventoryScreenState = InventoryScreenState.BindingTargetSelection;
            gameObject.SetActive(false); //closes the inventory screen
            return;

        }

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


        var spellItem = GetSelectedItem() as SpellItem; //this will return null if item is not a spell item
        
        if(spellItem != null) //Handles Spell Items
        {   
            yield return HandleSpellItems(selectedMonster, spellItem);
            yield break;
        }
        
        //Use Recovery Items       
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

    IEnumerator HandleSpellItems(Monster selectedMonster, SpellItem spellItem)
    {
        var newMove = spellItem.MoveToLearn;
        

        if(!spellItem.CanLearnMove(selectedMonster)) //monster can't learn move
        {
            yield return DialogManager.Instance.ShowDialogText($"{selectedMonster.Base.MonsterName} cannot learn {spellItem.MoveToLearn.MoveName}.");
            inventoryScreenState = InventoryScreenState.PartyScreen;
            yield break;
        }

        if(selectedMonster.HasMove(newMove)) // monster already knows move
        {
            yield return DialogManager.Instance.ShowDialogText($"{selectedMonster.Base.MonsterName} already knows {spellItem.MoveToLearn.MoveName}.");
            inventoryScreenState = InventoryScreenState.PartyScreen;
            yield break;
        }

        
        if(selectedMonster.Moves.Count < MonsterBase.MaxNumberOfMoves) //if the monster has less than 4 we can teach the move
        {
            //Teach Move
            selectedMonster.LearnMove(newMove);
            yield return DialogManager.Instance.ShowDialogText($"{selectedMonster.Base.MonsterName} learned {spellItem.MoveToLearn.MoveName}.");
            DecreaseItemQuanity();
        }
        else
        {
            //player will need to forget a move
            yield return DialogManager.Instance.ShowDialogText($"{selectedMonster.Base.MonsterName} wants to learn {newMove.MoveName}.");
            yield return DialogManager.Instance.ShowDialogText($"But {selectedMonster.Base.MonsterName} already knows {MonsterBase.MaxNumberOfMoves} moves.");
            yield return DialogManager.Instance.ShowDialogText($"Forget a move so that {selectedMonster.Base.MonsterName} can learn {newMove.MoveName}.");
            //add in yes or no option
            yield return ChooseMoveToForget(selectedMonster, newMove);


        }

        inventoryScreenState = InventoryScreenState.PartyScreen;

    }

    IEnumerator ChooseMoveToForget(Monster selectedMonster, MoveBase newMove)
    {
        inventoryScreenState = InventoryScreenState.ForgettingMove;
        yield return DialogManager.Instance.ShowDialogText("Choose a move to forget.", false, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(selectedMonster.Moves.Select(x => x.Base).ToList(), newMove);
        moveSelectionUI.SetMonsterImage(selectedMonster);
        monsterLearning = selectedMonster;
        moveToLearn = newMove;
        
        yield return new WaitUntil(() => inventoryScreenState != InventoryScreenState.ForgettingMove);
        moveSelectionUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

    }

    public void OnMoveForget(int moveIndex)
    {
        DialogManager.Instance.CloseDialog();
        if(moveIndex == MonsterBase.MaxNumberOfMoves)
        {
            //player doesn't want to learn new move
            StartCoroutine(DialogManager.Instance.ShowDialogText($"{monsterLearning.Base.MonsterName} did not learn {moveToLearn.MoveName}."));
        }
        else
        {
            //replace selected move with new move
            var selectedMove = monsterLearning.Moves[moveIndex].Base;
            StartCoroutine(DialogManager.Instance.ShowDialogText($"{monsterLearning.Base.MonsterName} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}."));

            monsterLearning.Moves[moveIndex] = new Move(moveToLearn); //replaces selected move with instance of the new move
            DecreaseItemQuanity();

        }

        moveToLearn = null;
        monsterLearning = null;

        inventoryScreenState = InventoryScreenState.Busy;
    }

    

    public ItemBase GetSelectedItem()
    {
        return selectedItemSlot.Item;
    }

    public void IncreaseQuantity()
    {
        inventory.IncreaseQuantity(selectedItemSlot.Item);
    }
    public void DecreaseItemQuanity()
    {
        if(selectedItemSlot.Item.isReusable) //if it is reusable then we don't decrease quanity
        {
            return;
        }

        inventory.DecreaseQuantity(selectedItemSlot.Item);
    }

    public void OpenPartyScreen()
    {

        partyScreen.gameObject.SetActive(true);

        if(GameController.Instance.GameState == GameState.Battle) //if in battle we show message in battle dialog box
        {
            StartCoroutine(battleDialogueBox.TypeDialog($"Select a monster to use {selectedItemSlot.Item.ItemName}."));
        }

        if(selectedItemSlot.Item is SpellItem)
        {
            //show if spell item is usable
            partyScreen.ShowIfSpellisUsable(selectedItemSlot.Item as SpellItem);
        }

        //GameController.Instance.ShowPartyScreen(); don't think we need this since we aren't changing the game state. 
        //We also don't want to use this because when we Open the party screen during battle we don't want to change the game state. 

    }
    public void ResetInventoryState()
    {
        inventoryScreenState = InventoryScreenState.Inventory;
    }

    public void BackToPauseMenu()
    {
        GameController.Instance.BackToPauseMenu();
        gameObject.SetActive(false);
    }

}
