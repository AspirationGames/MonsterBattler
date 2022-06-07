using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    int selectedChoiceIndex = 0;
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        yield return DialogManager.Instance.ShowDialog(dialog, new List<string>() {"Yes", "No"}, 
        (choiceIndex) => selectedChoiceIndex = choiceIndex );

        //the above action will set the selectedchoiceIndex variable in this script which we can use to trigger an if condition within this script itself
        if(selectedChoiceIndex == 0) //player selected yes
        {
            yield return Fader.Instance.FadeIn(0.5f);

            var playerParty = player.GetComponent<MonsterParty>();

            foreach(Monster monster in playerParty.Monsters) //heals each monster in the party
            {
                monster.Heal();
                monster.CureStatus();
            }

            playerParty.PartyUpdated();

            yield return Fader.Instance.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText("Thank you for waiting have a nice day.");
        } 
        else if(selectedChoiceIndex == 1) //player selected no
        {
            yield return DialogManager.Instance.ShowDialogText("Come back if you change your mind.");
        }

        

    }


}
