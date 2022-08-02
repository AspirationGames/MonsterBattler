using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterSummaryScreen : MonoBehaviour
{
    [SerializeField] Image monsterImage;

    [SerializeField] Transform StatTotals;
    [SerializeField] Transform naturalSkillAffinities;
    [SerializeField] Transform developmentalSkillPoints;
    [SerializeField] Transform developmentalSkillSliders;

    public void SetMonsterData(Monster monster)
    {
        monsterImage.sprite = monster.Base.FrontSprite;

        foreach(Transform statTotal in StatTotals)
        {
            TextMeshProUGUI naturalSkillTMP = statTotal.gameObject.GetComponent<TextMeshProUGUI>();
            
            if(statTotal.GetSiblingIndex() == 0)
            {
                naturalSkillTMP.text = monster.MaxHP.ToString(); 
            }
            else
            {
                int index = statTotal.GetSiblingIndex() - 1; // note that our stat dictionary doesn't include HP so at index 0 the first stat is attack
                Stat stat =  (Stat)index; 
                naturalSkillTMP.text = monster.Stats[stat].ToString();
            }

        }
    }
}
