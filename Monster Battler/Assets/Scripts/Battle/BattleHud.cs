using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
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
        SetEXP();

        SetStatusImage();
        monster.OnStatusChaged += SetStatusImage;

    }

    public void SetEXP()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);

    }

    public IEnumerator SetExpSmooth()
    {
        
        if(expBar == null) yield break;

        float normalizedExp = GetNormalizedExp();
        Debug.Log(normalizedExp);
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currentLevelExp = monster.Base.GetExpForLevel(monster.Level);
        int nextLevelExp = monster.Base.GetExpForLevel(monster.Level + 1);

        float normalizedExp = (float)(monster.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);

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
