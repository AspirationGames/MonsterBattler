using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum BattleState 
{
    PlayerAction1, PlayerAction2, 
    PlayerMove1, PlayerMove2, 
    PlayerTarget1, PlayerTarget2, 
    PlayerSwitch1, PlayerSwitch2, 
    PlayerItem1, PlayerItem2,
    EnemyAction1, EnemyAction2, 
    Busy, PlayerFaintedSwitching, ForgettingMove,BattleOver
    
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<BattleUnit> battleUnits;
    [SerializeField] BattleDialogBox battleDialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryScreen inventoryScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    [SerializeField] EnemyAI enemyAI;

    [SerializeField] Image playerImage;
    [SerializeField] Image enemyImage;

    [SerializeField] GameObject summoningCircle;

    [SerializeField] MoveDetailsUI moveDetailsUI;
    [SerializeField] AudioClip victoryMusic;

    BattleState battleState;

    public event Action<bool> OnBattleOver;    

    [SerializeField] List<BattleUnit> turnOrder = new List<BattleUnit>();
    List<Move> selectedMoves =  new List<Move>();
    List<Monster> selectedSwitch = new List<Monster>();
    List<BattleUnit> selectedTargets = new List<BattleUnit>();
    List<Monster> battleParticipants = new List<Monster>();
    MonsterParty playerParty;
    MonsterParty enemyParty;

    bool isSummonerBattle = false;
    PlayerController player;
    SummonerController summoner;

    public BattleFieldEffects battleFieldEffects {get; set;}

    MoveBase moveToLearn; //new Move monster is trying to learn
    Monster monsterLearning; //The monster trying to learn a new move

    int escapeAttempts;

    
    public void StartWildMonsterBattle(MonsterParty playerParty, MonsterParty wildMonsters, AudioClip battlemusic)
    {
        this.playerParty = playerParty;
        this.enemyParty = wildMonsters;
        
        player = playerParty.GetComponent<PlayerController>();
        
        AudioManager.i.PlayMusic(battlemusic);
        
        isSummonerBattle = false;
        enemyImage.gameObject.SetActive(false); //removes summoner sprite image from battle
        StartCoroutine(SetupBattle());
    }

    public void StartSummonerBattle(MonsterParty playerParty, MonsterParty summonerParty, AudioClip battlemusic)
    {
        this.playerParty = playerParty;
        enemyParty = summonerParty;

        player = playerParty.GetComponent<PlayerController>();
        summoner = summonerParty.GetComponent<SummonerController>();

        AudioManager.i.PlayMusic(battlemusic);

        isSummonerBattle = true;
        StartCoroutine(SetupBattle());
    }
    
    public IEnumerator SetupBattle()
    {  
        //Subscribe to item events
        inventoryScreen.bindingSelected += SelectTarget;

        //Show player sprite
        playerImage.gameObject.SetActive(true);
        playerImage.sprite = player.Sprite;

        foreach(BattleUnit unit in battleUnits) //disable units and huds until summons.
        {
                unit.gameObject.SetActive(false);
                unit.DeactivateUnitHUD();

                
        }
        if(isSummonerBattle) //show trainer and enemey summoner sprites and set dialogue to who challenges you in battle.
        {
            
            enemyImage.gameObject.SetActive(true);
            enemyImage.sprite = summoner.Sprite;

            yield return battleDialogueBox.TypeDialog($"{summoner.Name} challeneges you to a battle.");
        } 


        foreach(BattleUnit unit in battleUnits) //set up player
        {
            

            if(unit.IsPlayerMonster)
            {
                Monster incomingMonster = playerParty.FindNextHealthyMonster();

                if(incomingMonster == null) //in the event there is only one healthy monster
                {
                    continue;
                }

                unit.gameObject.SetActive(true); //reactivates unit
                unit.Setup(incomingMonster); 
                unit.Monster.InBattle = true;
                battleParticipants.Add(unit.Monster);
                turnOrder.Add(unit);

                //sort party selection screen. This is to account for monster fainting during battle but not being switched (APD code)
                playerParty.SortParty(incomingMonster, battleUnits.IndexOf(unit));

                yield return battleDialogueBox.TypeDialog($"{player.Name} summoned {unit.Monster.Base.MonsterName}.");

            }
            else
            {
                Monster incomingMonster = enemyParty.FindNextHealthyMonster();
                
                if(incomingMonster == null) //in the event there is only one healthy monster
                {
                    continue;
                }

                unit.gameObject.SetActive(true); //reactivates unit
                unit.Setup(incomingMonster); 
                unit.Monster.InBattle = true;
                turnOrder.Add(unit);

                if(isSummonerBattle)
                {
                    yield return battleDialogueBox.TypeDialog($"{summoner.Name} summoned {unit.Monster.Base.MonsterName}.");
                }
                else if(!isSummonerBattle)
                {
                    yield return battleDialogueBox.TypeDialog($"A wild {unit.Monster.Base.MonsterName} appeared infront of you.");
                }
                
            }
            
        }

        // if(!isSummonerBattle)
        // {
        //     yield return battleDialogueBox.TypeDialog($"{player.Name} summoned {battleUnits[0].Monster.Base.MonsterName} and {battleUnits[1].Monster.Base.MonsterName}.");
        //     yield return battleDialogueBox.TypeDialog($"A {battleUnits[2].Monster.Base.MonsterName} {battleUnits[3].Monster.Base.MonsterName} appeared infront of you!");
        // }
        // else if(isSummonerBattle)
        // {
        //     yield return battleDialogueBox.TypeDialog($"{player.Name} summoned {battleUnits[0].Monster.Base.MonsterName} {battleUnits[1]?.Monster.Base.MonsterName}.");
        //     yield return battleDialogueBox.TypeDialog($"{summoner.Name} summoned {battleUnits[2].Monster.Base.MonsterName} {battleUnits[3].Monster.Base.MonsterName}.");
        // }
        

        escapeAttempts = 0;
        partyScreen.Init();
        battleFieldEffects = new BattleFieldEffects();

        
        
       
        
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
        if(battleState == BattleState.PlayerAction1)
        {
            if(!battleUnits[0].Monster.InBattle) //player unit 1 is dead
            {
                battleDialogueBox.EnableActionSelector(false);
                battleDialogueBox.EnableRunButton(false);
                battleState = BattleState.PlayerAction2; //this should lead to player action 2

            }
            else
            {
                StartCoroutine(battleDialogueBox.TypeDialog($"What will you do?"));
                battleDialogueBox.EnableActionSelector(true);
                battleDialogueBox.EnableRunButton(true);
            }

        }

        if(battleState == BattleState.PlayerAction2)
        {

            if(!battleUnits[1].isActiveAndEnabled) //player unit 2 is dead or missing
            {

                battleDialogueBox.EnableActionSelector(false);
                battleDialogueBox.EnableRunButton(false);
                SkipUnit(1);
                battleState = BattleState.EnemyAction1;
                EnemyActionSelection();
            }
            else if(!battleUnits[0].isActiveAndEnabled) //if player unit 1 is missing you also need to enable run button
            {
                SkipUnit(0);
                StartCoroutine(battleDialogueBox.TypeDialog($"What will you do?"));
                battleDialogueBox.EnableActionSelector(true);
                battleDialogueBox.EnableRunButton(true);
                battleDialogueBox.EnableBackButton(true);
            }
            else
            {
                StartCoroutine(battleDialogueBox.TypeDialog($"What will you do?"));
                battleDialogueBox.EnableActionSelector(true);
                battleDialogueBox.EnableRunButton(false);
                battleDialogueBox.EnableBackButton(true);
            }
            
            

            //Disable back button if item was used on Action 1
            if(selectedMoves[0] == null && selectedSwitch[0] == null) //player didn't switch or attack during action 1 so the must have used an item
            {
                battleDialogueBox.EnableBackButton(false);
            }
        }

        
    }

    public void OnRun()
    {
        if(isSummonerBattle)
        {
            StartCoroutine(battleDialogueBox.TypeDialog("You can't flee from a summoner battle."));
            return;

        }

        for(int i = 0; i < 2; ++i) //passes turn for player 
        {
            SkipUnit(i);
        }
        battleDialogueBox.EnableActionSelector(false);
        StartCoroutine(AttemptToFlee());


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

        if(battleState == BattleState.PlayerAction1)
        {
            battleState = BattleState.PlayerSwitch1;
            StartCoroutine(battleDialogueBox.TypeDialog("Select a monster to switch in."));
        } 
        else if(battleState == BattleState.PlayerAction2)
        { 
            battleState = BattleState.PlayerSwitch2;
            battleDialogueBox.EnableBackButton(false);
            StartCoroutine(battleDialogueBox.TypeDialog("Select a monster to switch in."));
        }
        
        partyScreen.gameObject.SetActive(true);
        battleDialogueBox.EnableActionSelector(false);
        
        
        
    }

    public void SelectItem() //opens inventory
    {
        if(battleState == BattleState.PlayerAction1) battleState = BattleState.PlayerItem1;

        else if(battleState == BattleState.PlayerAction2)
        { 
            battleState = BattleState.PlayerItem2;
            battleDialogueBox.EnableBackButton(false);
        }
        
        inventoryScreen.gameObject.SetActive(true);
        battleDialogueBox.EnableActionSelector(false);
        StartCoroutine(battleDialogueBox.TypeDialog("Select an item to use."));

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
            case BattleState.PlayerItem1:
                if(inventoryScreen.InventoryScreenState == InventoryScreenState.PartyScreen) //prevents switching battle state back to far if in inventory section
                {
                    inventoryScreen.ResetInventoryState(); //resets inventory screen state to Inventory
                    partyScreen.gameObject.SetActive(false);
                    SelectItem();
                    break;
                }
                else if(inventoryScreen.InventoryScreenState == InventoryScreenState.BindingTargetSelection)
                {
                    inventoryScreen.ResetInventoryState();
                    battleDialogueBox.EnableTargetSelector(false);
                    SelectItem();
                    break;
                }
                battleState = BattleState.PlayerAction1;
                inventoryScreen.gameObject.SetActive(false);
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
                
                if(selectedMoves[0] != null) //you selected a move so reselect your target
                {
                    battleState = BattleState.PlayerTarget1;
                    selectedTargets.RemoveAt(0);
                    battleDialogueBox.EnableActionSelector(false);
                    SelectTarget();
                    break;
                }
                else if(selectedSwitch[0] != null) //you selected to switch return to switch selection
                {
                    battleState = BattleState.PlayerAction1;
                    selectedSwitch.RemoveAt(0);
                    selectedMoves.RemoveAt(0);
                    selectedTargets.RemoveAt(0);
                    battleDialogueBox.EnableActionSelector(false);
                    SelectAction();
                    break;
                }
                else //you selected an item an can't go back
                {
                    Debug.Log("Something went wrong with the back button. Check code.");
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
            case BattleState.PlayerItem2:
                if(inventoryScreen.InventoryScreenState == InventoryScreenState.PartyScreen) //prevents switching battle state back to far if in inventory section
                {
                    inventoryScreen.ResetInventoryState(); //resets inventory screen state to Inventory
                    partyScreen.gameObject.SetActive(false);
                    SelectItem();
                    break;
                }
                else if(inventoryScreen.InventoryScreenState == InventoryScreenState.BindingTargetSelection)
                {
                    inventoryScreen.ResetInventoryState();
                    battleDialogueBox.EnableTargetSelector(false);
                    SelectItem();
                    break;
                }
                battleState = BattleState.PlayerAction2;
                inventoryScreen.gameObject.SetActive(false);
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

        Debug.Log(battleState);

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

    public void OnMoveHover(int moveIndex)
    {
        if(battleState == BattleState.PlayerMove1)
        {
            Monster monster = battleUnits[0].Monster;
            Move move = monster.Moves[moveIndex];
            moveDetailsUI.gameObject.SetActive(true); 
            moveDetailsUI.ShowMoveDetails(move);
        }
        else if(battleState == BattleState.PlayerMove2)
        {
            Monster monster = battleUnits[1].Monster;
            Move move = monster.Moves[moveIndex];
            moveDetailsUI.gameObject.SetActive(true); 
            moveDetailsUI.ShowMoveDetails(move);
        }
    }
    public void OnMoveHoverExit()
    {
        moveDetailsUI.gameObject.SetActive(false);
    }


    void SelectTarget()
    {   
        StartCoroutine(battleDialogueBox.TypeDialog("Select a target"));

        battleDialogueBox.EnableMoveSelector(false);
        battleDialogueBox.EnableTargetSelector(true);
        battleDialogueBox.SetTargetNames(battleUnits);

    }

    public void OnTargetSelected(BattleUnit selectedTarget)
    {

        if( battleState == BattleState.PlayerTarget1 || 
            battleState == BattleState.PlayerTarget2)
        {
            SetMoveTargets(selectedTarget);
        }

        else if(battleState == BattleState.PlayerItem1 || battleState == BattleState.PlayerItem2)
        {
           //try to use binding crystal on target
          StartCoroutine(SetBindingTarget(selectedTarget));
           
        }

    }

    public void SetMoveTargets(BattleUnit targetUnit)
    {
        //save selected targets for moves
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

    
    

    public void OnPartyMemberSelected(int selectedMonsterIndex)
    {
        var selectedMonster = playerParty.Monsters[selectedMonsterIndex];

        if( battleState == BattleState.PlayerSwitch1 || 
            battleState == BattleState.PlayerSwitch2 || 
            battleState == BattleState.PlayerFaintedSwitching)
        {
            SetSwitchSelection(selectedMonster);
        }

        else if(battleState == BattleState.PlayerItem1 || battleState == BattleState.PlayerItem2)
        {
           StartCoroutine( SetItemTarget(selectedMonster) );
        }
    }

    void SetSwitchSelection(Monster selectedMonster)
    {
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

    IEnumerator SetItemTarget(Monster selectedMonster)
    {
        

        if(!inventoryScreen.GetCanUseItem(selectedMonster)) //if the slected monster can't use the selected item
        {
            yield return battleDialogueBox.TypeDialog($"{selectedMonster} can't use that item.");
            yield return battleDialogueBox.TypeDialog($"select a monster to use {inventoryScreen.GetSelectedItem().ItemName}.");
            yield break;
        }
        
        else if(battleState == BattleState.PlayerItem1)
        { 
            //Set Null Values
            selectedMoves.Insert(0,null);
            selectedSwitch.Insert(0, null);
            selectedTargets.Insert(0, null);

            //close party screen
            partyScreen.gameObject.SetActive(false);
            inventoryScreen.gameObject.SetActive(false);

            //Use Item
            yield return UseItem(selectedMonster);

            //player second action selection
            battleState = BattleState.PlayerAction2;
            SelectAction();
            
        }
        else if(battleState == BattleState.PlayerItem2)
        {
            //Set Null Values
            selectedMoves.Insert(1,null);
            selectedSwitch.Insert(1, null);
            selectedTargets.Insert(1, null);
            
            //close party screen and inventory screen
            partyScreen.gameObject.SetActive(false);
            inventoryScreen.gameObject.SetActive(false);
            

            //Use Item
            yield return UseItem(selectedMonster);

            //Enemy Action
            battleState = BattleState.EnemyAction1;
            EnemyActionSelection();
        }

    }

    IEnumerator SetBindingTarget(BattleUnit targetUnit) //place holder coroutine for catching monsters
    {
        
        if(isSummonerBattle)
        {
            yield return battleDialogueBox.TypeDialog($"That monster is already bound to another summoner.");
            yield return battleDialogueBox.TypeDialog("Select a target");
            yield break;
        }
        else if(targetUnit.IsPlayerMonster)
        {
            yield return battleDialogueBox.TypeDialog($"That monster is already bound to you.");
            yield return battleDialogueBox.TypeDialog("Select a target");
            yield break;
        }
        else if(battleState == BattleState.PlayerItem1)
        {
            //Set Null Values
            selectedMoves.Insert(0,null);
            selectedSwitch.Insert(0, null);
            selectedTargets.Insert(0, null);

            //close target selections
            battleDialogueBox.EnableTargetSelector(false);

            //Use Binding Crystal
            inventoryScreen.DecreaseItemQuanity(); //because we are not using the use item method in the inventory screen script
            yield return UsingBindingCrystal(targetUnit);

            //player second action selection
            battleState = BattleState.PlayerAction2;
            SelectAction();

        }
        else if(battleState == BattleState.PlayerItem2)
        {
            //Set Null Values
            selectedMoves.Insert(1,null);
            selectedSwitch.Insert(1, null);
            selectedTargets.Insert(1, null);
            
            //close target selections
            battleDialogueBox.EnableTargetSelector(false);
            

            //Use Binding Crystal
            inventoryScreen.DecreaseItemQuanity();
            yield return UsingBindingCrystal(targetUnit);

            //Enemy Action
            battleState = BattleState.EnemyAction1;
            EnemyActionSelection();

        }
        
        //After binding target is selected make sure to reset inventory state
        inventoryScreen.ResetInventoryState();
        
    }

    void EnemyActionSelection()
    {

        //EnemyMoves();
        RandomEnemyMove();
    }

    void EnemyMoves() //Placeholder for future AI implementation
    {
        for(int i=2; i < battleUnits.Count; i++)
        {   
            Move enemyMove = enemyAI.EnemyMoveSelection(battleUnits, turnOrder, battleUnits[i], battleFieldEffects);
            BattleUnit enemyTarget = enemyAI.EnemyTargetSelection(battleUnits, battleUnits[i], enemyMove, battleFieldEffects);
            selectedMoves.Insert(i, enemyMove);
            selectedTargets.Insert(i, enemyTarget);
            selectedSwitch.Insert(i,null);
        }

        
    }

    void RandomEnemyMove() //Selects Random move for enemy
    {

        for(int i=2; i < battleUnits.Count; i++)
        {
            if(battleUnits[i].isActiveAndEnabled)
            {
                var movesWithPP = battleUnits[i].Monster.Moves.Where(x => x.AP > 0).ToList();
                int randmomMoveIndex = UnityEngine.Random.Range(0, movesWithPP.Count);
                int randomTargetIndex = UnityEngine.Random.Range(0,2);
                selectedMoves.Insert(i, movesWithPP[randmomMoveIndex]);
                selectedTargets.Insert(i, battleUnits[randomTargetIndex]);
                selectedSwitch.Insert(i,null);
            }
            else
            {
                SkipUnit(i);
            }
            
        }

        
        PlayTurn();
        
    }

    void SkipUnit(int i) //sets all action values to null for a given unit inded
    {
        selectedMoves.Insert(i, null);
        selectedTargets.Insert(i, null);
        selectedSwitch.Insert(i,null);
    }
    
    void PlayTurn()
    {
       StartCoroutine( SwitchMonsters() );

    }
    
    IEnumerator UseItem(Monster itemTarget)
    {
        battleState = BattleState.Busy;

        var item = inventoryScreen.GetSelectedItem();
        yield return inventoryScreen.UseItem(itemTarget);
        yield return battleDialogueBox.TypeDialog($"{player.Name} used an {item.ItemName} on {itemTarget.Base.MonsterName}.");

        inventoryScreen.ResetInventoryState();
    }


    

    IEnumerator UsingBindingCrystal(BattleUnit targetUnit)
    {
        battleState = BattleState.Busy;
        BindingCrystal bindingCrystal = (BindingCrystal)inventoryScreen.GetSelectedItem();

        yield return battleDialogueBox.TypeDialog($"{player.Name} used a {bindingCrystal.ItemName}.");
        
        var summoningCircleObj = Instantiate(summoningCircle, targetUnit.transform.position, Quaternion.identity);
        var summoningCircleSprite = summoningCircleObj.GetComponent<SpriteRenderer>();
        summoningCircleSprite.sprite = bindingCrystal.SummoningCircleSprite;
        //Animations
        yield return summoningCircleSprite.transform.DORotate(new Vector3 (0f,0f, -5000f), 2f, RotateMode.Fast).WaitForCompletion();
        yield return targetUnit.PlayBindingAnimation();
        yield return summoningCircleSprite.DOFade(0,0.5f).WaitForCompletion();

        
        int flashCount = AttemptToBindMonster(targetUnit.Monster, bindingCrystal);
                
        for(int x=0; x< Mathf.Min(flashCount, 3); ++x) //Flashing Circle 
        {
            yield return new WaitForSeconds(0.5f);
            var sequence = DOTween.Sequence();
            var originalColor = summoningCircleSprite.color;
            sequence.Join(summoningCircleSprite.DOColor(Color.white,0.5f));
            sequence.Join(summoningCircleSprite.DOBlendableColor(Color.red,0.5f));
            sequence.Join(summoningCircleSprite.DOColor(originalColor,0.5f));
            yield return sequence.WaitForCompletion();
        }

        if(flashCount == 4)
        {
            //monster was binded
            yield return battleDialogueBox.TypeDialog($"{targetUnit.Monster.Base.MonsterName} was bound to you!");

            targetUnit.Monster.InBattle = false;
            turnOrder.Remove(targetUnit);
            targetUnit.Hud.gameObject.SetActive(false);
            enemyParty.RemoveMonster(targetUnit.Monster);

            if(playerParty.Monsters.Count == 4) //player already has max monsters
            {
                //captured monster will be sent to storage
                yield return battleDialogueBox.TypeDialog($"You can't have more than 4 monsters bound to you at once.");
                yield return battleDialogueBox.TypeDialog($"{targetUnit.Monster.Base.MonsterName}'s crystal will be teleported to your private storage at the Academy.");
            }
            else
            {
                playerParty.AddMonster(targetUnit.Monster);
                yield return battleDialogueBox.TypeDialog($"{targetUnit.Monster.Base.MonsterName} has been added to your party");
            }
            
            Destroy(summoningCircleSprite);

            //Check for battle over after using binding crystal
            if(!playerParty.HasHealthyMonster()) //player has no healthy monsters
            {
                yield return battleDialogueBox.TypeDialog("You Lose!");
                yield return new WaitForSeconds(1f);
                BattleOver(false);
            }
            else if(!enemyParty.HasHealthyMonster()) //enemy has no healthy monsters
            {
                yield return battleDialogueBox.TypeDialog("You Win!");
                yield return new WaitForSeconds(1f);
                BattleOver(true);
            }
            

        }
        else
        {
            //binding spell failed
            yield return new WaitForSeconds(1f);
            yield return targetUnit.PlayBreakOutAnimation();

            yield return battleDialogueBox.TypeDialog($"{targetUnit.Monster.Base.MonsterName} broke out of your binding spell.");
            Destroy(summoningCircleSprite);

        }

        inventoryScreen.ResetInventoryState();
    }

    int AttemptToBindMonster(Monster monster, BindingCrystal bindingCrystal)
    {

        float statusBonus = ConditionsDB.GetStatusBonus(monster.Status);
        float catchChance = (3 * monster.MaxHP - 2 * monster.HP) * monster.Base.CatchRate * bindingCrystal.CatchRateModifier * statusBonus / (3 * monster.MaxHP);

        if(catchChance >= 255)
        {
            return 4;
        }
        else
        {
            float f = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / catchChance)); 
            int flashCount = 0;
            while(flashCount < 4)
            {

               if( UnityEngine.Random.Range(0, 65535) >= f)
               {
                   break;
               }

               ++flashCount;
            }

            return flashCount;
        }

    }

    IEnumerator SwitchMonsters()
    {

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


                    //Add monter to list of battle participants if monster has not already been added.
                    if(!battleParticipants.Contains(incomingMonster))
                    {
                        battleParticipants.Add(incomingMonster);
                    }
                    
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
                yield return attackingUnit.Hud.WaitForHPUpdate(); //this is in the event that the status causes damage (i.e. confusion);
                continue;
            }
            yield return StatusChangeDialog(attackingMonster); //expirments add a show status change here too for some reason.
            
            Move attackingMove = selectedMoves[battleUnits.IndexOf(attackingUnit)]; 
            BattleUnit targetUnit = selectedTargets[battleUnits.IndexOf(attackingUnit)];
            Monster targetMonster = targetUnit.Monster;
            attackingMove.AP--; 

            
            if(!targetUnit.isActiveAndEnabled || targetMonster.HP <= 0 || !targetMonster.InBattle) // check if target is alive or has been removed from battle.
            {
                FindNewTarget(attackingUnit, ref targetMonster, ref targetUnit);
            }

            if(targetMonster == null) //if no new valid target can be found
            {
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} used {attackingMove.Base.MoveName}.");
                attackingUnit.PlayAttackAnimation();
                
                yield return battleDialogueBox.TypeDialog
                ("But it Failed");
            }


            else //Perform Attack
            {
                yield return battleDialogueBox.TypeDialog
                ($"{attackingMonster.Base.MonsterName} used {attackingMove.Base.MoveName}.");
                
                attackingUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(1f);

                if(targetMonster.IsProtected)
                {
                    yield return battleDialogueBox.TypeDialog($"{targetMonster.Base.MonsterName} was protected from the attack by a {targetMonster.ProtectedStatus.Name}.");
                    continue;
                }
                
                if(AccuracyCheck(attackingMove, attackingMonster, targetMonster)) //accuracy check
                {
                    if(attackingMove.Base.Category == MoveCategory.Status) //If move is a status move
                    {
                        yield return PerformEffects(attackingMove.Base.Effects, attackingMonster, targetMonster, attackingMove.Base.Target);
                        continue;
                    }
                    // if target monster is protected

                    else
                    {
                        targetUnit.PlayHitAnimation();
                        var damageDetails = targetMonster.TakeDamage(attackingMove, attackingMonster, battleFieldEffects.Weather);
                        yield return targetUnit.Hud.WaitForHPUpdate();
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
                        faintedUnits.Add(targetUnit);
                        yield return HandleMonsterKill(targetUnit);
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
                    yield return unit.Hud.WaitForHPUpdate();

                    if(unit.Monster.HP <= 0)//if the monster FAINTS
                    {
                        yield return battleDialogueBox.TypeDialog($"{unit.Monster.Base.MonsterName} fainted.");
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
                    yield return battleDialogueBox.TypeDialog($"The harsh weather has cleared up.");
                }
            }

            if(battleFieldEffects.WarpDuration!= null) //Time Warp Update
            {
                battleFieldEffects.WarpDuration--;

                if(battleFieldEffects.WarpDuration == 0)
                {
                    battleFieldEffects.TimeWarp = null;
                    battleFieldEffects.WarpDuration = null;
                    yield return battleDialogueBox.TypeDialog($"The dimensions of time have reverted to their normal state.");
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

    IEnumerator HandleMonsterKill(BattleUnit faintedUnit)
    {
        //Fainted
        faintedUnit.Monster.InBattle = false;
        faintedUnit.PlayFaintAnimation();
        faintedUnit.Hud.gameObject.SetActive(false); 
        yield return battleDialogueBox.TypeDialog($"{faintedUnit.Monster.Base.MonsterName} fainted.");
        
        if(!faintedUnit.IsPlayerMonster) //should only gain exp if player. 
        {
            //Check for victory to play victory music
            if(!enemyParty.HasHealthyMonster()) //enemy has no healthy monsters
            {
                AudioManager.i.PlayMusic(victoryMusic);
            }

            //EXP gain calculation
            int b = faintedUnit.Monster.Base.ExpYield;
            int lvl = faintedUnit.Monster.Level;
            float summonerBonus = (isSummonerBattle) ? 1.5f : 1f; //summoner battles provide more exp

            foreach(Monster monster in battleParticipants)
            {
                if(monster.HP > 0)
                {
                    int expGain = Mathf.FloorToInt(( (b*lvl * summonerBonus) / 7)); 

                    //Add EXP to monsters
                    monster.Exp += expGain;
                    if(monster.InBattle) // update HUD for monsters in battle the we update HUD
                    {
                        yield return battleUnits[playerParty.Monsters.IndexOf(monster)].Hud.SetExpSmooth();
                    }
                    //Check for Level Up
                   while ( monster.CheckForLevelUp() ) //A while loop will make sure if a monster gains multiple levels then it will continue to adjust
                   {
                       if(monster.InBattle) //if in battle we update HUD
                       {
                           battleUnits[playerParty.Monsters.IndexOf(monster)].Hud.SetLevel();
                           yield return battleUnits[playerParty.Monsters.IndexOf(monster)].Hud.SetExpSmooth(true); //in the event the monster gained more exp than required to level up
                       }
                       yield return battleDialogueBox.TypeDialog($"{monster.Base.MonsterName} leveled up!");

                       //check for new moves
                       var newMove = monster.GetLearnableMoveAtCurrLevel();
                        if(newMove != null)
                        {
                            if(monster.Moves.Count < MonsterBase.MaxNumberOfMoves)
                            {
                                monster.LearnMove(newMove.Base);

                                yield return battleDialogueBox.TypeDialog($"{monster.Base.MonsterName} learned {newMove.Base.MoveName}!");

                            }
                            else
                            {
                                //player will need to forget a move
                                yield return battleDialogueBox.TypeDialog($"{monster.Base.MonsterName} wants to learn {newMove.Base.MoveName}.");
                                yield return battleDialogueBox.TypeDialog($"But {monster.Base.MonsterName} already {MonsterBase.MaxNumberOfMoves} moves.");
                                yield return battleDialogueBox.TypeDialog($"Forget a move so that {monster.Base.MonsterName} can learn {newMove.Base.MoveName}.");
                                //add in yes or no option
                                yield return ChooseMoveToForget(monster, newMove.Base);
                                

                            }
                        }

                   }
                    

                }
                else
                {
                    continue;
                }
                
            }

        }  
        
    }

    public void OnMoveForget(int moveIndex)
    {
        if(moveIndex == MonsterBase.MaxNumberOfMoves)
        {
            //player doesn't want to learn new move
            StartCoroutine(battleDialogueBox.TypeDialog($"{monsterLearning.Base.MonsterName} did not learn {moveToLearn.MoveName}."));
        }
        else
        {
            //replace selected move with new move
            var selectedMove = monsterLearning.Moves[moveIndex].Base;
            StartCoroutine(battleDialogueBox.TypeDialog($"{monsterLearning.Base.MonsterName} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}."));

            monsterLearning.Moves[moveIndex] = new Move(moveToLearn); //replaces selected move with instance of the new move

        }

        moveToLearn = null;
        monsterLearning = null;

        battleState = BattleState.Busy;

    }

    IEnumerator ChooseMoveToForget(Monster monster, MoveBase newMove)
    {
        battleState = BattleState.ForgettingMove;
        yield return battleDialogueBox.TypeDialog("Choose a move to forget.");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(monster.Moves.Select(x => x.Base).ToList(), newMove);
        moveSelectionUI.SetMonsterImage(monster);
        monsterLearning = monster;
        moveToLearn = newMove;
        
        yield return new WaitUntil(() => battleState != BattleState.ForgettingMove);
        moveSelectionUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

    }




    IEnumerator RunAfterTurn(List<BattleUnit> faintedUnits)
    {
        foreach(BattleUnit unit in battleUnits)
        {
            if(!unit.isActiveAndEnabled) continue;

            unit.Monster.ResetProtect();

            if(unit.Monster.HP > 0 && unit.Monster.InBattle)
            {
                unit.Monster.OnAfterTurn();
                yield return StatusChangeDialog(unit.Monster);
                //yield return unit.Hud.UpdateHPAsync();

                

                if(unit.Monster.HP <= 0)//if the monster FAINTS
                {
                    yield return HandleMonsterKill(unit);
                    faintedUnits.Add(unit);
                }
            }
            else
            {
                continue;
            } 
            
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
                        yield return battleDialogueBox.TypeDialog($"The dimensions of time have reverted to their normal state.");
                    }

                }
                if(effects.Protect != ConditionID.none)
                {
                    if(moveTarget == MoveTarget.Self) //protecting self
                    {
                        attackingMonster.Protect(effects.Protect);
                        yield return StatusChangeDialog(attackingMonster);
                    }
                    else //protecting ally
                    {
                        targetMonster.Protect(effects.Protect);
                        yield return StatusChangeDialog(targetMonster);
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
            yield return battleDialogueBox.TypeDialog("You Lose!");
            yield return new WaitForSeconds(1f);
            BattleOver(false);
        }
        else if(!enemyParty.HasHealthyMonster()) //enemy has no healthy monsters
        {
            yield return battleDialogueBox.TypeDialog("You Win!");
            yield return new WaitForSeconds(1f);
            BattleOver(true);
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

        //incoming monster is set to be in battle at time of selection.
        faintedUnit.Setup(incomingMonster);
        battleDialogueBox.SetMoveNames(incomingMonster.Moves);
        yield return battleDialogueBox.TypeDialog($"Go  {incomingMonster.Base.MonsterName}!");
        selectedSwitch.Clear();
        
        //re-order position in party screen
        playerParty.SwapPartyPositions(faintedMonster, incomingMonster);

        //Add monter to list of battle participants if monster has not already been added.
        if(!battleParticipants.Contains(incomingMonster))
        {
            battleParticipants.Add(incomingMonster);
        }

    }



    void FindNewTarget(BattleUnit attackingUnit, ref Monster targetMonster, ref BattleUnit targetUnit)
    {
        
        if (attackingUnit.IsPlayerMonster && !targetUnit.IsPlayerMonster) //player attacking enemy
        {

            foreach (BattleUnit unit in battleUnits)
            {
                if(!unit.isActiveAndEnabled) continue;

                else if (!unit.IsPlayerMonster && unit.Monster.HP > 0 && unit.Monster.InBattle)
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
                if(!unit.isActiveAndEnabled) continue;

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

    IEnumerator AttemptToFlee()
    {
        battleState = BattleState.Busy;
        ++ escapeAttempts;

        
        int playerSpeed = playerParty.Monsters[UnityEngine.Random.Range(0, playerParty.Monsters.Count)].Speed;
        int enemeySpeed = enemyParty.Monsters[UnityEngine.Random.Range(0, enemyParty.Monsters.Count)].Speed;

        if(enemeySpeed < playerSpeed)
        {
            yield return battleDialogueBox.TypeDialog("You flee from the wild monsters!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemeySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return battleDialogueBox.TypeDialog("You flee from the wild monsters!");
                BattleOver(true);
            }
            else
            {

                yield return battleDialogueBox.TypeDialog("You couldn't escape.");
                battleState = BattleState.EnemyAction1;
                EnemyActionSelection();
            }
        }

    }
    

    void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;

        if(!isSummonerBattle) //if wild encounter we need to clear the enemy party to reset for next wild encounter.
        {
            enemyParty.Monsters.Clear();
        }

        
        battleUnits.ForEach(u => u.Hud.ClearData()); //unsubsribes units from events until start of next battle
        turnOrder.Clear(); //reset all units in turn.
        battleParticipants.Clear(); //clear list of battle participants
        playerParty.Monsters.ForEach(m => m.OnBattleOver()); //rests for each monster in party
        OnBattleOver(won);

    }

    

}
