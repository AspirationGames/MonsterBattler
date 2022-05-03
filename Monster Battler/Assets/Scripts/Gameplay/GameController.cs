using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene, Paused, PartyScreen, InventoryScreen}
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

    

    public SceneDetails CurrentScene {get; private set;}

    public SceneDetails PreviousScene {get; private set;}

    public static GameController Instance {get; private set;}

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
        previousState = gameState;
        gameState = GameState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerController.GetComponent<MonsterParty>().Monsters);
    }
    public void ClosePartyScreen()
    {
        gameState = previousState;
        partyScreen.gameObject.SetActive(false);
        
    }

    public void ShowInventoryScreen()
    {
        previousState = gameState;
        gameState = GameState.InventoryScreen;
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

        if(summoner != null && won == true) //if summoner battle
        {
            summoner.BattleLost();
            summoner = null;
        }

        gameState = GameState.OverWorld;
        battleSystem.gameObject.SetActive(false);
        

    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }

    public GameState GameState => gameState;

    
        
    

}
