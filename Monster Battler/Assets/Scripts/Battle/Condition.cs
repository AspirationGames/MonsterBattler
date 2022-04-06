using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public ConditionID Id {get; set;}
    public string Name {get; set;}

    public string Description {get; set;}

    public string StartMessage {get; set;}

    public string EffectMessage {get; set;}

    public Action<Monster> OnStart {get; set;} 
    public Func<Monster, bool> OnBeforeMove{get; set;} //works like a method with a specified return type. i.e. bool
    public Action<Monster> OnAfterTurn {get; set;} //works like a void method
    public Action<Monster> OnWeather {get; set;}
    public Func<Monster, bool> OnProtect {get; set;}  
    public Func<Monster, Monster, Move, float> OnDamageModify {get; set;}

}
