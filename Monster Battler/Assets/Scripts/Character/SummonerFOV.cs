using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerFOV : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatedly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.CharacterAnimator.IsMoving = false;
        GameController.Instance.OnEnterSummonerFOV(GetComponentInParent<SummonerController>());
    }
}
