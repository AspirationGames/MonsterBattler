using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable] //using this to be able to show quest tracker in inspector
public class QuestTracker : MonoBehaviour, ISavable
{
    List<Quest> questList = new List<Quest>();

    public event Action OnUpdated;

    public void AddQuest(Quest quest)
    {
        if(!questList.Contains(quest))
        {
            questList.Add(quest);
        }

        OnUpdated?.Invoke();
        
    }

    public bool IsStarted(string questName)
    {
      var questStatus = questList.FirstOrDefault(q => q.QBase.QuestName == questName)?.QStatus;
      return questStatus == QuestStatus.InProgress || questStatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = questList.FirstOrDefault(q => q.QBase.QuestName == questName)?.QStatus;
        return questStatus == QuestStatus.Completed;
    }
    public static QuestTracker GetQuestTracker()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestTracker>();
    }

    public object CaptureState()
    {
        return questList.Select(q => q.GetQuestSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;

        if(saveData != null)
        {
            questList = saveData.Select(q => new Quest(q)).ToList();
            OnUpdated?.Invoke();
        }
    }
}
