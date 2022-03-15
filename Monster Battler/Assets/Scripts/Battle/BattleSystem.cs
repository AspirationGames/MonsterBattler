using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState {Start, PlayerAction, PlayerMoveOne, PlayerMoveTwo, EnemyMove, Busy}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> battleUnits;
    [SerializeField] List<BattleHud> battleHuds;

    [SerializeField] BattleDialogBox battleDialogueBox;

    BattleState battleState;    

    [SerializeField] List<BattleUnit> turnOrder = new List<BattleUnit>();
    List<int> selectedMoves =  new List<int>();
    List<int> selectedTargets = new List<int>();

    [SerializeField] MonsterParty playerParty;
    [SerializeField] MonsterParty enemyParty;


    void Start()
    {

        StartCoroutine(SetupBattle());
    }

    void StartBattle(MonsterParty playerParty, MonsterParty enemyParty)
    {
        //this.playerParty = playerParty;
        //this.enemyParty = enemyParty;
        //StartCoroutine(SetupBattle());
    }
    
    public IEnumerator SetupBattle()
    {
        foreach(BattleUnit unit in battleUnits) //set up player
        {
            if(unit.isPlayerMonster)
            {
                unit.Setup(playerParty.GetMonster(battleUnits.IndexOf(unit))); //returns monsters at index 0 and 1
                battleHuds[battleUnits.IndexOf(unit)].SetData(unit.Monster);
                turnOrder.Add(unit);

            }
            else
            {
                unit.Setup(enemyParty.GetMonster(battleUnits.IndexOf(unit)-2)); //returns monsters at index 0 and 1 for enemy party which is why we subtract by 2
                battleHuds[battleUnits.IndexOf(unit)].SetData(unit.Monster);
                turnOrder.Add(unit);
                
            }
            
            
        }


       yield return battleDialogueBox.TypeDialog($"A wild {battleUnits[2].Monster.Base.MonsterName} and {battleUnits[3].Monster.Base.MonsterName} appeared!"); //you can use yield return to call anothe coroutine which is what we are doing here
       
        TurnOrder();
        PlayerAction();
    }

    void TurnOrder()
    {

        turnOrder.Sort(SpeedComparison);
        
    }

    int SpeedComparison(BattleUnit a, BattleUnit b)
    {
        if(a.Monster.Speed > b.Monster.Speed)
        {
            return -1; //we return negative 1 here because -1 means we are moving it up in the list or "to the left"
        }
        else if(a.Monster.Speed < b.Monster.Speed)
        {
            return 1;
        }
        return 0;
    }

   void PlayerAction()
    {
        battleState = BattleState.PlayerAction; //changing the battle state
        StartCoroutine(battleDialogueBox.TypeDialog("What will you do?"));
        battleDialogueBox.EnableActionSelector(true);
    }

    public void SelectMove()
    {

        battleDialogueBox.EnableMoveSelector(true);
        
        if(battleState == BattleState.PlayerAction)
        {
            battleDialogueBox.EnableActionSelector(false);
            battleState = BattleState.PlayerMoveOne;
            battleDialogueBox.SetMoveNames(battleUnits[0].Monster.Moves);
        } 
        else if(battleState == BattleState.PlayerMoveOne)
        {
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.PlayerMoveTwo;
            battleDialogueBox.SetMoveNames(battleUnits[1].Monster.Moves);
        } 
        
    }

    public void OnMoveSelected(int moveIndex)
    {

        //save selected moves
        if(battleState == BattleState.PlayerMoveOne)
        {
            selectedMoves.Insert(0,moveIndex);
        }
        else if(battleState == BattleState.PlayerMoveTwo)
        { 
            selectedMoves.Insert(1,moveIndex);
        }

        SelectTarget();

    }


    void SelectTarget()
    {   
        battleDialogueBox.EnableMoveSelector(false);
        battleDialogueBox.EnableTargetSelector(true);
        battleDialogueBox.SetTargetNames(battleUnits);

    }

    public void OnTargetSelected(int targetIndex)
    {
        //save selected targets
        if(battleState == BattleState.PlayerMoveOne)
        {

            selectedTargets.Insert(0,targetIndex);
            
        }
        else if(battleState == BattleState.PlayerMoveTwo)
        {
            selectedTargets.Insert(1,targetIndex);

        }

        PlayerMoveComplete();
    }

    void PlayerMoveComplete()
    {

        if(battleState == BattleState.PlayerMoveOne)
        {

            SelectMove();
            
        }
        else if(battleState == BattleState.PlayerMoveTwo)
        {
            //end trurn;
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.EnemyMove;
            EnemyMoveSelection();
            
        }
    }

    void EnemyMoveSelection()
    {

        for(int i=2; i < battleUnits.Count; i++)
        {
            int randmomMoveIndex = UnityEngine.Random.Range(0, battleUnits[i].Monster.Moves.Count);
            int randomTargetIndex = UnityEngine.Random.Range(0,1);
            selectedMoves.Insert(i, randmomMoveIndex);
            selectedTargets.Insert(i, randomTargetIndex);
        }

        StartCoroutine(PerformMoves());
    }
    

    IEnumerator PerformMoves()
    {
        battleState = BattleState.Busy;

        for(int i=0; i < turnOrder.Count; i++)
        {
            Monster attackingMonster = turnOrder[i].Monster;
            BattleUnit attackingUnit = turnOrder[i];
            Move attackingMove = attackingMonster.Moves[ selectedMoves[battleUnits.IndexOf(turnOrder[i])] ]; 
            Monster targetMonster = battleUnits[ selectedTargets[battleUnits.IndexOf(turnOrder[i])] ].Monster;
            BattleHud targetHud = battleHuds[ selectedTargets[battleUnits.IndexOf(turnOrder[i])] ];
            BattleUnit targetUnit = battleUnits[ selectedTargets[battleUnits.IndexOf(turnOrder[i])] ];

            // need to find way of changing target in the even that target dies mid turn. 

            if(targetMonster.HP <= 0) // check if target is alive
            {
                Debug.Log("Find new target");
                FindNewTarget(attackingUnit, ref targetMonster, ref targetHud, ref targetUnit);
                
            }

            if(targetMonster == null)
            {
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} use {attackingMove.Base.MoveName}");
                yield return battleDialogueBox.TypeDialog
                ("But it Failed");
            }
            else
            {
                //Perform attack
                attackingMove.AP--; 
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} use {attackingMove.Base.MoveName} on {targetMonster.Base.MonsterName}");

                var damageDetails = targetMonster.TakeDamage(attackingMove, attackingMonster);
                yield return targetHud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);

                if(damageDetails.Fainted)//if the monster FAINTS
                {
                    yield return battleDialogueBox.TypeDialog($"{targetMonster.Base.MonsterName} fainted");
                    turnOrder.Remove(targetUnit);

                }
            }


            
            
        }
        //attack phase over
        selectedMoves.Clear(); //clear move queu
        selectedTargets.Clear(); //clear target queu
        PlayerAction();
        
    }

    void FindNewTarget(BattleUnit attackingUnit, ref Monster targetMonster, ref BattleHud targetHud, ref BattleUnit targetUnit)
    {
        
        if (attackingUnit.isPlayerMonster && !targetUnit.isPlayerMonster) //player attacking enemy
        {
            foreach (BattleUnit unit in battleUnits)
            {
                if (!unit.isPlayerMonster && unit.Monster.HP > 0)
                {
                    targetUnit = unit;
                    targetMonster = unit.Monster;
                    targetHud = battleHuds[battleUnits.IndexOf(unit)];
                    return; //we use return to exit the method completly
                }
            }
        }
        else if(!attackingUnit.isPlayerMonster && targetUnit.isPlayerMonster) //enemy attacking player
        {
            foreach (BattleUnit unit in battleUnits)
            {
                if (unit.isPlayerMonster && unit.Monster.HP > 0)
                {
                    targetUnit = unit;
                    targetMonster = unit.Monster;
                    targetHud = battleHuds[battleUnits.IndexOf(unit)];
                    return;
                }
            }
        }
        else
        {
            targetMonster = null;
            return; //if the unit is targeting itself or an ally we will just return for now.
        }

    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if(damageDetails.Critical > 1f) 
            yield return battleDialogueBox.TypeDialog("A Critical Hit!");
        if(damageDetails.TypeEffectiveness > 1f) 
            yield return battleDialogueBox.TypeDialog("It's super Effective!");
        else if(damageDetails.TypeEffectiveness < 1f)
            yield return battleDialogueBox.TypeDialog("It's not very effective");
    }
    

    

    

}
