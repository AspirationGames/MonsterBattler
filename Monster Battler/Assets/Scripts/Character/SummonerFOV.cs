using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.OnEnterSummonerFOV(GetComponentInParent<SummonerController>());
    }
}
