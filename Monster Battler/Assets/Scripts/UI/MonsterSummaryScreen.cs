using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterSummaryScreen : MonoBehaviour
{
    [SerializeField] Image monsterImage;
    [SerializeField] TextMeshProUGUI[] naturalValues;
    [SerializeField] TMP_InputField[] developmentalValues;
    [SerializeField] TextMeshProUGUI[] statTotals;
    [SerializeField] Slider[] developmentalValueSliders;

    public void SetMonsterData(Monster monster)
    {
        monsterImage.sprite = monster.Base.FrontSprite;

        for(int i = 0; i < 6; i++)
        {
            print(i);
            Stat stat =  (Stat)i;
            naturalValues[i].text = monster.NaturalAffinities[stat].ToString();
            developmentalValues[i].text = monster.DevelopmentValues[stat].ToString();
            statTotals[i].text = monster.Stats[stat].ToString();
        }

    }
    
}
