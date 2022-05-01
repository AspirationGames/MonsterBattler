using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.CharacterAnimator.IsMoving = false;
        GameController.Instance.OnEnterSummonerFOV(GetComponentInParent<SummonerController>());
    }
}
