using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public List<Monster> Monsters
    {
        get
        {
            return monsters;
        }
    }

    void Awake()
    {
        foreach(var monster in monsters)
        {
            monster.Init();
        }
    }

    public bool HasHealthyMonster()
    {
        bool hasHealthyMonster;

        foreach(Monster monster in monsters)
        {
            if(monster.HP > 0)
            {
                hasHealthyMonster = true;
                return hasHealthyMonster;
            }
        }

        return hasHealthyMonster = false; 
    }

    public bool CanSwitch()
    {
        //checking to see if party as units available to switch in
        bool canSwitchIn;

        foreach(Monster monster in monsters)
        {
            if(monster.HP > 0 && !monster.InBattle)
            {
                canSwitchIn = true;
                return canSwitchIn;
            }
        }

        return canSwitchIn = false;

    }

}
