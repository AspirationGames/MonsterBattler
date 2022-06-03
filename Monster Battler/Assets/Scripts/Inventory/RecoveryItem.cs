using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New recovery Item", order = 0)]
public class RecoveryItem : ItemBase
{
    [Header("Healing")]
    [SerializeField] int healAmount;
    [SerializeField] bool restoresMaxHP;
    
    [Header("AP Recovery")]
    [SerializeField] int apAmount;
    [SerializeField] bool restoresMaxAP;

    [Header("Cure Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool CanUse(Monster monster)
    {
        if(revive || maxRevive)
        {
            if(monster.HP > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        if(healAmount > 0 || restoresMaxHP)
        {
            if(monster.HP == monster.MaxHP)
            {

                return false;
            }
            else
            {
                return true;
            }
        }

        if(status != ConditionID.none || recoverAllStatus)
        {

            if(monster.Status == null && monster.VolatileStatus == null)
            {
                return false;
            }
            else if(recoverAllStatus) //recover all status can work so long as monster has some condition
            {
                return true;
            }
            else if(monster.Status != null) //status is not the one that is null
            {
                if (monster.Status.Id == status)
                    return true;
                else
                    return false;
               
            }
            else if(monster.VolatileStatus != null) // if volatile status is not the one that is null
            {
                 if (monster.VolatileStatus.Id == status)
                    return true;
                else
                    return false;
            }

        }

        if(apAmount > 0 || restoresMaxAP)
        {
            //place holder logic for now
            return true;

        }

        
        

        return false;
    }

    public override void Use(Monster monster)
    {   
        //revive
        if(revive) 
        {
            monster.RestoreHP(monster.MaxHP/2);
            monster.CureStatus();
            return;
        }
        if(maxRevive)
        {
            monster.RestoreHP(monster.MaxHP);
            monster.CureStatus();
            return;
        }

        //Healing Items
        if(restoresMaxHP) monster.RestoreHP(monster.MaxHP);
        if(healAmount > 0  && !restoresMaxHP) monster.RestoreHP(healAmount);

        //Status Cures
        if(recoverAllStatus)
        {
            monster.CureStatus();
            monster.CureVolatileStatus();
        }
        if(status != ConditionID.none && !recoverAllStatus)
        {
            if(status == monster.Status.Id) // need to make sure condition can be cured by specified item
            {
                monster.CureStatus();
            }
            else if(status == monster.VolatileStatus.Id)
            {
                monster.CureVolatileStatus();
            }
        } 

        //AP Recovery (PLACE HOLDER LOGIC)
        if(restoresMaxAP) monster.Moves.ForEach(m => m.RestoreAP(m.Base.AP));
        if(apAmount > 0 && !restoresMaxAP) monster.Moves.ForEach(m => m.RestoreAP(apAmount));

        
        

    }

}
