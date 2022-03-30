using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
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
        {   //Burn NEED TO ADD ATTACK DROP
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
        {   //Sleep
            ConditionID.slp, 
            new Condition()
            {
                Name = "Sleep",
                Description = "Puts the target to sleep",
                StartMessage = "fell asleep.",
                OnStart = (Monster monster) =>
                {
                    monster.StatusTime = Random.Range(1,3);
                    //Debug.Log($"Monster will be asleep for {monster.StatusTime} turns");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.StatusTime <= 0)
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
                    if (Random.Range(1, 5) == 1) //roll to move 25%
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
                    if (Random.Range(1, 6) == 1) //roll to thaw out 20%
                    {
                        monster.CureStatus();
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} thawed out");
                        return true;
                    }

                    return false;
                }
            }
        },


    //Volatile Statuses

        {   //Confusion
            ConditionID.confusion, 
            new Condition()
            {
                Name = "Confusion",
                Description = "Confuses the target",
                StartMessage = "became confused",
                OnStart = (Monster monster) =>
                {
                    monster.VolatileStatusTime = Random.Range(2,6);
                    //Debug.Log($"Monster will be confused for {monster.VolatileStatusTime} turns");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} snapped out of confusion!");
                        return true;
                    }
                    monster.VolatileStatusTime--; //since this is called each turn we are decrementing our timer
                    monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} is confused.");

                    if(Random.Range(1, 4) == 1) //roll to hurt yoruself in confusion 33% chance
                    {
                        Move confusionDamage = monster.Moves[Random.Range(0,3)]; //call a random move that the monster knows.
                        monster.TakeDamage(confusionDamage, monster);
                        
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} hurt itself in its confusion.");
                        return false;
                    }

                    return true; //monster can move
                }
            }
        },

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, 
    confusion
}
