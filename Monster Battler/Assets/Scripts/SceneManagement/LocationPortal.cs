using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Teleports player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    Fader fader;
    
    public bool TriggerRepeatedly => false;
    
    private void Start() 
    {
        fader = FindObjectOfType<Fader>(); //this has to be in start because the Fader is instantiated in Awake
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.CharacterAnimator.IsMoving = false;
        StartCoroutine(Teleport());

    }

    IEnumerator Teleport()
    {


        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);
        
        var destinationPortal = FindObjectsOfType<LocationPortal>().First(x => x !=this && x.destinationPortal == this.destinationPortal); //This will retun the portal in the current scene
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
