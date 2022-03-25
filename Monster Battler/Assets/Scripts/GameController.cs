using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ TeamSelect, Battle}
public class GameController : MonoBehaviour
{
    GameState gameState;

    private void Awake() 
    {
        ConditionsDB.Init();
    }

    private void start()
    {
        
    }
    

    
        
    

}
