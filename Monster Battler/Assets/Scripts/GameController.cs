using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ TeamSelect, Battle}
public class GameController : MonoBehaviour
{
    GameState gameState;


    private void Update()
    {
        if (gameState == GameState.TeamSelect)
        {
            //give player controller control
        }
        else if(gameState == GameState.Battle)
        {
            //give battle system control
        }

    }
    

    
        
    

}
