using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGiver : MonoBehaviour, ISavable
{
    [SerializeField] Monster giveMonster;
    [SerializeField] Dialog dialog;

    bool monsterGiven = false;

    public IEnumerator GiveMonster(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        giveMonster.Init();
        player.GetComponent<MonsterParty>().AddMonster(giveMonster);
        monsterGiven = true;

        AudioManager.i.PlaySFX(AudioID.MonsterObtained, true);
        string dialogText = $"{player.Name} recieved a {giveMonster.Base.MonsterName}.";
        yield return DialogManager.Instance.ShowDialogText(dialogText);    

    }

    public bool CanGiveMonster()
    {
        return giveMonster != null && !monsterGiven;
    }

    public object CaptureState()
    {
        return monsterGiven;
    }

    public void RestoreState(object state)
    {
        monsterGiven = (bool)state;
    }
}
