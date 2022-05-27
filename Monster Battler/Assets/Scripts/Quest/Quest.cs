using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestBase QBase {get; private set; }

    public QuestState QState {get; private set;}

    
    public Quest(QuestBase _base)
    {
        QBase = _base;
    }

    public IEnumerator StartQuest()
    {
        QState = QuestState.InProgress;

        yield return DialogManager.Instance.ShowDialog(QBase.StartDialog);
    }

    public IEnumerator CompleteQuest(Transform player)
    {
        QState = QuestState.Completed;

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

public enum QuestState
{
    none,
    InProgress,
    Completed
}