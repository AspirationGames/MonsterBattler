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
    [SerializeField] TextMeshProUGUI[] naturalValues;
    [SerializeField] TMP_InputField[] developmentalValues;
    [SerializeField] TextMeshProUGUI[] statTotals;
    [SerializeField] Slider[] developmentalValueSliders;
    [SerializeField] TextMeshProUGUI skillPointsTMP;

    Monster monster;
    int skillPoints;

    public void SetMonsterData(Monster monster)
    {
        this.monster = monster;
        monsterImage.sprite = monster.Base.FrontSprite;

        for(int i = 0; i < 6; i++)
        {
            
            Stat stat =  (Stat)i;
            baseStats[i].text = monster.BaseStats[stat].ToString();
            naturalValues[i].text = monster.NaturalAffinities[stat].ToString();
            developmentalValues[i].text = monster.DevelopmentValues[stat].ToString();
            statTotals[i].text = monster.Stats[stat].ToString();
        }

        //TO DO set available skill points
        //skillPoints = availableSkillPoints;
        //skillPointsTMP.text = skillPoints.ToString();
    }

    public void OnValueChanged(TMP_InputField inputField)
    {
        
        print("value changed");
        
        int fieldIndex = inputField.transform.GetSiblingIndex();
        
        //update stat development value
        Stat stat =  (Stat)fieldIndex;
        

        //TO DO figure out best way to update monster values.
        //If we use dictionary like below we will need to change stat calcs to also use dictionary
        //We will also need to consider save system which currently uses individual variables for stat values
        //monster.DevelopmentValues[stat] = int.Parse(inputField.text);
    }
    
}
