using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held Items/New OnAfterTurn Item", order = 7)]

public class OnAfterTurnItem : ItemBase
{
    [SerializeField] float hpRecovery = 0;
    [SerializeField] MonsterType monsterTypeRequirment = MonsterType.None;
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

    public void OnAfterTurn(Monster monster)
    {   
        Debug.Log("one after turn");
        //HP Recovery
        if(hpRecovery > 0 && monsterTypeRequirment == MonsterType.None && monster.HP < monster.MaxHP)
        {
            int hpHealAmount = Mathf.FloorToInt(monster.MaxHP * hpRecovery);
            monster.RestoreHP(hpHealAmount);
            monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} recoverd HP with {this.ItemName}");
        
            
        }
        //HP Recovery with type Requiremnet
        else if(hpRecovery > 0 && monsterTypeRequirment != MonsterType.None && monster.HP < monster.MaxHP)
        {
            if(monsterTypeRequirment == monster.Base.Type1 || monsterTypeRequirment == monster.Base.Type2)
            {
                int hpHealAmount = Mathf.FloorToInt(monster.MaxHP * hpRecovery);
                monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} recoverd HP with {this.ItemName}");
            
            }
             
        }
        //Stat boost
        else if(statStageChanges != null && monsterTypeRequirment == MonsterType.None)
        {
            monster.ApplyStageChange(statStageChanges);
        
            
        }
        //Stat boost with type requirement
        else if(statStageChanges != null && monsterTypeRequirment != MonsterType.None)
        {
            if(monsterTypeRequirment == monster.Base.Type1 || monsterTypeRequirment == monster.Base.Type2)
            {
                monster.ApplyStageChange(statStageChanges);
                
            }
            
        }

    }
}

