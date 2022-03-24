using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name {get; set;}

    public string Description {get; set;}

    public string StartMessage {get; set;}

    public Action<Monster> OnStart {get; set;} 
    public Func<Monster, bool> OnBeforeMove{get; set;} //works like a method with a specified return type. i.e. bool
    public Action<Monster> OnAfterTurn {get; set;} //works like a void method

}
