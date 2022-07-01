using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjects : MonoBehaviour
{
    [SerializeField] QuestBase questToCheck;
    [SerializeField] ObjectAction onQuestStart;
    [SerializeField] ObjectAction onQuestComplete;

    QuestTracker questTracker;

    private void Start()
    {
        questTracker = QuestTracker.GetQuestTracker(); 
        questTracker.OnUpdated += UpdateObjectStatus;

        UpdateObjectStatus();
    }

    private void OnDestroy() 
    {
        questTracker.OnUpdated -= UpdateObjectStatus;   
    }
    
    public void UpdateObjectStatus()
    {
        //Debug.Log($"{questToCheck.QuestName}" + questTracker.IsCompleted(questToCheck.QuestName));

        if(onQuestStart != ObjectAction.DoNothing && questTracker.IsStarted(questToCheck.QuestName))
        {
            foreach(Transform child in transform) //loops through all object that are children of this quest object
            {
                if(onQuestStart == ObjectAction.Enable)
                {
                    child.gameObject.SetActive(true);

                    var savableEntity = child.GetComponent<SavableEntity>();
                    if(savableEntity != null)
                    {
                        SavingSystem.i.RestoreEntity(savableEntity);
                    }
                }
                else if(onQuestStart == ObjectAction.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        if(onQuestComplete != ObjectAction.DoNothing && questTracker.IsCompleted(questToCheck.QuestName))
        {
            foreach(Transform child in transform) //loops through all object that are children of this quest object
            {
                if(onQuestComplete == ObjectAction.Enable)
                {
                    child.gameObject.SetActive(true);

                    var savableEntity = child.GetComponent<SavableEntity>();
                    if(savableEntity != null)
                    {
                        SavingSystem.i.RestoreEntity(savableEntity);
                    }
                    
                }
                else if(onQuestComplete == ObjectAction.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }

    public enum ObjectAction {DoNothing ,Enable, Disable}
}
