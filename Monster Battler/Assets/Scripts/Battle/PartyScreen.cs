using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    PartyMemberUI[] memberSlots;
    List<Monster> monsters;

    MonsterParty playerParty;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        playerParty = MonsterParty.GetPlayerParty();
        SetPartyData();

        playerParty.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        monsters = playerParty.Monsters;

        for (int i=0; i < memberSlots.Length; i++)
        {
            if(i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(monsters[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false); //disables party slots that are not in use. Might be useful in order to re-use asset for pre battle party selection? 
            }
        }
    }

    public void OnSelect()
    {
        if(GameController.Instance.GameState == GameState.Inventory)
        {
            //use item on selected monster
        }

    }

    public void OnBack()
    {
        GameController.Instance.ClosePartyScreen();

    }



    
}
