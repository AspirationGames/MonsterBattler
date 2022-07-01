using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held Items/New stat-enhancing item", order = 5)]
public class StatEnhancingItem : ItemBase
{
    [SerializeField] Stat statBoost;
    [SerializeField] float boostAmount = 1.2f;
    [SerializeField] bool locksMoves = false;
    public bool LocksMoves => locksMoves;
    public override bool CanUse(Monster monster)
    {
        //these items can't be used and are intended to be held by a monster
        return false;
    }
    public override void Use(Monster monster)
    {
        Debug.LogError("use logic is handled within the Inventory screen script. This method should not be called.");
    }

    public float GetStatBoostModifier(Stat stat)
    {
        if(stat != statBoost) return 1;
        //else
        return boostAmount;
    }
}
