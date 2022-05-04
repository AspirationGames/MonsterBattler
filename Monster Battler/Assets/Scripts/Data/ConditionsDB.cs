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
                OnStart = (Monster monster) =>
                {
                    var psnSpAttackDrop = new StatStageChange();
                    psnSpAttackDrop.stat = Stat.SpAttack;
                    psnSpAttackDrop.stage = -2;

                    List<StatStageChange> psnStatStageChanges = new List<StatStageChange> ();
                    psnStatStageChanges.Add(psnSpAttackDrop);
                    
                    monster.ApplyStageChange(psnStatStageChanges);
                },
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHP / 8);
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
                OnStart = (Monster monster) =>
                {
                    var brnAttackDrop = new StatStageChange();
                    brnAttackDrop.stat = Stat.Attack;
                    brnAttackDrop.stage = -2;

                    List<StatStageChange> brnStatStageChanges = new List<StatStageChange> ();
                    brnStatStageChanges.Add(brnAttackDrop);
                    
                    monster.ApplyStageChange(brnStatStageChanges);
                },
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHP / 16);
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
                OnStart = (Monster monster) =>
                {
                    var parSpeedDrop = new StatStageChange();
                    parSpeedDrop.stat = Stat.Speed;
                    parSpeedDrop.stage = -2;

                    List<StatStageChange> parStatStageChanges = new List<StatStageChange> ();
                    parStatStageChanges.Add(parSpeedDrop);
                    
                    monster.ApplyStageChange(parStatStageChanges);
                },
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
                        //Move confusionDamage = monster.Moves[Random.Range(0,3)]; //call a random move that the monster knows.
                        //monster.TakeDamage(confusionDamage, monster);
                        
                        monster.DecreaseHP(monster.MaxHP / 8);

                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} hurt itself in its confusion.");
                        return false;
                    }

                    return true; //monster can move
                }
            }
        },

        //Weather

        {   //Scortching Sun
            ConditionID.sun, 
            new Condition()
            {
                Name = "Scortching Sun",
                StartMessage = "The heat of the sun intensified.",
                EffectMessage = "The battlefield is scortching hot.",
                OnDamageModify = (Monster attackingMonster, Monster targetMonster, Move move) =>
                {
                    if(move.Base.Type == MonsterType.Fire)
                        return 1.5f;
                    else if(move.Base.Type == MonsterType.Water)
                        return 0.5f;
                    else
                        return 1f;
                }
            }
        },
        {   //Pouring Rain
            ConditionID.rain, 
            new Condition()
            {
                Name = "Pouring Rain",
                StartMessage = "Rain began to pour down from above.",
                EffectMessage = "The battlefield is flooded with water",
                OnDamageModify = (Monster attackingMonster, Monster targetMonster, Move move) =>
                {
                    if(move.Base.Type == MonsterType.Water)
                        return 1.5f;
                    else if(move.Base.Type == MonsterType.Fire)
                        return 0.5f;
                    else
                        return 1f;
                }
            }
        },
        {   //Pouring Rain
            ConditionID.sandstorm, 
            new Condition()
            {
                Name = "Sandstorm",
                StartMessage = "A sand storm began.",
                EffectMessage = "The battlefield is engulfed in a sandstorm",
                OnWeather = (Monster monster) =>
                {
                    if(monster.Base.Type1 != MonsterType.Earth || monster.Base.Type2 != MonsterType.Earth)
                    {
                        monster.DecreaseHP(Mathf.RoundToInt((float)monster.MaxHP / 16f)); //damage monsters
                        monster.StatusChangeMessages.Enqueue($"{monster.Base.MonsterName} is ravaged by the sandstorm");
                    }
                    else
                    {
                        return;
                    }
                },
                OnDamageModify = (Monster attackingMonster, Monster targetMonster, Move move) =>
                {
                    if(targetMonster.Base.Type1 == MonsterType.Earth || targetMonster.Base.Type2 == MonsterType.Earth)
                        return 1.5f; //sandstorm will boost attack of monsters
                    else
                        return 1f;
                }
            }
        },

        //Time Warp Effects

        {   //Time Warp
            ConditionID.timewarp, 
            new Condition()
            {
                Name = "Time Warp",
                StartMessage = "Time dimensions were warped",
            }
        },

        //Protection effects

        {   //Protect
            ConditionID.barrier, 
            new Condition()
            {
                Name = "Barrier",
                EffectMessage = "was shielded by a barrier",
                OnStart = (Monster monster) =>
                {
                    //placeholder for future effects
                },
                OnProtect = (Monster monster) =>
                {
                    if(monster.ProtectSucessChance > Random.Range(1, 101))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        },

    };

    
    public static float GetStatusBonus(Condition condition)
    {
        if(condition == null)
        {
            return 1f;
        }
        else if(condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
        {
            return 2f;
        }
        else if(condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
        {
            return 1.5f;
        }

        return 1f;
    }
}

public enum ConditionID
{
    //Status Conditions
    none, psn, brn, slp, par, frz, 
    
    //Volatile Status Conditions
    confusion,

    //Weather Effects
    sun, rain, sandstorm,

    //Warp Effects
    timewarp,

    //Protected
    barrier
}
