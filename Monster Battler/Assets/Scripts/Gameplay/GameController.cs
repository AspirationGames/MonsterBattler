using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene, Paused, PartyManagement, Inventory}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryScreen inventoryScreen;

    Animator cameraAnimator;
    GameState gameState;
    GameState previousState;
    GameState stateBeforePause;

    
    public GameState PreviousState {get {return previousState;}}
    public SceneDetails CurrentScene {get; private set;}

    public SceneDetails PreviousScene {get; private set;}

    public static GameController Instance {get; private set;}

    Fader fader;

    private void Awake() 
    {
        cameraAnimator = GetComponent<Animator>();

        Instance = this;

        MonsterDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();

    }

    private void Start()
    {
        fader = FindObjectOfType<Fader>();

        battleSystem.OnBattleOver += EndBattle;
        partyScreen.Init();

        DialogManager.Instance.OnDialogStart += () => gameState = GameState.Dialog;
        DialogManager.Instance.OnDialogEnd += () => 
        {   
            if(gameState == GameState.Dialog) //we need this if statement for events we go from dialogue to battle
                gameState = GameState.OverWorld;
        };
    }

    private void Update()
    {

        if(gameState == GameState.OverWorld)
        {
            playerController.HandleUpdate();

        }
        else if(gameState == GameState.Battle)
        {
            //Battle is ongoing place any logic that needs to be handled in update system for battle system here. Currently we have none.
            return;
        }
        else if(gameState == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    public void PauseGame(bool pause)
    {

        if(pause)
        {
            stateBeforePause = gameState;
            gameState = GameState.Paused;
        }
        else
        {
            gameState = stateBeforePause;
        }

    }

    public void ShowPartyScreen()
    {
        if(gameState == GameState.Inventory) //if the party screen is call during the inventory state we will keep the game state in inventory.
        {
            partyScreen.gameObject.SetActive(true);
            return;
        }

        previousState = gameState;
        gameState = GameState.PartyManagement;
        partyScreen.gameObject.SetActive(true);
    }
    public void ClosePartyScreen()
    {
        if(gameState == GameState.Inventory) //need to keep inventory state as is when using items
        {
            partyScreen.gameObject.SetActive(false);
            return;    
        }

        gameState = previousState;
        partyScreen.gameObject.SetActive(false);
        
    }

    public void ShowInventoryScreen()
    {
        previousState = gameState;
        gameState = GameState.Inventory;
        inventoryScreen.gameObject.SetActive(true);
        
    }
    public void CloseInventoryScreen()
    {
        gameState = previousState;
        inventoryScreen.gameObject.SetActive(false);
        
    }

    public void StartMonsterBattle()
    {
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonsters = CurrentScene.GetComponent<MonsterParty>();
        
        
        for(int i = 0; i < 2; ++i)
        {
            var wildMonster = CurrentScene.GetComponent<MapArea>().GetRandomWildMonster();
            
            var wildMonsterCopy =  new Monster(wildMonster.Base, wildMonster.Level);

            wildMonsters.Monsters.Add(wildMonsterCopy);
        }
        
        battleSystem.StartWildMonsterBattle(playerParty, wildMonsters); 
    }

    SummonerController summoner;
    public void StartSummonerBattle(SummonerController summonerController)
    {
        
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);

        

        summoner = summonerController;
        var playerParty = playerController.GetComponent<MonsterParty>();
        var summonerParty = summonerController.GetComponent<MonsterParty>();

        battleSystem.StartSummonerBattle(playerParty, summonerParty);
    }

    public void OnEnterSummonerFOV(SummonerController summoner)
    {
        gameState = GameState.CutScene;
        StartCoroutine(summoner.TriggerMageBattle(playerController));
    }
    
    private void EndBattle(bool won)
    {

        if(summoner != null && won) //if summoner battle
        {
            summoner.BattleLost();
            summoner = null;
        }

        if(!won) //if player lost
        {
            StartCoroutine(ReloadLastSave());
        }

        gameState = GameState.OverWorld;
        battleSystem.gameObject.SetActive(false);
        

    }

    public IEnumerator ReloadLastSave()
    {
        yield return fader.FadeIn(1f);
        SavingSystem.i.Load("saveSlot1");
        yield return fader.FadeOut(0.5f);
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }

    public GameState GameState => gameState;

    
        
    

}
