using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Evolution Item", order = 3)]
public class EvolutionItem : ItemBase
{
    [SerializeField] List<MonsterBase> evolvableMonsters;

    Evolution evolution;
    public Evolution Evolution => evolution;
    public override bool CanUse(Monster monster)
    {
        evolution = monster.Base.Evolutions.FirstOrDefault(e => e.ItemEvolution == this);

        if (evolution != null)
        {
            if(evolution.EvolutionLevel == monster.Level)
            {
                return true;
            }

            return false;
        }
        
        return false;
    }

    public override void Use(Monster monster)
    {
        Debug.Log("Should not be calling Use when using Evolution Items");
        // we handle evolution in the inventory screen evolution item couroutine.
    }
}
