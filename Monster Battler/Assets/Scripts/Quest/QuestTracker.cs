using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();


    public void AddQuest(Quest quest)
    {
        if(!quests.Contains(quest))
        {
            quests.Add(quest);
        }
    }
}
