using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    public Move EnemyMoveSelection(List<BattleUnit> battleUnits, List<BattleUnit> turnOrder, BattleUnit enemyUnit, BattleFieldEffects battleFieldEffects)
    {
        int speedPriority = SpeedCheck(turnOrder, enemyUnit, battleFieldEffects);
        bool isWeak;

        Monster enemyMonster = enemyUnit.Monster;
        List<Move> enemyMoves = enemyMonster.Moves;

        

        if(this.tag == "Time Warp" && battleFieldEffects.TimeWarp != null && speedPriority != 1)
        {
            foreach(Move move in enemyMoves)
            {
                if(move.Base.Effects.TimeWarp == ConditionID.timewarp)
                {
                    return move;
                }
                else
                    continue;
            }
        }


    
    //Default Action

        return FindHighestDamageMove(battleUnits, enemyUnit, enemyMoves, battleFieldEffects);

    }

    public BattleUnit EnemyTargetSelection(List<BattleUnit> battleUnits, BattleUnit enemyUnit, Move enemyMove, BattleFieldEffects battleFieldEffects)
    {
        if(enemyMove.Base.Target == MoveTarget.Self)
        {
            return enemyUnit;
        }
        
        //Default Action

        return FindHighestDamageTarget(battleUnits, enemyUnit, enemyMove, battleFieldEffects);
    }

    

    private int SpeedCheck(List<BattleUnit> turnOrder, BattleUnit enemyUnit, BattleFieldEffects battleFieldEffects)
    {
        

        if (enemyUnit == turnOrder[0]) //fastest unit
        {
            return 1; //attack

        }
        else if (enemyUnit == turnOrder[1])
        {
            if (turnOrder[0].IsPlayerMonster)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else if (enemyUnit == turnOrder[2])
        {
            return 3;
        }
        else if (enemyUnit == turnOrder[3])
        {
            return 4;
        }


        return 4;
    }

    Move FindHighestDamageMove(List<BattleUnit> battleUnits, BattleUnit enemyUnit, List<Move> enemyMoves, BattleFieldEffects battleFieldEffects)
    {
        

        foreach(BattleUnit unit in battleUnits)
        {
            if(!unit.IsPlayerMonster)
                continue;
            else
                foreach(Move move in enemyMoves)
                {
                    //potential damage
                    DamageDetails potentialDamage = unit.Monster.TakeDamage(move, enemyUnit.Monster, battleFieldEffects.Weather);

                    if(potentialDamage.KO)
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

    BattleUnit FindHighestDamageTarget(List<BattleUnit> battleUnits, BattleUnit enemyUnit, Move move, BattleFieldEffects battleFieldEffects)
    {
       foreach(BattleUnit unit in battleUnits)
        {
            DamageDetails potentialDamage = unit.Monster.TakeDamage(move, enemyUnit.Monster, battleFieldEffects.Weather);

            if(!unit.IsPlayerMonster)
                continue;
            else
            {

                if(potentialDamage.KO)
                {
                    //player Unit can potentially KO enemy;
                    Debug.Log("enemy can be KO'd");
                    return unit;
                }
                else if(potentialDamage.TypeEffectiveness > 1)
                {
                    return unit;
                }
                else if(potentialDamage.TypeEffectiveness < 1)
                {
                    continue;
                }
                else if(potentialDamage.TypeEffectiveness == 1)
                {
                    return unit;
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

            if(potentialDamage.KO)
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