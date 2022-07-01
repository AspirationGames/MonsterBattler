using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held Items/New type-enhancing item", order = 4)]
public class TypeEnhancingItem : ItemBase
{
    [SerializeField] MonsterType typeBoost;
    [SerializeField] float boostAmount = 1.2f;

    public MonsterType TypeBoost => typeBoost;

    public override bool CanUse(Monster monster)
    {
        //these items can't be used and are intended to be held by a monster
        return false;
    }
    public override void Use(Monster monster)
    {
        Debug.LogError("use logic is handled within the Inventory screen script. This method should not be called.");
    }

    public float GetTypeBoostModifier(Move move)
    {
        if(typeBoost != move.Base.Type) return 1;
        //else
        return boostAmount;
    }
}
