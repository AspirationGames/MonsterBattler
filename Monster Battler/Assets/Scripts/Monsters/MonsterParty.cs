using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public event Action OnUpdated;

    public List<Monster> Monsters
    {
        get
        {
            return monsters;
        }
        set
        {
            monsters = value; //we need the set method in this case to be able to assign the monster party such as when loading game
            OnUpdated?.Invoke();
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

    public void SortParty(Monster monster, int BattlePosition)
    {
        if(monsters.IndexOf(monster) != BattlePosition)
        {
            SwapPartyPositions(monster, monsters[BattlePosition]);

            OnUpdated?.Invoke();
        }
        else
        {
            return;
        }

    }

    public void SwapPartyPositions(Monster currentMonster, Monster incomingMonster)
    {
        int currentMonsterIndex = monsters.IndexOf(currentMonster);
        int incomingMonsterIndex = monsters.IndexOf(incomingMonster);

        monsters.Remove(currentMonster);
        monsters.Remove(incomingMonster);
        monsters.Insert(currentMonsterIndex, incomingMonster);
        monsters.Insert(incomingMonsterIndex, currentMonster);

        OnUpdated?.Invoke();
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

        //No healthy monster found that is not already in battle
        return null;

    }

    public IEnumerator CheckForEvolution()
    {
        foreach(Monster monster in monsters)
        {
            var evolution = monster.CheckForEvolution();
            if(evolution != null)
            {
                yield return DialogManager.Instance.ShowDialogText($"Huh!? Something is happening with {monster.Base.MonsterName}.");
                yield return EvolutionManager.i.Evolve(monster, evolution);
                OnUpdated?.Invoke();
            }
            else
            {
                continue;
            }
        }

    }
    public void AddMonster(Monster monster)
    {
        if(monsters.Count < 4)
        {
            monsters.Add(monster);
            OnUpdated?.Invoke();
        }
        else
        {
            //transfer monster to storage
        }
    }

    public void RemoveMonster(Monster monster)
    {
        monsters.Remove(monster);
        OnUpdated?.Invoke();

    }

    public void PartyUpdated() //used to trigger update event from other scripts
    {
        OnUpdated?.Invoke();
    }

    public static MonsterParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<MonsterParty>();
    }

}
