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

    [SerializeField] LayerMask portalLayer;

    [SerializeField] LayerMask triggersLayer;
    [SerializeField] LayerMask ledgeLayer;

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

     public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask TriggerableLayers
    {
        get => monsterEncountersLayer | fovLayer | portalLayer | triggersLayer;
    }

    public LayerMask LedgeLayer
    {
        get => ledgeLayer;
    }


}
