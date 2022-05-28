using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable] //using this to be able to show quest tracker in inspector
public class QuestTracker : MonoBehaviour
{
    List<Quest> activeQuest = new List<Quest>();
    List<Quest> completedQuests = new List<Quest>();

    public event Action OnUpdated;

    public void AddActiveQuest(Quest quest)
    {
        if(!activeQuest.Contains(quest))
        {
            activeQuest.Add(quest);
        }

        OnUpdated?.Invoke();
        
    }

    public bool WasQuestStarted(string questName)
    {
      var questStarted = activeQuest.FirstOrDefault(q => q.QBase.QuestName == questName)?.QStatus;

      if(questStarted != null)
      {
          return true;
      }
      
      return false;
    }

    public bool WasQuestCompeted(string questName)
    {
        var questCompleted = completedQuests.FirstOrDefault(q => q.QBase.QuestName == questName)?.QStatus;
        
        

      if(questCompleted != null)
      {
          return true;
      }
      
      return false;
    }

    public void MarkQuestComplete(Quest quest)
    {
        if(activeQuest.Contains(quest))
        {
            activeQuest.Remove(quest);
           
        }
        if(!completedQuests.Contains(quest))
        {
            completedQuests.Add(quest);
           
        }

        OnUpdated?.Invoke();
    }

    public static QuestTracker GetQuestTracker()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestTracker>();
    }
}
