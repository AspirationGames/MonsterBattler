using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Treat", order = 5)]
public class Treat : ItemBase
{
    [Header("Healing")]
    [SerializeField] float healPercent = 0.33f;
    [SerializeField] float hpTriggerThreshold = 0.5f;

    [Header("Cure Status")]
    [SerializeField] ConditionID cureStatus;

    [Header("Type Resist")]
    [SerializeField] MonsterType typeResist;
    [SerializeField] float damageReduction = 0.5f;

    
    public override bool CanUse(Monster monster)
    {
        if(healPercent > 0) //healing berry
        {
            if(monster.HP == monster.MaxHP) return false;
            else if(monster.HP <= monster.MaxHP * hpTriggerThreshold)
            {
                return true;
            }
        }
        if(cureStatus != ConditionID.none)
        {
            if(monster.Status == null) return false;
            else return true;
        }
        if(typeResist != MonsterType.None)
        {
            return false;
        }

        return false;
    }
    public override void Use(Monster monster)
    {
        //Healing Items
        if(healPercent > 0) monster.RestoreHP(healPercent);

        //Cure Status
        if(cureStatus != ConditionID.none)
        {
            if(cureStatus == monster.Status.Id) // need to make sure condition can be cured by specified item
            {
                monster.CureStatus();
            }
            else if(cureStatus == monster.VolatileStatus.Id)
            {
                monster.CureVolatileStatus();
            }
        } 
    }

    public void OnDamageTaken(DamageDetails damageDetails, Move attackerMove, Monster monster)
    {
        if(attackerMove.Base.Type == typeResist && damageDetails.TypeEffectiveness > 1)
        {
            monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName}'s ate a {this.ItemName} to half the damage taken by {attackerMove.Base.MoveName}.");
            damageDetails.DamageAmount = Mathf.FloorToInt(damageDetails.DamageAmount * damageReduction);
            monster.SetHeldItem(null);
        }
    }

    
}
