using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] HPBar hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    Monster monster;

    public void Init(Monster mon)
    {
        monster = mon;
        UpdateData();

        monster.OnHPChanged += UpdateData;
        monster.OnLevelChanged += UpdateData;
        monster.OnStatusChanged += UpdateData;
        

    }

    void UpdateData()
    {

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
