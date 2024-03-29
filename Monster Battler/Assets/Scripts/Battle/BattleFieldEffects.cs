using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldEffects
{
    public Condition Weather {get; set;}

    public int? WeatherDuration {get; set;}

    public Condition TimeWarp{get; set;}

    public int? WarpDuration {get; set;}
    public void SetWeather(ConditionID conditionID)
    {
        Weather = ConditionsDB.Conditions[conditionID];
        Weather.Id = conditionID;
        Weather.OnStart?.Invoke(null);
    }

    public void SetWarp(ConditionID conditionID)
    {
        TimeWarp = ConditionsDB.Conditions[conditionID];
        TimeWarp.Id = conditionID;
        TimeWarp.OnStart?.Invoke(null);
    }
}
