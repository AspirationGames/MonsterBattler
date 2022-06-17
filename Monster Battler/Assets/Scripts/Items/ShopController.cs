using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ShopState {Menu, Buying, Selling, Busy}
public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryScreen inventoryScreen;
    [SerializeField] ShopScreen shopScreen;
    [SerializeField] MoneyUI moneyUI;
    [SerializeField] ShopQtyUI shopQtyUI;
    
    public static ShopController i {get; private set;}
    public event Action onStartShopping;
    public event Action onEndShopping;
    ShopState shopState;
    Inventory inventory;

    ShopKeeper shopKeeper;
    private void Awake() 
    {
        i = this;

        inventory = Inventory.GetInventory(); //gets inventory    
    }

    private void Start() 
    {
        //Need to figure out way to subscrive sell item selectio to couroutine calling method right now
        inventoryScreen.onItemSold += (ItemBase item) => StartCoroutine(SellItem(item));
        shopScreen.onItemBought += (ItemBase item) => StartCoroutine(BuyItem(item));
    }
    public IEnumerator StartTrading(ShopKeeper shopKeeper)
    {
        this.shopKeeper = shopKeeper;
        onStartShopping?.Invoke();
        yield return ShowShopMenu();

    }

    public IEnumerator ShowShopMenu()
    {
        shopState = ShopState.Menu;

        int selectedChoice = 0; //this variable gets set based on the return values of the action.

        yield return DialogManager.Instance.ShowDialogText("What would you like to do?", false, true, 
        new List<string>(){"Buy","Sell","Leave"}, choiceIndex => selectedChoice = choiceIndex );

        if(selectedChoice == 0) //Buy
        {
            shopState = ShopState.Buying;
            shopScreen.ShowShopUI(shopKeeper.AvailableItems);
            moneyUI.ShowMoneyBox();
            yield return DialogManager.Instance.ShowDialogText("What would you like to purchase?", false, false);
        }
        else if(selectedChoice == 1) //Sell
        {
            shopState = ShopState.Selling;
            inventoryScreen.gameObject.SetActive(true);
            moneyUI.ShowMoneyBox();
            yield return DialogManager.Instance.ShowDialogText("Select an Item to sell.", false, false);

        }
        else if(selectedChoice == 2) //leave
        {
            onEndShopping?.Invoke();
            yield break;
            
        }

        
    }

    IEnumerator BuyItem(ItemBase item)
    {
        shopState = ShopState.Busy;

        int buyQty = 1;
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("How many would you like to buy?", false, false);
        yield return shopQtyUI.ShowQuantity(99, item.BuyPrice, (qtyConfirmed) => buyQty = qtyConfirmed);
        DialogManager.Instance.CloseDialog(); //closes the dialog sicne we did not auto close
        int buyPrice = buyQty * item.BuyPrice;
        //confirm sale
        if(buyPrice > PlayerMoney.i.Money)
        {
            yield return DialogManager.Instance.ShowDialogText("You can't afford to buy that.",closeDelay: 1f);
            yield return DialogManager.Instance.ShowDialogText("Select an Item to buy.", false, false);
            yield break; //BREAK and player must select new item to buy or stop shopping
        }

        yield return DialogManager.Instance.ShowDialogText($"Buy {buyQty} {item.ItemName} for {buyPrice}?", choices: new List<string>() {"Yes","No"}, onChoiceSelectedAction: (choiceSelectionIndex) => 
        {
                selectedChoice = choiceSelectionIndex;
        });

        if(selectedChoice == 0) //yes
        {
            //Buy Item
            inventory.AddItem(item, buyQty);
            PlayerMoney.i.ReduceMoney(buyPrice);
            yield return DialogManager.Instance.ShowDialogText($"You purchased {buyQty} {item.ItemName} for {buyPrice}.",closeDelay: 1f);
            yield return DialogManager.Instance.ShowDialogText("Select an Item to buy.", false, false);
        }

        shopState = ShopState.Buying;

    }
    IEnumerator SellItem(ItemBase item)
    {
        shopState = ShopState.Busy;

        if(!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("You can't sell that item");
            shopState = ShopState.Selling; //return to sell item selection
            yield return DialogManager.Instance.ShowDialogText("Select an Item to sell.", false, false);
            yield break;
        }

        int itemQty = inventory.GetItemCount(item);
        int sellQty = 1;
        int selectedChoice = 0;
        

        if(itemQty > 1) //if we have more than 1 item
        {

            yield return DialogManager.Instance.ShowDialogText("How many would you like to sell?", false, false);
            yield return shopQtyUI.ShowQuantity(itemQty, item.SellPrice, (qtyConfirmed) => sellQty = qtyConfirmed);
            DialogManager.Instance.CloseDialog(); //closes the dialog sicne we did not auto close
            
        }

        int sellPrice = sellQty * item.SellPrice;
        //confirm sale
        yield return DialogManager.Instance.ShowDialogText($"Sell {sellQty} {item.ItemName} for {sellPrice}?", choices: new List<string>() {"Yes","No"}, onChoiceSelectedAction: (choiceSelectionIndex) => 
        {
                selectedChoice = choiceSelectionIndex;
        });

        if(selectedChoice == 0) //yes
        {
               //Sell the item
            inventory.DecreaseQuantity(item, sellQty);  //double check correct item is reduced
            PlayerMoney.i.AddMoney(sellPrice);
            yield return DialogManager.Instance.ShowDialogText($"You sold {sellQty} {item.ItemName} for {sellPrice}.",closeDelay: 1f);
            yield return DialogManager.Instance.ShowDialogText("Select an Item to sell.", false, false);
        }

        shopState = ShopState.Selling;
        
    }

    public void CloseShopScreen()
    {
        shopScreen.CloseShopUI();
        moneyUI.CloseMoneyBox();
        shopQtyUI.gameObject.SetActive(false);
        StartCoroutine(ShowShopMenu());
    }
    public void CloseInventory()
    {
        if(GameController.Instance.GameState != GameState.Shopping)
        {
            return;
        }
        
        inventoryScreen.gameObject.SetActive(false);
        moneyUI.CloseMoneyBox();
        shopQtyUI.gameObject.SetActive(false);
        StartCoroutine(ShowShopMenu());
        
    }

    

}
