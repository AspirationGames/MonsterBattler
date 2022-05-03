using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New recovery item", order = 1)]
public class RecoveryItem : ItemBase
{
    [Header("Healing")]
    [SerializeField] int healAmount;
    [SerializeField] bool restoresMaxHP;
    
    [Header("AP Recovery")]
    [SerializeField] int apAmount;
    [SerializeField] bool restoresMaxAP;

    [Header("Cure Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

}
