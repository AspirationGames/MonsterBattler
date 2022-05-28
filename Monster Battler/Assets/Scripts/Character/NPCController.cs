using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    
    [Header("Dialog")]    
    [SerializeField] Dialog dialog;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    
    [Header("Quest")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete; //used when NPCs should complete quest like starter mosnter giver
    
    Quest activeQuest;
    Character character;
    ItemGiver itemGiver;
    MonsterGiver monsterGiver;
    Healer healer;
    NPCState npcState;
    
    float idleTime = 0f;
    int currentPattern = 0;


    void Awake() 
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        monsterGiver = GetComponent<MonsterGiver>();
        healer = GetComponent<Healer>();    
    }
    public IEnumerator Interact(Transform initiator)
    {   
        
        if(npcState == NPCState.Idle)
        {

            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if(questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                quest.CompleteQuest(initiator);
                questToComplete = null;

                Debug.Log($"{quest.QBase.QuestName} was completed");
            }
            if(itemGiver != null && itemGiver.CanGiveItem())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            if(monsterGiver != null && monsterGiver.CanGiveMonster())
            {
                yield return monsterGiver.GiveMonster(initiator.GetComponent<PlayerController>());
            }
            else if(questToStart != null)
            {
                activeQuest = new Quest(questToStart); //creates instance of quest because we plug in the questBase
                yield return activeQuest.StartQuest();
                questToStart = null;
                if(activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null; //clears active quest variable
                }
            }
            else if(activeQuest != null)
            {
                
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
