using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState {PlayerAction1, PlayerAction2, EnemyMove1, EnemmyMove2, Busy, PartyScreen}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> battleUnits;
    [SerializeField] List<BattleHud> battleHuds;

    [SerializeField] BattleDialogBox battleDialogueBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState battleState;    

    [SerializeField] List<BattleUnit> turnOrder = new List<BattleUnit>();
    List<int> selectedMoves =  new List<int>();
    List<int> selectedTargets = new List<int>();
    List<int> selectedSwitch = new List<int>();

    [SerializeField] MonsterParty playerParty;
    [SerializeField] MonsterParty enemyParty;

    int skipIndex = 99;


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
                unit.Setup(playerParty.Monsters[battleUnits.IndexOf(unit)]); //returns monsters at index 0 and 1
                battleHuds[battleUnits.IndexOf(unit)].SetData(unit.Monster);
                unit.Monster.InBattle = true;
                turnOrder.Add(unit);

            }
            else
            {
                unit.Setup(enemyParty.Monsters[battleUnits.IndexOf(unit)-2]); //returns monsters at index 0 and 1 for enemy party which is why we subtract by 2
                battleHuds[battleUnits.IndexOf(unit)].SetData(unit.Monster);
                unit.Monster.InBattle = true;
                turnOrder.Add(unit);
                
            }
            
            
        }

        partyScreen.Init();

       yield return battleDialogueBox.TypeDialog($"A wild {battleUnits[2].Monster.Base.MonsterName} and {battleUnits[3].Monster.Base.MonsterName} appeared!"); //you can use yield return to call anothe coroutine which is what we are doing here
       
        
        TurnOrder();
        battleState = BattleState.PlayerAction1; //changing the battle state
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
        
        StartCoroutine(battleDialogueBox.TypeDialog("What will you do?"));
        battleDialogueBox.EnableActionSelector(true);
    }

    public void Fight()
    {

        battleDialogueBox.EnableMoveSelector(true);
        
        if(battleState == BattleState.PlayerAction1)
        {
            battleDialogueBox.EnableActionSelector(false);
            battleDialogueBox.SetMoveNames(battleUnits[0].Monster.Moves);
        } 
        else if(battleState == BattleState.PlayerAction2)
        {
            battleDialogueBox.EnableTargetSelector(false);
            battleDialogueBox.SetMoveNames(battleUnits[1].Monster.Moves);
        } 
        
    }

    public void Switch()
    {

        
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
        battleDialogueBox.EnableActionSelector(false);
        StartCoroutine(battleDialogueBox.TypeDialog("Select a monster to switch in."));
        
        
    }



    public void OnMoveSelected(int moveIndex)
    {

        //save selected moves
        if(battleState == BattleState.PlayerAction1)
        {
            selectedMoves.Insert(0,moveIndex);
            //skip value for switch
            selectedSwitch.Insert(0,skipIndex);
        }
        else if(battleState == BattleState.PlayerAction2)
        { 
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
        if(battleState == BattleState.PlayerAction1)
        {

            selectedTargets.Insert(0,targetIndex);
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.PlayerAction2;
            PlayerAction();
            
            
            
        }
        else if(battleState == BattleState.PlayerAction2)
        {
            selectedTargets.Insert(1,targetIndex);
            battleDialogueBox.EnableTargetSelector(false);
            battleState = BattleState.EnemyMove1;
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
            StartCoroutine(battleDialogueBox.TypeDialog($"{selectedMonster} is already in battle."));
            return;
        }
        else if(battleState == BattleState.PlayerAction1)
        {
            selectedSwitch.Insert(0,switchMonsterIndex);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(0,skipIndex);
            selectedTargets.Insert(0,skipIndex);

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.PlayerAction2;
            PlayerAction();
            
        }
        else if(battleState == BattleState.PlayerAction2)
        {
            selectedSwitch.Insert(1,switchMonsterIndex);
            // dummy numbers  to allow code to work
            selectedMoves.Insert(1,skipIndex);
            selectedTargets.Insert(1,skipIndex);

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.EnemyMove1;
            EnemyActionSelection();

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
                Monster incomingMonster = playerParty.Monsters[ selectedSwitch[battleUnits.IndexOf(turnOrder[i])] ];
                currentUnit.Monster.InBattle = false;
                yield return battleDialogueBox.TypeDialog
                ($"{currentMonster.Base.MonsterName} switched out. You sent out {incomingMonster.Base.MonsterName}");
                turnOrder.Remove(currentUnit);
            }

            //Need to add more logic for switch out and in.

            
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
            Move attackingMove = attackingMonster.Moves[ selectedMoves[battleUnits.IndexOf(attackingUnit)] ]; 
            Monster targetMonster = battleUnits[ selectedTargets[battleUnits.IndexOf(attackingUnit)] ].Monster;
            BattleHud targetHud = battleHuds[ selectedTargets[battleUnits.IndexOf(attackingUnit)] ];
            BattleUnit targetUnit = battleUnits[ selectedTargets[battleUnits.IndexOf(attackingUnit)] ];

            // need to find way of changing target in the even that target dies mid turn. 

            if(targetMonster.HP <= 0) // check if target is alive
            {
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
                    faintedUnits.Add(targetUnit);

                }
            }


            
            
        }
        //attack phase over
        selectedMoves.Clear(); //clear move queu
        selectedTargets.Clear(); //clear target queu

        if (faintedUnits.Count > 0)
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
                foreach(BattleUnit faintedUnit in faintedUnits)
                {
                    if (faintedUnit.isPlayerMonster)
                    {
                        //Switch Out Monster
                        Debug.Log("you need to swtich in new monsters");
                    }
                }
            }
        }
        else
        {
            PlayerAction();
            battleState = BattleState.PlayerAction1;
        }
        
        
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
                else
                {
                    targetMonster = null;
                    return;
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
                else
                {
                    targetMonster = null;
                    return;
                }
            }
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
