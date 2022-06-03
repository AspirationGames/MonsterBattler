using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene, Paused, PartyManagement, Inventory, Evolution}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryScreen inventoryScreen;

    Animator cameraAnimator;
    GameState gameState;
    GameState stateBeforeDialog;
    GameState stateBeforePause;
    GameState stateBeforeEvolution;

    
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
        ItemDB.Init();
        QuestDB.Init();

    }

    private void Start()
    {

        battleSystem.OnBattleOver += EndBattle;
        partyScreen.Init();

        DialogManager.Instance.OnDialogStart += () => 
        {
            stateBeforeDialog = gameState;
            gameState = GameState.Dialog;
        };
        DialogManager.Instance.OnDialogEnd += () => 
        {   
            if(gameState == GameState.Dialog) //we need this if statement for events we go from dialogue to battle
                gameState = stateBeforeDialog;
        };

        EvolutionManager.i.OnEvolutionStart += () => 
        {
            stateBeforeEvolution = gameState;
            gameState = GameState.Evolution;
        };
        EvolutionManager.i.OnEvolutionStart += () => 
        {
           gameState = stateBeforeEvolution;
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

    public void PartyManagement() //called when managing party from pause menu
    {
        gameState = GameState.PartyManagement;
        partyScreen.gameObject.SetActive(true);
    }

    public void ShowInventoryScreen() //called when opening inventory from pause menu
    {
        gameState = GameState.Inventory;
        inventoryScreen.gameObject.SetActive(true);
        
    }
    public void BackToPauseMenu() //called when closing out of pause menue option
    {
        gameState = GameState.Paused;
        
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
        StartCoroutine(summoner.TriggerSummonerBattle(playerController));
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
        
        var playerParty = playerController.GetComponent<MonsterParty>();
        StartCoroutine(playerParty.CheckForEvolution());

    }

    public IEnumerator ReloadLastSave()
    {
        yield return Fader.Instance.FadeIn(1f);
        SavingSystem.i.Load("saveSlot1");
        yield return Fader.Instance.FadeOut(0.5f);
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }

    public GameState GameState => gameState;

    
        
    

}
