using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public AbilityID ID {get; set;}
    public string AbilityName {get; set;}
    public string AbilityDescription {get; set;}
    
    public Action OnEnter {get; set;}
    
    public Action OnWeatherInEffect {get; set;}
    public Action OnBeforeTurn {get; set;}
}
