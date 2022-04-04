using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    public void ActionSelection(List<BattleUnit> battleUnits, List<BattleUnit> turnOrder, BattleUnit enemyUnit)
    {

        if(!turnOrder[0].IsPlayerMonster || !turnOrder[0].IsPlayerMonster && !turnOrder[1].IsPlayerMonster)
        {
            //enemy units are fastest units
            Debug.Log("enemy has two fastest units on field");
        }
        else if(!turnOrder[1].IsPlayerMonster)
        {
            //enemy is second fastest
            Debug.Log("enemy has second fastest unit but not the fastest unit");
            //check if enemy is weak to player units
            bool weak = isWeak(battleUnits, enemyUnit);
            Debug.Log(weak);

        }
        else
        {
            Debug.Log("something went wrong");
        }

        

    }

    bool isWeak(List<BattleUnit> battleUnits, BattleUnit enemyUnit)
    {
        //List<Move> playerMoves = new List<Move>();
        List<Move> enemyUnitMoves = new List<Move>();
        
        for(int i=0; i < 2; i++)
        {   
            Monster attackingMonster = battleUnits[i].Monster;
            List<Move> playerMoves = attackingMonster.Moves;
           
           foreach(Move move in playerMoves)
           {
               //potential damage
               //DamageDetails potentialDamage = enemyUnit.Monster.TakeDamage(move,attackingMonster,);

               //if(potentialDamage.Fainted)
               {
                   //player Unit can potentially KO enemy;
                   //return true;
               }
               //else
               {
                   //return false;
               }
           }

        }

        return false;

    }

}
