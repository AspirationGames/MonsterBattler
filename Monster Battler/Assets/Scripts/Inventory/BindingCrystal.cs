using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Binding Crystal", order = 1)]
public class BindingCrystal : ItemBase
{
    [SerializeField] float catchRateModifier = 1;
    [SerializeField] Sprite summoningCircleSprite;

    public override bool CanUsedOutsideBattle => false;
    public override bool CanUse(Monster monster)
    {
        return true;
    }

    public override void Use(Monster monster)
    {
        Debug.Log("you should not call thsi method for binding crystals as their use method is within the battle system.");
    }

    public float CatchRateModifier => catchRateModifier;
    public Sprite SummoningCircleSprite => summoningCircleSprite;
}
