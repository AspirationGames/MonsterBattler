using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    Animator cameraAnimator;
    GameState gameState;

    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        cameraAnimator = GetComponent<Animator>();

        Instance = this;

        ConditionsDB.Init();

    }

    private void Start()
    {
        playerController.OnEncounter += StartWildMonsterBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnMageEncounter += (Collider2D summonerCollider) => 
        {
            var summonerController = summonerCollider.GetComponentInParent<SummonerController>();
            
            if(summonerCollider != null)
            {
                gameState = GameState.CutScene;
                StartCoroutine(summonerController.TriggerMageBattle(playerController));
            }
        };

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

    private void StartWildMonsterBattle()
    {
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonsters = FindObjectOfType<MapArea>().GetComponent<MonsterParty>();
        
        
        for(int i = 0; i < 2; ++i)
        {
            var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildMonster();
            
            var wildMonsterCopy =  new Monster(wildMonster.Base, wildMonster.Level);

            wildMonsters.Monsters.Add(wildMonsterCopy);
        }
        
        battleSystem.StartWildMonsterBattle(playerParty, wildMonsters);
        wildMonsters.Monsters.Clear(); //clear wild monster party after starting battle. 
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

    public GameState GameState => gameState;

    
        
    

}
