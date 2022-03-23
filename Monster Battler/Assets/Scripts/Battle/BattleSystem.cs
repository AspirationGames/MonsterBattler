using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState 
{PlayerAction1, PlayerAction2, PlayerMove1, PlayerMove2, PlayerTarget1, PlayerTarget2, PlayerSwitch1, PlayerSwitch2, EnemyAction1, EnemyAction2, Busy, PlayerFaintedSwitching}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> battleUnits;
    [SerializeField] BattleDialogBox battleDialogueBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState battleState;    

    [SerializeField] List<BattleUnit> turnOrder = new List<BattleUnit>();
    List<int> selectedMoves =  new List<int>();
    List<int> selectedTargets = new List<int>();
    List<int> selectedSwitch = new List<int>();

    [SerializeField] MonsterParty playerParty;
    [SerializeField] MonsterParty enemyParty;

    const int skipIndex = 99;


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
            if(unit.IsPlayerMonster)
            {
                unit.Setup(playerParty.Monsters[battleUnits.IndexOf(unit)]); //returns monsters at index 0 and 1
                unit.Monster.InBattle = true;
                turnOrder.Add(unit);

            }
            else
            {
                unit.Setup(enemyParty.Monsters[battleUnits.IndexOf(unit)-2]); //returns monsters at index 0 and 1 for enemy party which is why we subtract by 2
                unit.Monster.InBattle = true;
                turnOrder.Add(unit);
                
            }
            
            
        }

        partyScreen.Init();

       yield return battleDialogueBox.TypeDialog($"A wild {battleUnits[2].Monster.Base.MonsterName} and {battleUnits[3].Monster.Base.MonsterName} appeared!"); //you can use yield return to call anothe coroutine which is what we are doing here
       
        
       NewTurn();
    }

    void NewTurn()
    {

        turnOrder.Sort(SpeedComparison);
        battleState = BattleState.PlayerAction1; //changing the battle state
        SelectAction();
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

   void SelectAction()
    {
        
        StartCoroutine(battleDialogueBox.TypeDialog("What will you do?"));
        battleDialogueBox.EnableActionSelector(true);

        if(battleState == BattleState.PlayerAction2) battleDialogueBox.EnableBackButton(true);
    }

    public void SelectMove()
    {

        battleDialogueBox.EnableMoveSelector(true);
        
        if(battleState == BattleState.PlayerAction1)
        {
            battleState = BattleState.PlayerMove1;
            battleDialogueBox.EnableActionSelector(false);
            battleDialogueBox.SetMoveNames(battleUnits[0].Monster.Moves);
        } 
        else if(battleState == BattleState.PlayerAction2)
        {
            battleState = BattleState.PlayerMove2;
            battleDialogueBox.EnableActionSelector(false);
            battleDialogueBox.SetMoveNames(battleUnits[1].Monster.Moves);
        } 
        
    }

    public void SelectSwitch()
    {

        if(battleState == BattleState.PlayerAction1) battleState = BattleState.PlayerSwitch1;

        else if(battleState == BattleState.PlayerAction2) battleState = BattleState.PlayerSwitch2;
        
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
        battleDialogueBox.EnableActionSelector(false);
        StartCoroutine(battleDialogueBox.TypeDialog("Select a monster to switch in."));
        
        
    }

    public void Back()
    {

        switch(battleState)
        {
            case BattleState.PlayerMove1:
                battleState = BattleState.PlayerAction1;
                battleDialogueBox.EnableMoveSelector(false);
                SelectAction();
                break;
            case BattleState.PlayerSwitch1:
                battleState = BattleState.PlayerAction1;
                partyScreen.gameObject.SetActive(false);
                SelectAction();
                break;
            case BattleState.PlayerTarget1:
                battleState = BattleState.PlayerAction1; //note our if statment in Fight() method actually changes our battle state again
                selectedMoves.RemoveAt(0);
                selectedSwitch.RemoveAt(0); //clears skip index for switch list
                battleDialogueBox.EnableTargetSelector(false);
                SelectMove();
                break;
            case BattleState.PlayerAction2: 
                switch (selectedMoves[0])
                {
                    case skipIndex: //you selected switch during phase 1
                        battleState = BattleState.PlayerSwitch1;
                        selectedSwitch.RemoveAt(0);
                        selectedMoves.RemoveAt(0);
                        selectedTargets.RemoveAt(0);
                        battleDialogueBox.EnableActionSelector(false);
                        SelectSwitch();
                        break;
                    default: //re-select your target
                        battleState = BattleState.PlayerTarget1;
                        selectedTargets.RemoveAt(0);
                        battleDialogueBox.EnableActionSelector(false);
                        SelectTarget();
                        break;
                }
                break;
            case BattleState.PlayerMove2:
                battleState = BattleState.PlayerAction2;
                battleDialogueBox.EnableMoveSelector(false);
                SelectAction();
                break;
            case BattleState.PlayerSwitch2:
                battleState = BattleState.PlayerAction2;
                partyScreen.gameObject.SetActive(false);
                SelectAction();
                break;
            case BattleState.PlayerTarget2:
                battleState = BattleState.PlayerAction2; //note our if statment in Fight() method actually changes our battle state again
                selectedMoves.RemoveAt(1);
                selectedSwitch.RemoveAt(1); //clears skip index for switch list
                battleDialogueBox.EnableTargetSelector(false);
                SelectMove();
                break;
            case BattleState.PlayerFaintedSwitching:
                StartCoroutine(battleDialogueBox.TypeDialog($"You must select a monster to send out into battle."));
                break;
            default:
                Debug.Log("Something went wrong check your back button method");
                break;


        }


    }



    public void OnMoveSelected(int moveIndex)
    {

        //save selected moves
        if(battleState == BattleState.PlayerMove1)
        {
            battleState = BattleState.PlayerTarget1;
            selectedMoves.Insert(0,moveIndex);
            //skip value for switch
            selectedSwitch.Insert(0,skipIndex);
        }
        else if(battleState == BattleState.PlayerMove2)
        { 
            battleState = BattleState.PlayerTarget2;
            selectedMoves.Insert(1,moveIndex);
            //skip value for switch
            selectedSwitch.Insert(1,skipIndex);
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
        if(battleState == BattleState.PlayerTarget1)
        {

            selectedTargets.Insert(0,targetIndex);
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.PlayerAction2;
            SelectAction();
            
            
            
        }
        else if(battleState == BattleState.PlayerTarget2)
        {
            selectedTargets.Insert(1,targetIndex);
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.EnemyAction1;
            EnemyActionSelection();

        }

    }
    public void OnSwitchSelected(int switchMonsterIndex)
    {
        var selectedMonster = playerParty.Monsters[switchMonsterIndex];

        if(selectedMonster.HP <= 0) //if the slected monster is fainted
        {
            StartCoroutine(battleDialogueBox.TypeDialog($"{selectedMonster} is fainted."));
            return;
        }
        if(selectedMonster.InBattle) //if the slected monster is already in battle
        {
            StartCoroutine(battleDialogueBox.TypeDialog($"{selectedMonster} is already selected for battle."));
            return;
        }
        else if(battleState == BattleState.PlayerSwitch1)
        {
            selectedSwitch.Insert(0,switchMonsterIndex);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(0,skipIndex);
            selectedTargets.Insert(0,skipIndex);

            selectedMonster.InBattle = true; //We set the selected monster inbattle to prevent it from being selected again

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.PlayerAction2;
            SelectAction();
            
        }
        else if(battleState == BattleState.PlayerSwitch2)
        {
            selectedSwitch.Insert(1,switchMonsterIndex);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(1,skipIndex);
            selectedTargets.Insert(1,skipIndex);

            selectedMonster.InBattle = true;

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.EnemyAction1;
            EnemyActionSelection();

        }
        else if(battleState == BattleState.PlayerFaintedSwitching)
        {
            selectedSwitch.Add(switchMonsterIndex);
            selectedMonster.InBattle = true;
            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.Busy; //switch back to busy state to allow for coroutines to finish.
        }
    }

    void EnemyActionSelection()
    {
        //potentially allow enemy to switch for now just attack
        EnemyMoveSelection();
    }

    void EnemyMoveSelection()
    {

        for(int i=2; i < battleUnits.Count; i++)
        {
            int randmomMoveIndex = UnityEngine.Random.Range(0, battleUnits[i].Monster.Moves.Count);
            int randomTargetIndex = UnityEngine.Random.Range(0,1);
            selectedMoves.Insert(i, randmomMoveIndex);
            selectedTargets.Insert(i, randomTargetIndex);
            selectedSwitch.Insert(i,skipIndex);
        }

        StartCoroutine(SwitchMonsters());

        
    }
    
    

    IEnumerator SwitchMonsters()
    {
        battleState = BattleState.Busy;


        for(int i=0; i < turnOrder.Count; i++)
        {
            Monster currentMonster = turnOrder[i].Monster;
            BattleUnit currentUnit = turnOrder[i];

            

            if(selectedSwitch[battleUnits.IndexOf(currentUnit)] == skipIndex)//if no switch was initiated for this turn just continue onto the next turn
            {
                continue;
            }
            else
            {
                if(currentUnit.IsPlayerMonster)
                {
                    Monster incomingMonster = playerParty.Monsters[ selectedSwitch[battleUnits.IndexOf(turnOrder[i])] ];

                    currentUnit.Monster.InBattle = false;
                    yield return battleDialogueBox.TypeDialog
                    ($"{currentMonster.Base.MonsterName} switched out.");

                    //set up new unit
                    currentUnit.Setup(incomingMonster);
                    battleDialogueBox.SetMoveNames(incomingMonster.Moves);
                    yield return battleDialogueBox.TypeDialog($"Go  {incomingMonster.Base.MonsterName}!");

                    //re-order position in party screen
                    playerParty.SwapPartyPositions(currentMonster, incomingMonster);
                    
                }
                else if(!currentUnit.IsPlayerMonster)
                {
                    Monster incomingMonster = enemyParty.Monsters[ selectedSwitch[battleUnits.IndexOf(turnOrder[i])] ];

                    currentUnit.Monster.InBattle = false;
                    yield return battleDialogueBox.TypeDialog
                    ($"{currentMonster.Base.MonsterName} switched out.");

                    //set up new unit
                    currentUnit.Setup(incomingMonster);
                    battleDialogueBox.SetMoveNames(incomingMonster.Moves);
                    yield return battleDialogueBox.TypeDialog($"Enemy Sent out  {incomingMonster.Base.MonsterName}!");

                    //re-order position in party screen
                    enemyParty.SwapPartyPositions(currentMonster, incomingMonster);
                }
                
                

            }



            

            
        }

        selectedSwitch.Clear(); //clear swtichqueue 
        yield return PerformMoves();

        
    }

    

    IEnumerator PerformMoves()
    {
        List<BattleUnit> faintedUnits = new List<BattleUnit>();

        for(int i=0; i < turnOrder.Count; i++)
        {
            Monster attackingMonster = turnOrder[i].Monster;
            BattleUnit attackingUnit = turnOrder[i];
        
            if(selectedMoves[battleUnits.IndexOf(attackingUnit)] == skipIndex || attackingMonster.HP <= 0) //Skip attack check
            {
                continue;
            }

                
            Move attackingMove = attackingMonster.Moves[ selectedMoves[battleUnits.IndexOf(attackingUnit)] ]; 
            Monster targetMonster = battleUnits[ selectedTargets[battleUnits.IndexOf(attackingUnit)] ].Monster;
            BattleUnit targetUnit = battleUnits[ selectedTargets[battleUnits.IndexOf(attackingUnit)] ];

            if(targetMonster.HP <= 0) // check if target is alive
            {
                FindNewTarget(attackingUnit, ref targetMonster, ref targetUnit);

            }

            if(targetMonster == null) //if no new valid target can be found
            {
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} use {attackingMove.Base.MoveName}");
                yield return battleDialogueBox.TypeDialog
                ("But it Failed");
            }
            else //Perform Attack
            {
                attackingMove.AP--; 
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} use {attackingMove.Base.MoveName} on {targetMonster.Base.MonsterName}");

                var damageDetails = targetMonster.TakeDamage(attackingMove, attackingMonster);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);

                if(damageDetails.Fainted)//if the monster FAINTS
                {
                    yield return battleDialogueBox.TypeDialog($"{targetMonster.Base.MonsterName} fainted");
                    faintedUnits.Add(targetUnit);

                }
            }


            
            
        }
        //attack phase over
        selectedMoves.Clear(); 
        selectedTargets.Clear(); 

        //Check for Battle Over

        if (faintedUnits.Count > 0)
        {
            yield return CheckForBattleOver(faintedUnits);
            faintedUnits.Clear(); //clear list of fainted units after checking for battle over
            
        }
        else
        {
            NewTurn();
        }
        
        
    }

    IEnumerator CheckForBattleOver(List<BattleUnit> faintedUnits)
    {
        if(!playerParty.HasHealthyMonster()) //player has no healthy monsters
        {
            Debug.Log("you lose");
        }
        else if(!enemyParty.HasHealthyMonster()) //enemyu has no healthy monsters
        {
            Debug.Log("you win");
        }
        else
        {
            foreach (BattleUnit faintedUnit in faintedUnits)
            {
                if (faintedUnit.IsPlayerMonster && playerParty.CanSwitch())
                {

                    yield return FaintedSwitch(faintedUnit); //pretty sure both of these need to be couroutines or text gets messed up

                }
                else if(!faintedUnit.IsPlayerMonster && enemyParty.CanSwitch())
                {
                    yield return EnemyFaintedSwitch(faintedUnit);
                }
                else
                {
                        break;
                }
            } 
                NewTurn(); //new turn once you break out of loop
        }
    }

    IEnumerator EnemyFaintedSwitch(BattleUnit faintedUnit)
    {
        Monster faintedMonster = faintedUnit.Monster;
        Monster incomingMonster = enemyParty.FindNextHealthyMonster(); //Place Holder for more robust enemy logic
        

        faintedMonster.InBattle = false;
        incomingMonster.InBattle = true;
        faintedUnit.Setup(incomingMonster);
        battleDialogueBox.SetMoveNames(incomingMonster.Moves);
        yield return battleDialogueBox.TypeDialog($"Enemy sent out  {incomingMonster.Base.MonsterName}!"); 
        selectedSwitch.Clear();
        
        //re-order position in party screen
        enemyParty.SwapPartyPositions(faintedMonster, incomingMonster);
    }

    IEnumerator FaintedSwitch(BattleUnit faintedUnit)
    {
        battleState = BattleState.PlayerFaintedSwitching;
        SelectSwitch();

        while(battleState == BattleState.PlayerFaintedSwitching)
        {
            yield return null;
        }

        //Switching out Monster below

        
        Monster faintedMonster = faintedUnit.Monster;
        Monster incomingMonster = playerParty.Monsters[ selectedSwitch[0] ];
        

        faintedMonster.InBattle = false;
        faintedUnit.Setup(incomingMonster);
        battleDialogueBox.SetMoveNames(incomingMonster.Moves);
        yield return battleDialogueBox.TypeDialog($"Go  {incomingMonster.Base.MonsterName}!");
        selectedSwitch.Clear();
        
        //re-order position in party screen
        playerParty.SwapPartyPositions(faintedMonster, incomingMonster);

    }



    void FindNewTarget(BattleUnit attackingUnit, ref Monster targetMonster, ref BattleUnit targetUnit)
    {
        
        if (attackingUnit.IsPlayerMonster && !targetUnit.IsPlayerMonster) //player attacking enemy
        {

            foreach (BattleUnit unit in battleUnits)
            {
                if (!unit.IsPlayerMonster && unit.Monster.HP > 0)
                {
                    
                    targetUnit = unit;
                    targetMonster = unit.Monster;
                    return; //we use return to exit the method completly
                }
                else
                {
                    continue;
                }
            }

            targetMonster = null;
            return;

        }
        else if(!attackingUnit.IsPlayerMonster && targetUnit.IsPlayerMonster) //enemy attacking player
        {
            foreach (BattleUnit unit in battleUnits)
            {
                if (unit.IsPlayerMonster && unit.Monster.HP > 0)
                {
                    targetUnit = unit;
                    targetMonster = unit.Monster;
                    return;
                }
                else
                {
                    continue;
                }
            }
            
            targetMonster = null;
            return;
        }
        else                                                                //targeted ally
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
