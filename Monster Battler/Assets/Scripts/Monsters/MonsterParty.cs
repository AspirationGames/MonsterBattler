using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    void Awake()
    {
        foreach(var monster in monsters)
        {
            monster.Init();
        }
    }

    public Monster GetMonster(int monsterIndex)
    {
        return monsters[monsterIndex];
    }


}
