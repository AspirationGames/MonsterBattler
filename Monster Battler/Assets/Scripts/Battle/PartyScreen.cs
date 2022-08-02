using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Image monsterImage;
    [SerializeField] MonsterSummaryScreen monsterSummaryScreen;
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
        if(GameController.Instance.GameState == GameState.Dialog) return; //prevents switching monster images when dialog is showing

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
            StartCoroutine(ManagePartyMember(selectedMonster));
            return;
        }
        
        monsterSelected?.Invoke(selectedMonster); //event to notify other scripts that require party member selection i.e. inventory

    }

    IEnumerator ManagePartyMember(Monster monster)
    {
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"What would you like to do with {monster.Base.MonsterName}?", 
        choices: new List<String> {"Monster Summary", "Swap Position", "Use Item", "Give Item to Hold", "Back"}, 
        onChoiceSelectedAction: (int selectionIndex) => selectedChoice = selectionIndex);

        switch(selectedChoice)
        {
            case 0:
                yield return ShowMonsterSummary(monster);
                break;
            case 1:
                yield return SwapMonsterPosition(monster);
                break;
            case 2:
                yield return UseItem(monster);
                break;
            case 3:
                yield return UseItem(monster);
                break;
            case 4:
                yield break;
            default:
                Debug.LogError("No choice selected");
                break;
        }
    }

    IEnumerator ShowMonsterSummary(Monster monster)
    {
        monsterSummaryScreen.gameObject.SetActive(true);
        monsterSummaryScreen.SetMonsterData(monster);
        yield return null;
    }
    IEnumerator SwapMonsterPosition(Monster monster)
    {
        Debug.Log("Swapping monster position");
        //WIP
        yield return null;
    }

    IEnumerator UseItem(Monster monster)
    {
        Debug.Log("Using Item on Monster");
        //WIP
        yield return null;
    }

    IEnumerator GiveMonsterItem(Monster monster)
    {
        Debug.Log("Giving Monster Item to hold");
        //WIP
        yield return null;
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
