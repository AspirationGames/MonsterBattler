using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene, Paused}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    Animator cameraAnimator;
    GameState gameState;
    GameState stateBeforePause;

    public SceneDetails CurrentScene {get; private set;}

    public SceneDetails PreviousScene {get; private set;}

    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        cameraAnimator = GetComponent<Animator>();

        Instance = this;

        ConditionsDB.Init();

    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

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
