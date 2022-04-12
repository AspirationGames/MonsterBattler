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
    public void Interact(Transform initiator)
    {   
        if(npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine( DialogManager.Instance.ShowDialog(dialog, () => 
            {
                idleTime = 0f;
                npcState = NPCState.Idle;
            }));
        }
        
    }

    public void Update()
    {

        if(npcState == NPCState.Idle)
        {
            idleTime += Time.deltaTime;
            if(idleTime > timeBetweenPattern)
            {
                idleTime = 0f;
                if (movementPattern.Count > 0)
                {
                   StartCoroutine(Walk());
                   
                }
                
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        npcState = NPCState.Walking;

        var oldPosition = transform.position;

        yield return character.Move(movementPattern[currentPattern]);
       
        if(transform.position != oldPosition) //we are checking to make sure NPC moved in the event it was blocked by something we will not increment the movement pattern
        {
            currentPattern = (currentPattern+ 1) % movementPattern.Count;
        }

        npcState = NPCState.Idle;
    }
    
}

public enum NPCState {Idle, Walking, Dialog}
