using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> monsters)
    {
        for (int i=0; i < memberSlots.Length; i++)
        {
            if(i < monsters.Count)
            {
                memberSlots[i].SetData(monsters[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false); //disables party slots that are not in use. Might be useful in order to re-use asset for pre battle party selection? 
            }
        }
    }


}
