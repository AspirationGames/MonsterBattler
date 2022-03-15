using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] HPBar hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    Monster monster;
    public void SetData(Monster mon)
    {
        monster = mon;

        nameText.text = monster.Base.MonsterName;
        levelText.text = "Lvl " + monster.Level.ToString();
        hpBar.SetHP((float) monster.HP / monster.MaxHP);
        hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();

    }

    public IEnumerator UpdateHP()
    {
       yield return hpBar.SetHPSmooth((float) monster.HP / monster.MaxHP);
       hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();
    }


}
