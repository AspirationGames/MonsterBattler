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

    public void SwapPartyPositions(Monster currentMonster, Monster incomingMonster)
    {
        int currentMonsterIndex = monsters.IndexOf(currentMonster);
        int incomingMonsterIndex = monsters.IndexOf(incomingMonster);

        monsters.Remove(currentMonster);
        monsters.Remove(incomingMonster);
        monsters.Insert(currentMonsterIndex, incomingMonster);
        monsters.Insert(incomingMonsterIndex, currentMonster);
    }

    public Monster FindNextHealthyMonster()
    {
        foreach(Monster monster in monsters)
        {
            if(monster.HP > 0 && !monster.InBattle)
            {
                
                return monster;
            }
            
        }

        Debug.Log("No Healthy Monster found. Please review code!");
        return monsters[0];

    }

    public void AddMonster(Monster monster)
    {
        if(monsters.Count < 4)
        {
            monsters.Add(monster);
        }
        else
        {
            //transfer monster to storage
        }
    }

}
