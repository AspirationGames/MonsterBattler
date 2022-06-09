using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New spell item", order = 2)]
public class SpellItem : ItemBase
{
    [SerializeField] MoveBase moveToLearn;
    [SerializeField] bool isBook;

    public override string ItemName => base.ItemName + $": {moveToLearn.MoveName}";
    public override bool CanUsedInBattle => false;

    public override bool isReusable => isBook;
    public override bool CanUse(Monster monster)
    {
        //spell items are handled within inventory screen script
        return true;
    }
    public override void Use(Monster monster)
    {
        Debug.Log("use logic is handled within the Inventory screen script. This method should not be called.");
    }

    public bool CanLearnMove(Monster monster)
    {
        return monster.Base.LearnableMovesByItems.Contains(moveToLearn);
    }


    public MoveBase MoveToLearn => moveToLearn;
    public bool IsBook => isBook;
}
