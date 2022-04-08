using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    Character character;

    NPCState npcState;
    float idleTime = 0f;
    int currentPattern = 0;

    void Awake() 
    {
        character = GetComponent<Character>();    
    }
    public void Interact()
    {   
        if(npcState == NPCState.Idle)
        {
            StartCoroutine( DialogManager.Instance.ShowDialog(dialog) );
        }
        
    }

    public void Update()
    {
        if(DialogManager.Instance.IsShowing) return;

        if(npcState == NPCState.Idle)
        {
            idleTime += Time.deltaTime;
            if(idleTime > timeBetweenPattern)
            {
                idleTime = 0f;
                if (movementPattern.Count > 0)
                {
                   StartCoroutine(Walk());
                   currentPattern = (currentPattern+ 1) % movementPattern.Count;
                }
                
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        npcState = NPCState.Walking;

        yield return character.Move(movementPattern[currentPattern]);

        npcState = NPCState.Idle;
    }
    
}

public enum NPCState {Idle, Walking}
