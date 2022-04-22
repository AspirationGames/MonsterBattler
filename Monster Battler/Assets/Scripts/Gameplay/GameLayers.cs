using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers i {get; set;}

    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask monsterEncountersLayer;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] LayerMask fovLayer;

    void Awake()
    {
        i = this;
    }

    public LayerMask SolidObjectsLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask MonsterEncountersLayer
    {
        get => monsterEncountersLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask Fovlayer
    {
        get => fovLayer;
    }


}