using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle, Dialog, CutScene}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    GameState gameState;

    public static GameController Instance {get; private set;}

    private void Awake() 
    {
        Instance = this;

        ConditionsDB.Init();

    }

    private void Start()
    {
        playerController.OnEncounter += StartBattle;
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

    private void StartBattle()
    {
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        battleSystem.StartBattle();
    }
    
    private void EndBattle(bool won)
    {
        gameState = GameState.OverWorld;
        battleSystem.gameObject.SetActive(false);
        

    }

    public GameState GameState => gameState;

    
        
    

}
