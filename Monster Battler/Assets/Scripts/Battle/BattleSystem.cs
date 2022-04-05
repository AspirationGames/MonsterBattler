using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState 
{PlayerAction1, PlayerAction2, PlayerMove1, PlayerMove2, PlayerTarget1, PlayerTarget2, PlayerSwitch1, PlayerSwitch2, EnemyAction1, EnemyAction2, Busy, PlayerFaintedSwitching, BattleOver}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> battleUnits;
    [SerializeField] BattleDialogBox battleDialogueBox;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] EnemyAI enemyAI;

    BattleState battleState;    

    [SerializeField] List<BattleUnit> turnOrder = new List<BattleUnit>();
    List<Move> selectedMoves =  new List<Move>();
    List<Monster> selectedSwitch = new List<Monster>();
    List<BattleUnit> selectedTargets = new List<BattleUnit>();

    [SerializeField] MonsterParty playerParty;
    [SerializeField] MonsterParty enemyParty;

    public BattleFieldEffects battleFieldEffects {get; set;}


    void Start()
    {
        
        
    }

    void StartRandomEncounter()
    {
        //this.playerParty = playerParty;
        //this.enemyParty = untamedMonsters;
        //StartCoroutine(SetupBattle());

        StartCoroutine(SetupBattle());
    }

    void StartDruidBattle(MonsterParty playerParty, MonsterParty druidParty)
    {
        //this.playerParty = playerParty;
        //this.enemyParty = druidParty;
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
        battleFieldEffects = new BattleFieldEffects();

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
        if(battleFieldEffects.TimeWarp != null) //time warp speed priority
        {
            
            if(a.Monster.Speed < b.Monster.Speed)
            {
                return -1; //we return negative 1 here because -1 means we are moving it up in the list or "to the left"
            }
            else if(a.Monster.Speed > b.Monster.Speed)
            {
                return 1;
            }
            return 0;

        }
        else //normal speed check
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
                    case null: //you selected switch during phase 1
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
            Move selectedMove = battleUnits[0].Monster.Moves[moveIndex];
            if(selectedMove.AP < 1)
            {
                StartCoroutine(battleDialogueBox.TypeDialog($"{selectedMove} is out of AP."));
                return;
            }
            else
            {
                battleState = BattleState.PlayerTarget1;
                selectedMoves.Insert(0,selectedMove);
                selectedSwitch.Insert(0,null);//skip value for switch

                if(selectedMove.Base.Target == MoveTarget.Self) //if move target is self
                {
                    selectedTargets.Insert(0,battleUnits[0]); 
                    battleDialogueBox.EnableMoveSelector(false);
                    
                    battleState = BattleState.PlayerAction2;
                    SelectAction();
                }
                else
                {
                    SelectTarget();
                }
            }   
        }
        else if(battleState == BattleState.PlayerMove2)
        { 
            Move selectedMove = battleUnits[1].Monster.Moves[moveIndex];
            if(selectedMove.AP < 1)
            {
                StartCoroutine(battleDialogueBox.TypeDialog($"{selectedMove} is out of AP."));
                return;
            }
            else
            {
                battleState = BattleState.PlayerTarget2;
                selectedMoves.Insert(1,selectedMove);
                selectedSwitch.Insert(1,null);//skip value for switch

                if(selectedMove.Base.Target == MoveTarget.Self) //if move target is self
                {
                    selectedTargets.Insert(1,battleUnits[1]); 
                    battleDialogueBox.EnableMoveSelector(false);

                    battleState = BattleState.EnemyAction1;
                    EnemyActionSelection();
                    
                }
                else
                {
                    SelectTarget();
                }
            }
            
        }

        

    }


    void SelectTarget()
    {   
        battleDialogueBox.EnableMoveSelector(false);
        battleDialogueBox.EnableTargetSelector(true);
        battleDialogueBox.SetTargetNames(battleUnits);

    }

    public void OnTargetSelected(BattleUnit targetUnit)
    {
        //save selected targets
        if(battleState == BattleState.PlayerTarget1)
        {

            selectedTargets.Insert(0,targetUnit);
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.PlayerAction2;
            SelectAction();
            
            
            
        }
        else if(battleState == BattleState.PlayerTarget2)
        {
            selectedTargets.Insert(1,targetUnit);
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
            selectedSwitch.Insert(0,selectedMonster);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(0,null);
            selectedTargets.Insert(0,null);

            selectedMonster.InBattle = true; //We set the selected monster inbattle to prevent it from being selected again

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.PlayerAction2;
            SelectAction();
            
        }
        else if(battleState == BattleState.PlayerSwitch2)
        {
            selectedSwitch.Insert(1,selectedMonster);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(1,null);
            selectedTargets.Insert(1,null);

            selectedMonster.InBattle = true;

            partyScreen.gameObject.SetActive(false);
            battleDialogueBox.EnableBackButton(false);
            battleState = BattleState.EnemyAction1;
            //EnemyActionSelection();
            EnemyActionSelection();

        }
        else if(battleState == BattleState.PlayerFaintedSwitching)
        {
            selectedSwitch.Add(selectedMonster);
            selectedMonster.InBattle = true;
            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.Busy; //switch back to busy state to allow for coroutines to finish.
        }
    }

    void EnemyActionSelection()
    {

        //EnemyMoves();
        RandomEnemyMove();
    }

    void EnemyMoves()
    {
        for(int i=2; i < battleUnits.Count; i++)
        {   
            Move enemyMove = enemyAI.EnemyMoveSelection(battleUnits, turnOrder, battleUnits[i], battleFieldEffects);
            BattleUnit enemyTarget = enemyAI.EnemyTargetSelection(battleUnits, battleUnits[i], enemyMove, battleFieldEffects);
            selectedMoves.Insert(i, enemyMove);
            selectedTargets.Insert(i, enemyTarget);
            selectedSwitch.Insert(i,null);
        }

        StartCoroutine(SwitchMonsters());
    }

    void RandomEnemyMove() //Selects Random move for enemy
    {

        for(int i=2; i < battleUnits.Count; i++)
        {
            var movesWithPP = battleUnits[i].Monster.Moves.Where(x => x.AP > 0).ToList();
            
            int randmomMoveIndex = UnityEngine.Random.Range(0, movesWithPP.Count);
            int randomTargetIndex = UnityEngine.Random.Range(0,1);
            selectedMoves.Insert(i, movesWithPP[randmomMoveIndex]);
            selectedTargets.Insert(i, battleUnits[randomTargetIndex]);
            selectedSwitch.Insert(i,null);
        }

        StartCoroutine(SwitchMonsters());

        
    }
    
    

    IEnumerator SwitchMonsters()
    {
        battleState = BattleState.Busy;


        for(int i=0; i < turnOrder.Count; i++)
        {
            BattleUnit currentUnit = turnOrder[i];
            Monster currentMonster = currentUnit.Monster;


            
            if(selectedSwitch[battleUnits.IndexOf(currentUnit)] == null)//if no switch was initiated for this turn just continue onto the next turn
            {
                continue;
            }
            else
            {
                if(currentUnit.IsPlayerMonster)
                {
                    Monster incomingMonster = selectedSwitch[battleUnits.IndexOf(turnOrder[i])];

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
                    Monster incomingMonster = selectedSwitch[battleUnits.IndexOf(turnOrder[i])];

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
    
    int CheckMovePriority(BattleUnit a, BattleUnit b)
    {
        Move moveA = selectedMoves[battleUnits.IndexOf(a)];
        Move moveB = selectedMoves[battleUnits.IndexOf(b)];
        if(moveA == null || moveB == null)
        {
            return 0;
        }

        int priorityA = moveA.Base.Priority;
        int priorityB = moveB.Base.Priority;
        
        if(priorityA > priorityB)
        {
            return -1; //we return negative 1 here because -1 means we are moving it up in the list or "to the left"
        }
        else if(priorityA < priorityB)
        {
            return 1;
        }

        return 0;

    }

    IEnumerator PerformMoves()
    {
        

        List<BattleUnit> faintedUnits = new List<BattleUnit>(); //list to keep track of fainted units
        turnOrder.Sort(CheckMovePriority); //check for priority moves

        for(int i=0; i < turnOrder.Count; i++)
        {
            BattleUnit attackingUnit = turnOrder[i];
            Monster attackingMonster = attackingUnit.Monster;
            bool canAttack = attackingMonster.OnBeforeMove();
        
            if(selectedMoves[battleUnits.IndexOf(attackingUnit)] == null || attackingMonster.HP <= 0) //Skip attack check
            {
                continue;
            }
            if(!canAttack)
            {
                yield return StatusChangeDialog(attackingMonster);
                yield return attackingUnit.Hud.UpdateHP(); //this is in the event that the status causes damage (i.e. confusion);
                continue;
            }
            yield return StatusChangeDialog(attackingMonster); //expirments add a show status change here too for some reason.
            
            Move attackingMove = selectedMoves[battleUnits.IndexOf(attackingUnit)]; 
            BattleUnit targetUnit = selectedTargets[battleUnits.IndexOf(attackingUnit)];
            Monster targetMonster = targetUnit.Monster;
            attackingMove.AP--; 

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
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} use {attackingMove.Base.MoveName} on {targetMonster.Base.MonsterName}");
                
                if(AccuracyCheck(attackingMove, attackingMonster, targetMonster)) //accuracy check
                {
                    if(attackingMove.Base.Category == MoveCategory.Status) //If move is a status move
                    {
                        yield return PerformEffects(attackingMove.Base.Effects, attackingMonster, targetMonster, attackingMove.Base.Target);
                        continue;
                    }
                    else
                    {
                        var damageDetails = targetMonster.TakeDamage(attackingMove, attackingMonster, battleFieldEffects.Weather);
                        yield return targetUnit.Hud.UpdateHP();
                        yield return ShowDamageDetails(damageDetails);
                    }

                    //Secondary Effects, Likely need to go back and review HP check in order to apply recoil ect in the event target faints
                    if(attackingMove.Base.SecondaryEffects != null && attackingMove.Base.SecondaryEffects.Count > 0 && targetMonster.HP > 0 )
                    {
                        foreach(var secondaryEffect in attackingMove.Base.SecondaryEffects)
                        {
                            int rng = UnityEngine.Random.Range(1, 101); //check chance of secondary effect occuring
                            if(secondaryEffect.Chance >= rng)
                            {
                                yield return PerformEffects(secondaryEffect, attackingMonster, targetMonster, secondaryEffect.Target);
                            }
                        }

                    }

                    if(targetUnit.Monster.HP <= 0)//if the monster FAINTS
                    {
                    yield return battleDialogueBox.TypeDialog($"{targetMonster.Base.MonsterName} fainted");
                    faintedUnits.Add(targetUnit);
                    }
                }
                else //attack missed
                {
                    yield return battleDialogueBox.TypeDialog($"but it missed!");

                }
            }        
        }
        //attack phase over
        selectedMoves.Clear(); 
        selectedTargets.Clear(); 

        //After Turn Effects
        yield return RunAfterTurn(faintedUnits);

        //BattleField Effects
        if(battleFieldEffects.Weather != null)
        {
            yield return battleDialogueBox.TypeDialog(battleFieldEffects.Weather.EffectMessage);

            foreach(BattleUnit unit in turnOrder)
            {
                if(unit.Monster.HP > 0)
                {
                    battleFieldEffects.Weather.OnWeather?.Invoke(unit.Monster);
                    yield return StatusChangeDialog(unit.Monster);
                    yield return unit.Hud.UpdateHP();

                    if(unit.Monster.HP <= 0)//if the monster FAINTS
                    {
                        yield return battleDialogueBox.TypeDialog($"{unit.Monster.Base.MonsterName} fainted");
                        faintedUnits.Add(unit);
                    }
                } 
            }

            if(battleFieldEffects.WeatherDuration != null) //Weather Duration Update
            {
                battleFieldEffects.WeatherDuration--;

                if(battleFieldEffects.WeatherDuration == 0)
                {
                    battleFieldEffects.Weather = null;
                    battleFieldEffects.WeatherDuration = null;
                    yield return battleDialogueBox.TypeDialog($"The harsh weather has cleared up");
                }
            }

            if(battleFieldEffects.WarpDuration!= null) //Time Warp Update
            {
                battleFieldEffects.WarpDuration--;

                if(battleFieldEffects.WarpDuration == 0)
                {
                    battleFieldEffects.TimeWarp = null;
                    battleFieldEffects.WarpDuration = null;
                    yield return battleDialogueBox.TypeDialog($"The dimensions of time have reverted to their normal state");
                }
            }
        }
        
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

    IEnumerator RunAfterTurn(List<BattleUnit> faintedUnits)
    {
        foreach(BattleUnit unit in battleUnits)
        {
            if(unit.Monster.HP > 0)
            {
                unit.Monster.OnAfterTurn();
                yield return StatusChangeDialog(unit.Monster);
                yield return unit.Hud.UpdateHP();

                

                if(unit.Monster.HP <= 0)//if the monster FAINTS
                {
                    yield return battleDialogueBox.TypeDialog($"{unit.Monster.Base.MonsterName} fainted");
                    faintedUnits.Add(unit);
                }
            }
            else continue;
        }


    }

    bool AccuracyCheck(Move attackingMove, Monster attackingMonster, Monster targetMonster)
    {   

        if(attackingMove.Base.AlwaysHits)
        {
            return true;
        }
        
        int requiredAccuracy = UnityEngine.Random.Range(1, 101);
        float moveAccuracy = attackingMove.Base.Accuracy;
        int accuracyStage = attackingMonster.StatStages[Stat.Accuracy];
        int evasionStage = targetMonster.StatStages[Stat.Evasion];


        var stageModifiers = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        //Accuracy Stage Modifiers
        if(accuracyStage > 0)
        {
            moveAccuracy *= stageModifiers[accuracyStage];
            
        }
        else if(accuracyStage < 0)
        {
            moveAccuracy /= stageModifiers[-accuracyStage];
            
        }
        //Evasion Stage Modifiers
        if(evasionStage > 0)
        {
            moveAccuracy /= stageModifiers[evasionStage];
        }
        else if(accuracyStage < 0)
        {
            moveAccuracy *= stageModifiers[-evasionStage];
        }
        

        return moveAccuracy >= requiredAccuracy; 

    }

    IEnumerator PerformEffects(MoveEffects effects, Monster attackingMonster, Monster targetMonster, MoveTarget moveTarget)
    {

                if(effects.StageChanges != null) //stat change effects
                {
                    if(moveTarget == MoveTarget.Self) //self inflicted stagechange
                    {
                        attackingMonster.ApplyStageChange(effects.StageChanges);
                        yield return StatusChangeDialog(attackingMonster);
                    }
                    else
                    {
                        targetMonster.ApplyStageChange(effects.StageChanges);
                        yield return StatusChangeDialog(targetMonster);
                    }
                }
                if(effects.StatusEffect != ConditionID.none) //status effects
                {
                    targetMonster.SetStatus((ConditionID)effects.StatusEffect);
                    yield return StatusChangeDialog(targetMonster);
                }
                if(effects.VolatileStatusEffect != ConditionID.none) //volatie status effects
                {
                    targetMonster.SetVolatileStatus((ConditionID)effects.VolatileStatusEffect);
                    yield return StatusChangeDialog(targetMonster);
                }
                if(effects.Weather != ConditionID.none) //weather effects
                {
                    battleFieldEffects.SetWeather(effects.Weather);
                    battleFieldEffects.WeatherDuration = 5;
                    yield return battleDialogueBox.TypeDialog($"{battleFieldEffects.Weather.StartMessage}");
                    
                }
                if(effects.TimeWarp != ConditionID.none)
                {
                    if(battleFieldEffects.TimeWarp == null)
                    {
                        battleFieldEffects.SetWarp(effects.TimeWarp);
                        battleFieldEffects.WarpDuration = 5;
                        yield return battleDialogueBox.TypeDialog($"{battleFieldEffects.TimeWarp.StartMessage}");
                    }
                    else
                    {
                        battleFieldEffects.TimeWarp = null;
                        battleFieldEffects.WarpDuration = null;
                        yield return battleDialogueBox.TypeDialog($"The dimensions of time have reverted to their normal state");
                    }
                

                }

    }

    IEnumerator StatusChangeDialog(Monster monster)
    {
        while (monster.StatusChangeMessages.Count > 0)
        {
            var message = monster.StatusChangeMessages.Dequeue(); //takes message from Queue
            yield return battleDialogueBox.TypeDialog(message);
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
        Monster incomingMonster = selectedSwitch[0];
        

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
    

    void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;
        playerParty.Monsters.ForEach(m => m.OnBattleOver()); //rests for each monster in party
        //OnBattleOver(won);

    }

    

}
