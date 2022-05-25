using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEncounter : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] float encoutnerRate = 10f;

    public bool TriggerRepeatedly => true;
    public void OnPlayerTriggered(PlayerController player)
    {
        if(UnityEngine.Random.Range(1, 101) <= encoutnerRate) //10% chance of random monster encounter
        {
            player.Character.CharacterAnimator.IsMoving = false;
            GameController.Instance.StartMonsterBattle();

        }
    }

}
