using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;

    [SerializeField] Sprite icon;

    
    public virtual string ItemName => itemName;

    public string Description => description;

    public Sprite Icon => icon;

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
