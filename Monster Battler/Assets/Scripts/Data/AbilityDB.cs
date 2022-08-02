using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDB : MonoBehaviour
{
    public static void Init()
    {
        //foreach(var)
    }
    public static Dictionary<AbilityID, Ability> Abilities {get; set;} = new Dictionary<AbilityID, Ability>
    {
        //Intimidate



    };

    
}

public enum AbilityID
{
    intim
}
