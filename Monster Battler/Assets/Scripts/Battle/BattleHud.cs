using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] HPBar hpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image statusImage;
    [SerializeField] List<Sprite> statusSprites;

    Monster monster;
    public void SetData(Monster mon)
    {
        monster = mon;

        nameText.text = monster.Base.MonsterName;
        levelText.text = "Lvl " + monster.Level.ToString();
        hpBar.SetHP((float) monster.HP / monster.MaxHP);
        hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();

        SetStatusImage();
        monster.OnStatusChaged += SetStatusImage;

    }

    public IEnumerator UpdateHP()
    {
        if(monster.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float) monster.HP / monster.MaxHP);
            hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();
            monster.HpChanged = false;
        }
       
    }

    public void SetStatusImage()
    {
        if (monster.Status == null)
        {
            statusImage.enabled = false;
        }
        else
        {
            statusImage.enabled = true;
            switch(monster.Status.Id)
            {
            case ConditionID.psn:
                statusImage.sprite = statusSprites[0];
                break;
            case ConditionID.brn:
                statusImage.sprite = statusSprites[1];
                break;
            case ConditionID.slp:
                statusImage.sprite = statusSprites[2];
                break;
            case ConditionID.par:
                statusImage.sprite = statusSprites[3];
                break;
            case ConditionID.frz:
                statusImage.sprite = statusSprites[4];
                break;
            default:
                statusImage.enabled = false;
                break;

            }
        }
       
        
    }


}
