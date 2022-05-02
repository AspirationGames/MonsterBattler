using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
   static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        var movesArray = Resources.LoadAll<MoveBase>(""); //loads all MoveBase scriptable objects in the Resources folder

        foreach(var move in movesArray)
        {
            
            if(moves.ContainsKey(move.MoveName)) //if the dictionary already contains the name of the move it is trying to add
            {
                Debug.Log($"Two moves have the {move.MoveName} unable to add duplicate, check game object named {move.name}");

            }

            moves[move.MoveName] = move; //adds each move to the dictionary
        }
    }

    public static MoveBase GetMoveByName(string moveName)
    {
        if(!moves.ContainsKey(moveName))
        {
            Debug.Log($"moves with name {moveName} doese not exist.");
            return null;
        }

        return moves[moveName];
    }
    
}
    
