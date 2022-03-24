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
        }

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
