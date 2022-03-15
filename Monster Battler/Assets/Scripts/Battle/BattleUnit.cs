using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] public bool isPlayerMonster;

    public Monster Monster {get; set;} //We create a property to store the monster we created in the setup

    void Start()
    {
        
    }
    public void Setup(Monster monster)
    {
       Monster =  monster;

       //Debug.Log(Monster.Base.MonsterName);

       if(isPlayerMonster)
       {
           GetComponent<Image>().sprite = Monster.Base.BackSprite;
       }
       else
       {
           GetComponent<Image>().sprite = Monster.Base.FrontSprite;
       }
    }


}
