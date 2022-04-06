using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ OverWorld, Battle}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    GameState gameState;

    private void Awake() 
    {
        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
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
        }
    }

    private void StartBattle()
    {
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
    }
    
    private void EndBattle(bool won)
    {
        gameState = GameState.OverWorld;
        battleSystem.gameObject.SetActive(false);
        battleSystem.StartBattle();

    }

    
        
    

}
