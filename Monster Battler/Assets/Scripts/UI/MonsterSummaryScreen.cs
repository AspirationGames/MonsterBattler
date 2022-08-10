using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterSummaryScreen : MonoBehaviour
{
    [SerializeField] Image monsterImage;
    [SerializeField] TextMeshProUGUI[] baseStats;
    [SerializeField] TextMeshProUGUI[] naturalAffinities;
    [SerializeField] TextMeshProUGUI[] BondPointValues;
    [SerializeField] TextMeshProUGUI[] statTotals;
    [SerializeField] TextMeshProUGUI bondPointsTMP;

    Monster monster;
    int unspentBondPoints;
    int investedBondPoints;
    int[] pointsPendingConfirmation = new int[6];

    public void SetMonsterData(Monster monster)
    {
        this.monster = monster;
        monsterImage.sprite = monster.Base.FrontSprite;
        investedBondPoints = 0; //clears previously calculated investment values

        for(int i = 0; i < 6; i++)
        {
            
            Stat stat =  (Stat)i;

            baseStats[i].text = monster.BaseStats[stat].ToString();
            naturalAffinities[i].text = monster.NaturalAffinities[stat].ToString();
            BondPointValues[i].text = monster.BondPointValues[stat].ToString();
            investedBondPoints += monster.BondPointValues[stat]; //sums all invested development points
            statTotals[i].text = monster.Stats[stat].ToString();
        }

        //Calc Dev Points Availalbe
        unspentBondPoints = monster.BondPoints - investedBondPoints;
        bondPointsTMP.text = unspentBondPoints.ToString();
        
    }

    public void IncreaseBondPoints(Transform bpTransform)
    {
        int statIndex = bpTransform.GetSiblingIndex();
        Debug.Log(statIndex);
        //update stat development value
        Stat stat =  (Stat)statIndex;
        
        if(unspentBondPoints > 0) //if the player has available points
        {
            monster.BondPointValues[stat] ++;
            pointsPendingConfirmation[statIndex] ++; //keeps track of points pending confirmation per index
            
        }
        else
        {
            print("you do not have bond points to spend");
        }

        RecalculateMonsterData();
    }

    public void DecreaseBondPoints(Transform bpTransform)
    {
        int statIndex = bpTransform.GetSiblingIndex();
        
        //update stat development value
        Stat stat =  (Stat)statIndex;

        if(pointsPendingConfirmation[statIndex] > 0)
        {
            monster.BondPointValues[stat] --;
            pointsPendingConfirmation[statIndex] --; //keeps track of points pending confirmation per index
            
        }
        else
        {
            print("you do not have bond points to spend");
        }

        RecalculateMonsterData();
    }

    private void RecalculateMonsterData()
    {
        monster.CalculateStats();
        SetMonsterData(monster);
    }

    public void ResetPoints()
    {
        for(int i = 0; i < 6; i++)
        {
            if(pointsPendingConfirmation[i] > 0)
            {
                monster.BondPointValues[(Stat)i] -= pointsPendingConfirmation[i]; // removes any invested points from the stat
                pointsPendingConfirmation[i] = 0;
            }
        }

        RecalculateMonsterData();
    }

    public void ConfirmPointInvestment()
    {
        for(int i = 0; i < 6; i++)
        {
            if(pointsPendingConfirmation[i] > 0)
            {
                pointsPendingConfirmation[i] = 0; //removes all pending points confirming the investment
            }
        }
    }
    
    public void CloseSummaryScreen()
    {
        gameObject.SetActive(false);
    }
}
