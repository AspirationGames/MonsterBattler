using System;
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

    [SerializeField] TextMeshProUGUI messageText;

    Monster monster;

    [SerializeField] Image statusImage;
    [SerializeField] List<Sprite> statusSprites;

    public static event Action<PartyMemberUI> UIHover;
    public static event Action<PartyMemberUI> UISelected;
    public void Init(Monster mon)
    {
        monster = mon;
        UpdateData();
        SetStatusImage();
        SetMessageText("");

        monster.OnHPChanged += UpdateData;
        monster.OnLevelChanged += UpdateData;
        monster.OnStatusChanged += SetStatusImage;
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

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    public void OnHover()
    {
        UIHover?.Invoke(this);
    }

    public void OnSelect()
    {
        UISelected?.Invoke(this);

    }
    
}
