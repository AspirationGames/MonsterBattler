using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStartSelection : MonoBehaviour, ISavable
{
    [SerializeField] List<Monster> starterMonsters;
    [SerializeField] string introText;
    [SerializeField] string selectionText;

    int selectedChoice = 0;
    bool recievedStarterMonster = false;

    public IEnumerator SelectStartPokemon(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialogText(introText);

        List<string> choices = new List<string>();
        foreach(Monster monster in starterMonsters)
        {
            choices.Add(monster.Base.MonsterName);
        };

        //input required below
        //yield return DialogManager.Instance.ShowDialogText(
            //selectionText,
            //waitForInput: false,
            //choices: choices,
            //onChoiceSelected: choiceIndex => selectedChoice = choiceIndex
       // );

        Monster monsterToStartWith = starterMonsters[selectedChoice];

        monsterToStartWith.Init();
        player.GetComponent<MonsterParty>().AddMonster(monsterToStartWith);

        recievedStarterMonster = true;

        //AudioManager.Instance.PlaySfx(AudioId.PokemonObtained, pauseMusic: true);

        string dialogText = $"{player.Name} received {monsterToStartWith.Base.MonsterName}";
        yield return DialogManager.Instance.ShowDialogText(dialogText);
    }

    public bool ReceivedStarterMonster()
    {
        return recievedStarterMonster;
    }

    public object CaptureState()
    {
        return recievedStarterMonster;
    }

    public void RestoreState(object state)
    {
        recievedStarterMonster = (bool)state;
    }
}
