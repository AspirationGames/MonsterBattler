using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Image monsterImage;
    PartyMemberUI[] memberSlots;
    List<Monster> monsters;
    MonsterParty playerParty;
    public event Action<Monster> monsterSelected;
    public event Action screenClosed;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        playerParty = MonsterParty.GetPlayerParty();
        SetPartyData();
        playerParty.OnUpdated += SetPartyData;
    }

    private void OnEnable() 
    {
        PartyMemberUI.UIHover += PartyMemberHover;
        PartyMemberUI.UISelected += PartyMemberSelected;
    }

    private void OnDisable() 
    {
        PartyMemberUI.UIHover -= PartyMemberHover;
        PartyMemberUI.UISelected -= PartyMemberSelected;
    }

    public void SetPartyData()
    {
        monsters = playerParty.Monsters;

        for (int i=0; i < memberSlots.Length; i++)
        {
            if(i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(monsters[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false); //disables party slots that are not in use. Might be useful in order to re-use asset for pre battle party selection? 
            }
        }

        SetMonsterImage(playerParty.Monsters[0]);
    }

    public void SetMonsterImage(Monster monster)
    {
        monsterImage.sprite = monster.Base.FrontSprite;
    }

    public void PartyMemberHover(PartyMemberUI partyMemberUI)
    {
        int partyMemberIndex = partyMemberUI.transform.GetSiblingIndex();
        Monster selectedMonster = playerParty.Monsters[partyMemberIndex];
        
        SetMonsterImage(selectedMonster);
    }

    public void PartyMemberSelected(PartyMemberUI partyMemberUI)
    {
        int selectedMonsterIndex = partyMemberUI.transform.GetSiblingIndex();
        Monster selectedMonster = playerParty.Monsters[selectedMonsterIndex];
        
        if(GameController.Instance.GameState == GameState.PartyManagement)
        {
            //Show Monster Selected UI
            

        }
        
        print("monster selected");
        monsterSelected?.Invoke(selectedMonster); //event to notify other scripts that require party member selection i.e. inventory

    }
    public void ShowIfSpellisUsable(SpellItem spellItem)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
           string message = spellItem.CanLearnMove(monsters[i]) ? "ABLE !" : "NOT ABLE!";

            if(monsters[i].HasMove(spellItem.MoveToLearn))
            {
                message = "ALREADY LEARNED";
            }
            
            
            memberSlots[i].SetMessageText(message);
        }
    }

    public void ClearUIMessages()
    {
        for (int i = 0; i < monsters.Count; i++)
        {   
            memberSlots[i].SetMessageText("");
        }
    }

    public void ClosePartyScreen()
    {
        screenClosed?.Invoke(); 

        if(GameController.Instance.GameState == GameState.Battle)
        {
            return;
        }
        if(GameController.Instance.GameState == GameState.Inventory)
        {
            gameObject.SetActive(false);
        }
        if(GameController.Instance.GameState == GameState.PartyManagement)
        {
            GameController.Instance.BackToPauseMenu();
            gameObject.SetActive(false);
        }

        ClearUIMessages();
    }



    
}
