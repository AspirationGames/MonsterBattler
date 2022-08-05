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
    [SerializeField] GameObject bondExpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image statusImage;
    [SerializeField] List<Sprite> statusSprites;

    Monster monster;


    public void SetData(Monster mon)
    {
        if(monster != null) //clears the previously subsribed monster from the below events
        {
            monster.OnStatusChanged -= SetStatusImage;
            monster.OnHPChanged -= UpdateHP;
        }

        monster = mon;

        nameText.text = monster.Base.MonsterName;
        SetLevel();
        hpBar.SetHP((float) monster.HP / monster.MaxHP);
        hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();
        SetEXP();

        SetStatusImage();
        monster.OnStatusChanged += SetStatusImage;
        monster.OnHPChanged += UpdateHP;

    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + monster.Level.ToString();

    }

    public void SetEXP()
    {
        if (expBar == null) return;
        if (bondExpBar == null) return;

        float normalizedExp = GetNormalizedExp();
        float normalizeBondExp = GetNormalizedBondExp();
        print(normalizeBondExp);
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
        bondExpBar.transform.localScale = new Vector3(normalizeBondExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool expFull=false)
    {
        if(expBar == null) yield break;

        if(expFull)
        {
            //reset bar
            expBar.transform.localScale = new Vector3(0,1,1);

        }

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    public IEnumerator SetBondExpSmooth(bool expFull=false)
    {
        if(expFull)
        {
            //reset bar
            bondExpBar.transform.localScale = new Vector3(0,1,1);

        }

        float normalizedExp = GetNormalizedBondExp();
        yield return bondExpBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currentLevelExp = monster.Base.GetExpForLevel(monster.Level);
        int nextLevelExp = monster.Base.GetExpForLevel(monster.Level + 1);

        float normalizedExp = (float)(monster.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);

    }
    float GetNormalizedBondExp()
    {
        int currentBondExp = monster.GetExpForBondPoints();
        int nextBondLevelExp = monster.GetExpForBondPoints(true);

        float normalizeBondExp = (float)(monster.DevExp - currentBondExp) / (nextBondLevelExp - currentBondExp);
        
        return Mathf.Clamp01(normalizeBondExp);  
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float) monster.HP / monster.MaxHP);
        hpText.text = monster.HP.ToString() + "/" + monster.MaxHP.ToString();
       
    }

    public IEnumerator WaitForHPUpdate()
    {

        yield return new WaitUntil(() => hpBar.HPisUpdating == false); //waits until HP update is complete
    }

    public void ClearData()
    {
        if(monster != null) //clears the previously subsribed monster from the below events
        {
            monster.OnStatusChanged -= SetStatusImage;
            monster.OnHPChanged -= UpdateHP;
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
