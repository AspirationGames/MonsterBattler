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
            }
        }

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
