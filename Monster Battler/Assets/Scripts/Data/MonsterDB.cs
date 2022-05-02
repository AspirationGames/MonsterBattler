using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDB
{
static Dictionary<string, MonsterBase> monsters;

    public static void Init()
    {
        monsters = new Dictionary<string, MonsterBase>();

        var monsterArray = Resources.LoadAll<MonsterBase>(""); //loads all MonsterBase scriptable objects in the Resources folder

        foreach(var monster in monsterArray)
        {
            
            if(monsters.ContainsKey(monster.MonsterName)) //if the dictionary already contains the name of the monster it is trying to add
            {
                Debug.Log($"Two monsters have the {monster.MonsterName} unable to add duplicate, check game object named {monster.name}");

            }

            monsters[monster.MonsterName] = monster; //adds each monster to the dictionary
        }
    }

    public static MonsterBase GetMonsterByName(string monsterName)
    {
        if(!monsters.ContainsKey(monsterName))
        {
            Debug.Log($"monster with name {monsterName} doese not exist.");
            return null;
        }

        return monsters[monsterName];
    }
    
}
