using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;

    [SerializeField] Sprite icon;

    [SerializeField] int buyPrice;
    [SerializeField] int sellPrice;
    [SerializeField] bool isSellable;

    
    public virtual string ItemName => itemName;

    public string Description => description;

    public Sprite Icon => icon;

    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public bool IsSellable => isSellable;

    public virtual bool CanUse(Monster monster)
    {
        return false;

    }
    public virtual void Use(Monster monster)
    {


    }

    public virtual bool CanUsedInBattle => true;
    public virtual bool CanUsedOutsideBattle => true;
    public virtual bool isReusable => false;
   
}
