using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> party;

    void Awake()
    {
        foreach(var monster in party)
        {
            monster.Init();
        }
    }

    public Monster GetMonster(int monsterIndex)
    {
        return party[monsterIndex];
    }

    public bool HasHealthyMonster()
    {
        bool hasHealthyMonster;

        foreach(Monster monster in party)
        {
            if(monster.HP > 0)
            {
                hasHealthyMonster = true;
                return hasHealthyMonster;
            }
        }

        return hasHealthyMonster = false; 

        

        
    }

}
