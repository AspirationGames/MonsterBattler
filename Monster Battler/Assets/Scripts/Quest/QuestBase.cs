using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Create a new Quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] string description;

    [SerializeField] Dialog startDialog;
    [SerializeField] Dialog inProgressDialog;
    [SerializeField] Dialog completedDialog;

    [SerializeField] ItemBase questItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;
    public Dialog StartDialog => startDialog;

    public Dialog InProgressDialog => inProgressDialog?.Lines?.Count > 0 ? inProgressDialog : startDialog;
    public Dialog CompleteDialog => completedDialog;
    public ItemBase QuestItem => questItem;
    public ItemBase RewardItem => rewardItem;


}
