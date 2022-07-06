using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageTrigger {KOTrigger, WeaknessTrigger, AttackTypeTrigger }

[CreateAssetMenu(menuName = "Items/Held Items/New OnDamageTrigger Item", order = 6)]
public class OnDamageTriggerItem : ItemBase
{
    [SerializeField] DamageTrigger trigger;
    [SerializeField] MonsterType damageTypeRequirment;
    [SerializeField] List<StatStageChange> statStageChanges;
    public override bool CanUsedInBattle => false;
    public override bool CanUse(Monster monster)
    {
        //these items can't be used and are intended to be held by a monster
        return false;
    }
    public override void Use(Monster monster)
    {
        Debug.LogError("use logic within Battle System");
    }

    public void OnDamageTaken(DamageDetails damageDetails, Move attackerMove, Monster monster)
    {
        if(trigger == DamageTrigger.KOTrigger && damageDetails.KO && monster.HP == monster.MaxHP) //Focus Sash
        {
            monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName}'s held on with a {this.ItemName} to half the damage taken by {attackerMove.Base.MoveName}.");
            damageDetails.DamageAmount -= 1;
            monster.HeldItem = null;
        }
        else if(trigger == DamageTrigger.WeaknessTrigger && damageDetails.TypeEffectiveness > 1)
        {
            monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName}'s {this.ItemName} was triggered.");
            monster.ApplyStageChange(statStageChanges);
            monster.HeldItem = null;
        }
        else if(trigger == DamageTrigger.AttackTypeTrigger && attackerMove.Base.Type == damageTypeRequirment)
        {
            monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName}'s {this.ItemName} was triggered.");
            monster.ApplyStageChange(statStageChanges);
            monster.HeldItem = null;
        }
    }
}
