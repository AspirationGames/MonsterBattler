using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    
    public int ActionSelection(List<BattleUnit> battleUnits, List<BattleUnit> turnOrder, BattleUnit enemyUnit, BattleFieldEffects battleFieldEffects)
    {
        bool isWeak;

        if(enemyUnit == turnOrder[0]) //fastest unit
        {
            return 1; //attack

        }
        else if(enemyUnit == turnOrder[1])
        {
            if(turnOrder[0].IsPlayerMonster)
            {
                isWeak = IsWeak(turnOrder[0],enemyUnit,battleFieldEffects);
                if (isWeak) return 2; //defend
                else return 1; //attack
            }
            else
            {
                return 1;
            }
        }
        else if(enemyUnit == turnOrder[2])
        {
            return 3;
        }
        else if(enemyUnit == turnOrder[3])
        {
            return 4;
        }


        return 4;

    }

    Move SelectMove(List<BattleUnit> battleUnits, BattleUnit enemyUnit, BattleFieldEffects battleFieldEffects)
    {
        Monster enemyMonster = enemyUnit.Monster;
        List<Move> enemyMoves = enemyMonster.Moves;


        for(int i=0; i < 2; i++)
        {
            Monster playerMonster = battleUnits[i].Monster;

            foreach(Move move in enemyMoves)
            {
                //potential damage
               DamageDetails potentialDamage = playerMonster.TakeDamage(move, enemyMonster, battleFieldEffects.Weather);

                if(potentialDamage.Fainted)
               {
                   //player Unit can potentially KO enemy;
                   Debug.Log("enemy can be KO'd");
                   return move;
               }
               else if(potentialDamage.TypeEffectiveness > 1)
               {
                   return move;
               }
               else
               {
                   continue;
               }

            }
        }

        return null;
        
    }

    bool IsWeak(BattleUnit playerUnit, BattleUnit enemyUnit, BattleFieldEffects battleFieldEffects)
    {
           
        Monster attackingMonster = playerUnit.Monster;
        List<Move> playerMoves = attackingMonster.Moves;
           
        foreach(Move move in playerMoves)
        {
            //potential damage
            DamageDetails potentialDamage = enemyUnit.Monster.TakeDamage(move,attackingMonster, battleFieldEffects.Weather);

            if(potentialDamage.Fainted)
            {
                //player Unit can potentially KO enemy;
                Debug.Log("enemy can be KO'd");
                return true;
            }
            else
            {
                return false;
            }
        }

        

        return false;

    }

}
