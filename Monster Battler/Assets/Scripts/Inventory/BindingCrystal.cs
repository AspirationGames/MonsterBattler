using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Binding Crystal", order = 1)]
public class BindingCrystal : ItemBase
{
    public override bool CanUse(Monster monster)
    {
        return base.CanUse(monster);
    }

    public override void Use(Monster monster)
    {
        base.Use(monster);
    }
}
