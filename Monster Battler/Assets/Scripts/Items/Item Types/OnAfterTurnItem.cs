using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held Items/New OnAfterTurn Item", order = 7)]
public class OnAfterTurnItem : ItemBase
{
    [SerializeField] float hpRecovery;
    [SerializeField] MonsterType monsterTypeRequirment;
    [SerializeField] List<StatStageChange> statStageChanges;
    public override bool CanUsedInBattle => false;
    public override bool CanUse(Monster monster)
    {
        //these items can't be used and are intended to be held by a monster
        return false;
    }
    public override void Use(Monster monster)
    {
        Debug.LogError("use logic within Monster Script");
    }

    public void OnAfterTurn()
    {   
        //Black Sludge (Type Requirement)
        //HP Recovery
        //Speed Booster

    }
}

