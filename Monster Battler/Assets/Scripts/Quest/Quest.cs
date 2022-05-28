using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestBase QBase {get; private set; }

    public QuestStatus QStatus {get; private set;}

    
    public Quest(QuestBase _base)
    {
        QBase = _base;
    }

    public IEnumerator StartQuest()
    {
        QStatus = QuestStatus.InProgress;

        yield return DialogManager.Instance.ShowDialog(QBase.StartDialog);

        var questList = QuestTracker.GetQuestTracker();
        questList.AddActiveQuest(this);
    }

    public IEnumerator CompleteQuest(Transform player)
    {
        QStatus = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(QBase.CompleteDialog);

        //give rewards
        var inventory = Inventory.GetInventory();
        if(QBase.QuestItem != null)
        {
            inventory.DecreaseQuantity(QBase.QuestItem);
            
        }
        if(QBase.RewardItem != null)
        {
            inventory.AddItem(QBase.RewardItem);
            var playerName = player.GetComponent<PlayerController>().Name;
            yield return DialogManager.Instance.ShowDialogText($"{playerName} recieved {QBase.RewardItem.ItemName}");
        }

        var questList = QuestTracker.GetQuestTracker();
        questList.MarkQuestComplete(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if(QBase.QuestItem != null)
        {   
            return inventory.HasItem(QBase.QuestItem);
            
        }

        return true; //defualt
    }
}

public enum QuestStatus
{
    none,
    InProgress,
    Completed
}