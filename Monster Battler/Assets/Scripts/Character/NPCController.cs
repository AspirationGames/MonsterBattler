using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [SerializeField] QuestBase questToStart;
    Character character;
    ItemGiver itemGiver;
    Healer healer;
    NPCState npcState;
    Quest activeQuest;
    float idleTime = 0f;
    int currentPattern = 0;


    void Awake() 
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        healer = GetComponent<Healer>();    
    }
    public IEnumerator Interact(Transform initiator)
    {   
        
        if(npcState == NPCState.Idle)
        {

            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);

            
            if(itemGiver != null && itemGiver.CanGiveItem())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if(questToStart != null)
            {
                activeQuest = new Quest(questToStart); //creates instance of quest because we plug in the questBase
                yield return activeQuest.StartQuest();
                questToStart = null;
            }
            else if(activeQuest != null)
            {
                Debug.Log("active quest");
                if(activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null; //clears active quest variable
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.QBase.InProgressDialog); 
                }
            }
            else if(healer != null) //npc is a healer.. this is place holder code
            {
                yield return healer.Heal(initiator, dialog);
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }
            
            idleTime = 0f;
            npcState = NPCState.Idle;
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
