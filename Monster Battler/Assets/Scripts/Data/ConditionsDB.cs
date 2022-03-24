using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions {get; set;} = new Dictionary<ConditionID, Condition>()
    {
        {   //Poison
            ConditionID.psn, 
            new Condition()
            {
                Name = "Poison",
                Description = "Poisons the target.",
                StartMessage = "was poisoned.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHP / 8);
                    monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} was hurt by poison");
                }
            }
        },
        {   //Burn
            ConditionID.brn, 
            new Condition()
            {
                Name = "Burn",
                Description = "Burns the target.",
                StartMessage = "was burned.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHP / 16);
                    monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} was hurt by burn");
                }
            }
        },
        {   //Sleep NEEDS TO BE CORRECTED
            ConditionID.slp, 
            new Condition()
            {
                Name = "Sleep",
                Description = "Puts the target to sleep",
                StartMessage = "fell asleep.",
                OnStart = (Monster monster) =>
                {
                    monster.StatusTime = Random.Range(1,3);
                    Debug.Log($"Monster will be asleep for {monster.StatusTime} turns");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.StatusTime == 0)
                    {
                        monster.CureStatus();
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} woke up!");
                        return true;
                    }
                    monster.StatusTime--; //since this is called each turn we are decrementing our timer for sleep
                    monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} is fast asleep.");
                    return false;
                }
            }
        },
        {   //Paralyze
            ConditionID.par, 
            new Condition()
            {
                Name = "Paralyze",
                Description = "Paralyzes the target",
                StartMessage = "was Paralyzed.",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1) //monster can't move
                    {
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} is paralyzed and unable to move.");
                        return false;
                    }

                    return true;
                }
            }
        },
        {   //Freeze
            ConditionID.frz, 
            new Condition()
            {
                Name = "Freeze",
                Description = "Freezes the target",
                StartMessage = "was Frozen.",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.CureStatus();
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} thawed out");
                        return true;
                    }

                    return false;
                }
            }
        }

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
